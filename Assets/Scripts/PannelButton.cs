using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PannelButton : CustomButton
{
    [SerializeField] private CanvasGroup _canvasGroup;
    public override void OnPointerClick(PointerEventData eventData)
    {
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0f;
        base.OnPointerClick(eventData);

    }

}
