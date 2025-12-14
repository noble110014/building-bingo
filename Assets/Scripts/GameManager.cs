using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private BoolReactiveProperty _isGameFinished = new BoolReactiveProperty(false);
    public IReadOnlyReactiveProperty<bool> IsGameFinished => _isGameFinished;

    [SerializeField] private CanvasGroup _clearMenu;
    [SerializeField] private CanvasGroup _gameOverMenu;

    public void Initialize()
    {
        ResetGame();
        TimerManager.Instance.CountdownTimePrep
            .Where(value => value <= 0)
            .Subscribe(value =>
            {
                FinishGame();
                Debug.Log("GameOver!");
            })
            .AddTo(this);
    }
    public async Task FinishGame()
    {
        _isGameFinished.Value = true;
        TimerManager.Instance.StopTimer();
        EventManager.Instance.StopEvent();

        if (BingoManager.Instance.IsBingo())
        {
            await StageFinishMovie(_clearMenu);
        }
        else
        {
            await StageFinishMovie(_gameOverMenu);
        }

    }

    public void ResetGame()
    {
        _isGameFinished.Value = false;
    }

    public void StartGame()
    {
        BingoManager.Instance.SetBingoCard(BuildingManager.Instance.GetAllBuildingIDArray());
    }

    private async UniTask StageFinishMovie(CanvasGroup canvas)
    {
        await DOTween.Sequence()
            .Append(canvas.DOFade(1f, 2f).SetEase(Ease.OutCubic))
            .AppendCallback(() =>
            {
                canvas.interactable = true;
            })
            .AsyncWaitForCompletion();
    }
}