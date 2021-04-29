using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Core
{
    public class NetworkCore : MonoBehaviour
    {

        public static NetworkCore Instance;
        
        public Data.UserData UserData;
        public Data.GameData GameData;

        private Thread _networkThread;
        private int _expireTime;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            FMSocketIOManager.instance.Connect();
            DontDestroyOnLoad(gameObject);
            _networkThread = new Thread(Run);
            _networkThread.Start();
            Screen.orientation = ScreenOrientation.Portrait;
        }

        private void Run()
        {
            while (true)
            {
                if (!FMSocketIOManager.instance.Ready) { _expireTime += 1; }
                else { _expireTime = 0; }
                
                if (_expireTime >= 10)
                {
                    FMSocketIOManager.instance.Connect();
                    _expireTime = 0;
                }
                Thread.Sleep(1000);
            }
        }

        public void DataDump(char opt)
        {
            StringBuilder strBuilder = new StringBuilder();
            switch (opt)
            {
                case 'u':
                    strBuilder.Append("UID: ");
                    strBuilder.Append(UserData.uid);
                    strBuilder.Append("\nuserID: ");
                    strBuilder.Append(UserData.userID);
                    strBuilder.Append("\nuserNickName: ");
                    strBuilder.Append(UserData.userNickName);
                    strBuilder.Append("\nuserName: ");
                    strBuilder.Append(UserData.userName);
                    strBuilder.Append("\nuserBirthday: ");
                    strBuilder.Append(UserData.userBirthday);
                    break;
                
                case 'g':
                    strBuilder.Append("UID: ");
                    strBuilder.Append(GameData.uid);
                    strBuilder.Append("\nuserNumOfWins: ");
                    strBuilder.Append(GameData.userNumOfWins);
                    strBuilder.Append("\nuserNumOfDefeats: ");
                    strBuilder.Append(GameData.userNumOfDefeats);
                    strBuilder.Append("\nuserRankPoint: ");
                    strBuilder.Append(GameData.userRankPoint);
                    break;
            }
            Debug.Log(strBuilder.ToString());
        }

        public void SaveGameData()
        {
            if (FMSocketIOManager.instance.Ready)
            {
                FMSocketIOManager.instance.Emit("Event_SaveGameData", JsonUtility.ToJson(GameData));
            }
        }
    }
}