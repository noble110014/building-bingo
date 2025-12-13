using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private BoolReactiveProperty _isGameFinished = new BoolReactiveProperty(false);
    public IReadOnlyReactiveProperty<bool> IsGameFinished => _isGameFinished;

    public void Initialize()
    {
        ResetGame();
        TimerManager.Instance.CountdownTimePrep
            .Where(value => value <= 0)
            .Subscribe(value =>
            {
                FinishGame();
            })
            .AddTo(this);
    }
    public void FinishGame()
    {
        _isGameFinished.Value = true;
    }

    public void ResetGame()
    {
        _isGameFinished.Value = false;
    }
}