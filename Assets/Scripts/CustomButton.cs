using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] protected UnityEvent clickedEvent = new UnityEvent(); // クリックイベント
    

    protected virtual void Start()
    {
        
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // マウスオーバー時のスケール変更
        
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // マウス離脱時に元のスケールに戻す
        
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        
        clickedEvent?.Invoke();
   
    }

    


}
