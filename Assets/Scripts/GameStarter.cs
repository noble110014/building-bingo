using Cysharp.Threading.Tasks;
using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class GameStarter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ThirdPersonController player;

    [SerializeField] private TextMeshProUGUI startText;
    async void Start()
    {
        Initialize();
        await StartEffect();
        startText.text = "";
        FirstStep();
    }

    private async UniTask StartEffect()
    {
        startText.text = "Ready?";
        await DOTween.Sequence()
            .AppendInterval(1f)
            .Append(startText.transform.DOScale(1f, 1f).SetEase(Ease.OutCubic))
            .Append(startText.transform.DOScale(10f, 0.5f).SetEase(Ease.OutCubic))
            .AppendCallback(() =>
            {
                startText.text = "GO!!!!";
                startText.transform.localScale = Vector3.zero;
            })
            .Append(startText.transform.DOScale(1f, 1f).SetEase(Ease.OutBounce))
            .Append(startText.transform.DOScale(10f, 0.5f).SetEase(Ease.OutCubic))
            .AsyncWaitForCompletion();
    }
    private void Initialize()
    {
        BuildingManager.Instance.Initialize();
        TimerManager.Instance.Initialize();
        GameManager.Instance.Initialize();
        BingoManager.Instance.Initialize();
        player.Initialize();
    }

    private void FirstStep()
    {
        TimerManager.Instance.StartTimer();
        EventManager.Instance.StartEvent();
        GameManager.Instance.StartGame();
    }
}
