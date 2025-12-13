using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    // Start is called before the first frame update
    [ContextMenu("DebugStartGame")]
    void Start()
    {
        BuildingManager.Instance.Initialize();
        TimerManager.Instance.Initialize();
        GameManager.Instance.Initialize();
        BingoManager.Instance.Initialize();
        GameManager.Instance.StartGame();
        TimerManager.Instance.StartTimer();
    }
}
