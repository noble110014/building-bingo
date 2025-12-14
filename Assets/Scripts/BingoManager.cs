using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class BingoManager : SingletonBase<BingoManager>
{
    // ビンゴカード全体（5x5のマスデータの配列）
    // 配列自体の入れ替えを検知したい場合はReactivePropertyで包みます
    private ReactiveProperty<BingoSquare[,]> _bingoCard = new ReactiveProperty<BingoSquare[,]>();
    public IReadOnlyReactiveProperty<BingoSquare[,]> BingoCard => _bingoCard;

    [SerializeField] private int bingoCardSquareNum = 5;

    [SerializeField] private BingoCardView bingoCardView;

    public void Initialize()
    {
        bingoCardView.Initialize();
    }

    // カード生成（ここでは穴あき通知は飛びません）
    public void SetBingoCard(string[] ids)
    {
        var nums = ids.ToList();
        var newCard = new BingoSquare[bingoCardSquareNum, bingoCardSquareNum];

        for (int i = 0; i < bingoCardSquareNum; i++)
        {
            for (int j = 0; j < bingoCardSquareNum; j++)
            {
                // 中央のマス
                if (i == 2 && j == 2)
                {
                    var freeSquare = new BingoSquare("");
                    freeSquare.IsOpen.Value = true; // 最初から開けておく
                    newCard[i, j] = freeSquare;
                    continue;
                }

                if (nums.Count > 0)
                {
                    var winNumIndex = Random.Range(0, nums.Count);
                    var id = nums[winNumIndex];

                    // 新しいマスを作成（まだ誰もSubscribeしていないので通知は飛ばない）
                    newCard[i, j] = new BingoSquare(id);

                    nums.RemoveAt(winNumIndex);
                }
                else
                {
                    newCard[i, j] = new BingoSquare("");
                }
            }
        }

        // データの作成が完了したタイミングでセット
        // これで「新しいカードができたよ」という通知が1回だけ飛びます
        _bingoCard.Value = newCard;
    }

    // ゲーム中に番号が呼ばれた時の処理
    public void OpenNumber(string targetId)
    {
        var card = _bingoCard.Value;
        if (card == null) return;

        bool isFoundOnCard = false; // カードの中にそのIDが存在したか

        // 全マス走査して一致するIDを探す
        foreach (var square in card)
        {
            // まずIDが一致するかどうかをチェック
            if (square.ID == targetId)
            {
                isFoundOnCard = true;

                // まだ空いていない場合のみ処理する
                if (!square.IsOpen.Value)
                {
                    square.IsOpen.Value = true;
                    Debug.Log($"{targetId}が空きました");

                    // 正解（赤）にする
                    BuildingManager.Instance.ChangeColorBuilding(targetId, Color.red);
                }
                else
                {
                    Debug.Log($"{targetId}は既に空いています");
                }

                // IDはユニーク（1つしかない）はずなので、見つかったらループを抜け
            }
        }

        // --- ループ終了後の判定 ---

        if (isFoundOnCard)
        {
            // カードにあった場合：ビンゴ判定を行う
            if (IsBingo())
            {
                GameManager.Instance.FinishGame();
                Debug.Log("Clear");
            }
        }
        else
        {
            // カードになかった場合（お手つき）：ペナルティ処理
            // ここで初めて「不正解の色（黒）」にする
            BuildingManager.Instance.ChangeColorBuilding(targetId, Color.black);
            TimerManager.Instance.PenaltyTime();
        }
    }

    public bool IsBingo()
    {
        var card = _bingoCard.Value;
        int size = bingoCardSquareNum;

        // 1. 横(行)のチェック
        for (int y = 0; y < size; y++)
        {
            bool isRowBingo = true;
            for (int x = 0; x < size; x++)
            {
                if (!card[y, x].IsOpen.Value)
                {
                    isRowBingo = false;
                    break;
                }
            }
            if (isRowBingo) return true;
        }

        // 2. 縦(列)のチェック
        for (int x = 0; x < size; x++)
        {
            bool isColBingo = true;
            for (int y = 0; y < size; y++)
            {
                if (!card[y, x].IsOpen.Value)
                {
                    isColBingo = false;
                    break;
                }
            }
            if (isColBingo) return true;
        }

        // 3. 斜め（左上 -> 右下）のチェック
        bool isDiagonal1Bingo = true;
        for (int i = 0; i < size; i++)
        {
            if (!card[i, i].IsOpen.Value)
            {
                isDiagonal1Bingo = false;
                break;
            }
        }
        if (isDiagonal1Bingo) return true;

        // 4. 斜め（右上 -> 左下）のチェック
        bool isDiagonal2Bingo = true;
        for (int i = 0; i < size; i++)
        {
            // x座標は (サイズ-1) から引いていく
            if (!card[i, (size - 1) - i].IsOpen.Value)
            {
                isDiagonal2Bingo = false;
                break;
            }
        }
        if (isDiagonal2Bingo) return true;

        return false;
    }

    public void ChangeBingoTextColor(string id,Color color)
    {
        bingoCardView.AccessBingoCardTextColor(id, color);
    }
}

// 1マスごとのデータを管理するクラス
public class BingoSquare
{
    // 数字ID（途中で変わらないので普通の変数）
    public string ID { get; private set; }

    // 穴が開いているかどうか（ここをReactivePropertyにする）
    // 初期値は false (開いていない)
    public ReactiveProperty<bool> IsOpen { get; } = new ReactiveProperty<bool>(false);

    public BingoSquare(string id)
    {
        this.ID = id;
    }
}