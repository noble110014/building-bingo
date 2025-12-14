using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements; // DOTweenの名前空間を追加

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float hoverScale = 1.1f; // ホバー時の倍率
    [SerializeField] private float animDuration = 0.1f; // アニメーション時間
    [SerializeField] private Ease animEase = Ease.OutQuad; // イージング設定

    [Space(10)]
    [SerializeField] protected UnityEvent clickedEvent = new UnityEvent();

    // 内部変数
    private Vector3 _initialScale;
    private Tween _scaleTween; // 現在実行中のTweenを保持する変数

    protected virtual void Start()
    {
        // 開始時のスケールを保存（ここを基準に戻すため）
        _initialScale = transform.localScale;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // マウスオーバー時のスケール変更
        PlayScaleTween(_initialScale * hoverScale,animEase);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // マウス離脱時に元のスケールに戻す
        PlayScaleTween(_initialScale, animEase);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // クリック時の動き（オプション：少し押し込む演出などを入れると気持ちいいです）
        // ここでは単純にイベント発火のみ行います
        clickedEvent?.Invoke();
        PlayScaleTween(_initialScale,Ease.InBounce);

        // もしクリック時にもアニメーションさせたい場合は以下のようなPunchScaleが有効です
        // transform.DOPunchScale(Vector3.one * -0.1f, 0.1f).SetLink(gameObject);
    }

    /// <summary>
    /// スケールアニメーションの共通処理（キャンセル処理付き）
    /// </summary>
    private void PlayScaleTween(Vector3 targetScale,Ease ease)
    {
        // 【キャンセル処理 1】
        // すでに再生中のTweenがあれば強制停止（Kill）する。
        // これをしないと、連続でマウスを出し入れした際に挙動がおかしくなります。
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }

        // 新しいTweenを作成して保存
        _scaleTween = transform.DOScale(targetScale, animDuration)
            .SetEase(ease)
            .SetLink(gameObject); // 【キャンセル処理 2】GameObjectが消えたらTweenも自動破棄
    }

    /// <summary>
    /// オブジェクト破棄時のキャンセル処理
    /// </summary>
    protected virtual void OnDestroy()
    {
        // 【キャンセル処理 3】
        // オブジェクトが破棄される際、Tweenが残っているとエラーの原因になるため確実にKillする
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }
    }
}