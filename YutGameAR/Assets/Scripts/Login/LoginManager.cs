using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public class UserData
    {
        public string id = "abcd";
        public string pw = "abab";
    }
    private FMSocketIOManager _instance;

    private void Init()
    {
        _instance = FMSocketIOManager.instance;
    }

    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _instance.IsWebSocketConnected())
        {
            _instance.Emit("Event_Click", JsonUtility.ToJson(new UserData()));
        }
    }
}
