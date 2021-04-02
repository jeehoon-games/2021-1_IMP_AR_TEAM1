using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetworkCore : MonoBehaviour
{
    private FMSocketIOManager _instance;

    private void Init()
    {
        if (_instance == null) { _instance = FMSocketIOManager.instance; }
        _instance.Connect();
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        Init();
    }
}
