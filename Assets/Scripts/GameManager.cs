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
                Debug.Log("GameOver!");
            })
            .AddTo(this);
    }
    public void FinishGame()
    {
        _isGameFinished.Value = true;
        TimerManager.Instance.StopTimer();
    }

    public void ResetGame()
    {
        _isGameFinished.Value = false;
    }

    public void StartGame()
    {
        BingoManager.Instance.SetBingoCard(BuildingManager.Instance.GetAllBuildingIDArray());
    }
}