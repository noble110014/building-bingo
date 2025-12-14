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
        TimerManager.Instance.StartTimer();
        EventManager.Instance.StartEvent();
        GameManager.Instance.StartGame();
        
    }
}
