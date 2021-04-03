using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetworkCore : MonoBehaviour
{
    private FMSocketIOManager _ioInstance;
    private NetworkCore _netInstance;

    public UserData UserData;
    public GameData GameData;
    public NetworkCore Instance
    {
        get
        {
            if (_netInstance == null)
            {
                
            }

            return _netInstance;
        }
    }

    private void Init()
    {
        if (_ioInstance == null) { _ioInstance = FMSocketIOManager.instance; }
        _ioInstance.Connect();
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        Init();
    }

    public void SaveGameData()
    {
        
    }
    
    IEnumerator ReConnect()
    {
        yield return null;
    }
}
