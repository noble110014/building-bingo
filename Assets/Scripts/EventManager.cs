using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using System.Linq;

public class EventManager : SingletonBase<EventManager>
{
    [Header("Settings")]
    [SerializeField] private int eventInterval = 5; // イベント間の待機時間
    [SerializeField] private int eventTime = 3;     // イベント実行中の時間

    private string eventBuildingID;

    // 現在の残り時間を表示するためのRP
    private readonly ReactiveProperty<int> _currentCountdown = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> CurrentCountdown => _currentCountdown;

    // 現在の状態（イベント中かどうか）を知りたい場合
    public bool IsEventRunning { get; private set; } = false;

    // 非同期処理をキャンセルするためのトークンソース
    private CancellationTokenSource _cts;

    [ContextMenu("DebugStartEvent")]
    public void StartEvent()
    {
        // 既に動いていれば止める（リセット挙動）
        StopEvent();

        // 新しいトークンを発行
        _cts = new CancellationTokenSource();

        // 非同期ループを開始（投げっぱなしにするので async void でOK、または UniTask なら .Forget()）
        // 渡したトークンを使って、StopEvent時に止まれるようにする
        _ = EventLoopAsync(_cts.Token);
    }

    public void StopEvent()
    {
        if (_cts != null)
        {
            _cts.Cancel(); // 処理をキャンセル（中断）
            _cts.Dispose();
            _cts = null;
        }
        IsEventRunning = false;
        _currentCountdown.Value = 0;
        Debug.Log("Event Loop Stopped");
    }

    // メインのループ処理
    private async Task EventLoopAsync(CancellationToken token)
    {
        try
        {
            // 無限ループで繰り返す
            while (!token.IsCancellationRequested)
            {
                // ------------------------------------------------
                // フェーズ1: インターバル（待機時間）
                // ------------------------------------------------
                Debug.Log($"<color=yellow>Next event in {eventInterval} seconds...</color>");
                IsEventRunning = false;

                // カウントダウンしながら待つ
                await CountDownAsync(eventInterval, token);


                // ------------------------------------------------
                // フェーズ2: イベント開始！
                // ------------------------------------------------
                Debug.Log("<color=red>Event STARTED!</color>");
                OnEventStart(); // 関数実行
                IsEventRunning = true;

                // イベント時間分、カウントダウンしながら待つ
                await CountDownAsync(eventTime, token);


                // ------------------------------------------------
                // フェーズ3: イベント終了
                // ------------------------------------------------
                Debug.Log("<color=cyan>Event FINISHED</color>");
                OnEventEnd(); // 関数実行
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた（StopEventが呼ばれた）時の処理
            Debug.Log("Canceled logic safely.");
        }
        finally
        {
            IsEventRunning = false;
        }
    }

    // 指定秒数だけカウントダウンしながら待機するヘルパー関数
    private async Task CountDownAsync(int seconds, CancellationToken token)
    {
        _currentCountdown.Value = seconds;

        while (_currentCountdown.Value > 0)
        {
            // 1秒待つ (UnityのTimeScaleの影響を受けたい場合は UniTask推奨、ここでは標準Task)
            await Task.Delay(1000, token);

            // カウントを減らす
            _currentCountdown.Value--;
        }
    }

    // 実際に実行したい関数A
    private void OnEventStart()
    {
        // 1. '.values' ではなく '.Value' (大文字V) です
        var card = BingoManager.Instance.BingoCard.Value;

        // ガード節：まだカードが生成されていない場合は何もしない
        if (card == null) return;

        var result = card
            // 2. 2次元配列をLINQで扱うために Cast<T> で1列に並べ直す必要があります
            .Cast<BingoSquare>()
            // 3. 条件で絞る (ここでは「開いているマス」から選んでいます)
            .Where(x => !x.IsOpen.Value)
            // 4. ランダムに並べ替え
            .OrderBy(_ => Guid.NewGuid())
            // 5. 最初の1つを取得
            .FirstOrDefault();

        // 6. 見つかった場合のみ実行 (nullチェック)
        if (result != null)
        {
            // 第2引数(Color)が必要なはずなので追加してください（例：青にする）
            BuildingManager.Instance.ChangeColorBuilding(result.ID, Color.blue);
            eventBuildingID = result.ID;

            Debug.Log($"イベント発生: {result.ID} が選ばれました");
        }
    }

    // 実際に実行したい関数B
    private void OnEventEnd()
    {
        BuildingManager.Instance.ChangeColorBuilding(eventBuildingID, Color.white);

    }

    protected override void OnDestroy()
    {
        StopEvent(); // オブジェクト破棄時に確実に止める
        base.OnDestroy();
    }
}