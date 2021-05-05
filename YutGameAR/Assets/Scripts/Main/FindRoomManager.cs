using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class FindRoomManager : MonoBehaviour
    {
        public GameObject mainMenuGroup;
        public GameObject lobbyGroup;
        public GameObject roomPanel;
        public RoomPanelManager selectedPanelMgr;
        public struct Room
        {
            public string sid;
            public string roomLeader;
            public int currUserCnt;
            public int maxUserCnt;
        }
        private struct JoinInfo
        {
            public string roomName;
            public string userName;
        }
        
        private Dictionary<string, Room> _currRoomTable;
        private GameObject _scrollViewContent;
        private TextMeshProUGUI _notification;
        private Button _refreshListBtn;
        private Button _joinRoomBtn;
        private int _notificationTimer;

        void Init()
        {
            Transform cvsTransform = transform.Find("FindRoomCanvas");
            _scrollViewContent = cvsTransform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
            _notification = cvsTransform.Find("notification").GetComponent<TextMeshProUGUI>();
            _refreshListBtn = cvsTransform.Find("RefreshListBtn").GetComponent<Button>();
            _joinRoomBtn = cvsTransform.Find("JoinRoomBtn").GetComponent<Button>();
            
            _refreshListBtn.onClick.AddListener(delegate { OnRefreshListBtnClick(); });
            _joinRoomBtn.onClick.AddListener(delegate { OnJoinRoomBtnClick(); });
            StartCoroutine(NotificationTimer());
            StartCoroutine(RegisterSocketIOEvent());
        }

        void Awake()
        {
            Init();
        }

        void Start()
        {
            FMSocketIOManager.instance.Emit("Event_RefreshRoomList");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mainMenuGroup.SetActive(true);
                mainMenuGroup.GetComponent<MainMenuManager>().Reset();
                StopCoroutine(NotificationTimer());
                gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            Init();
            _notification.text = "";
            _notificationTimer = 0;
            StartCoroutine(NotificationTimer());
        }
        
        void RefreshRoomList()
        {
            for (int i = 0; i < _scrollViewContent.transform.childCount; i++)
                Destroy(_scrollViewContent.transform.GetChild(i).gameObject);
            
            if (_currRoomTable != null)
            {
                int j = 0;
                foreach (string rName in _currRoomTable.Keys)
                {
                    GameObject panel = Instantiate(roomPanel, new Vector3(0, -75 + (-155) * j, 0), Quaternion.identity);
                    RoomPanelManager pMgr = panel.GetComponent<RoomPanelManager>();
                    pMgr.SetRoomInfo(rName, _currRoomTable[rName]);
                    panel.transform.SetParent(_scrollViewContent.transform);
                    j++;
                }
            }
        }

        void OnRefreshListBtnClick()
        {
            if (FMSocketIOManager.instance != null)
            {
                if (FMSocketIOManager.instance.Ready) { FMSocketIOManager.instance.Emit("Event_RefreshRoomList"); }
                else { _notification.text = "Cannot connect to server"; }
            }
            else { _notification.text = "Socket IO Object is null"; }
        }

        void OnJoinRoomBtnClick()
        {
            _notificationTimer = 0;
            if (FMSocketIOManager.instance != null)
            {
                if (FMSocketIOManager.instance.Ready)
                {
                    if (!selectedPanelMgr.roomName.Equals(""))
                    {
                        JoinInfo joinInfo = new JoinInfo();
                        joinInfo.roomName = selectedPanelMgr.roomName;
                        joinInfo.userName = NetworkCore.Instance.UserData.userNickName;
                        FMSocketIOManager.instance.Emit("Event_JoinRoom", JsonUtility.ToJson(joinInfo));
                    }
                    else { _notification.text = "Please select the room."; }
                }
                else { _notification.text = "Cannot connect to server"; }
            }
            else { _notification.text = "Socket IO Object is null"; }
        }

        IEnumerator NotificationTimer()
        {
            while (true)
            {
                _notificationTimer += 1;
                if (_notificationTimer >= 90)
                {
                    _notification.text = "";
                    _notificationTimer = 0;
                }
                yield return null;
            }
        }

        IEnumerator RegisterSocketIOEvent()
        {
            while (FMSocketIOManager.instance == null)
                yield return null;
            
            while (!FMSocketIOManager.instance.Ready)
                yield return null;
            
            FMSocketIOManager.instance.On("Event_RefreshRoomList_Result", (e) =>
            {
                string data = e.data.Replace("\\", "");
                data = data.Substring(1, data.Length - 2);
                _currRoomTable = JsonConvert.DeserializeObject<Dictionary<string, Room>>(data);
                Debug.Log(data);
                RefreshRoomList();
            });
            
            FMSocketIOManager.instance.On("Event_JoinRoom_Result", (e) =>
            {
                _notificationTimer = 0;
                string data = e.data.Substring(1, e.data.Length - 2);
                switch (data)
                {
                    case "Success":
                        StopCoroutine(NotificationTimer());
                        gameObject.SetActive(false);
                        lobbyGroup.gameObject.SetActive(true);
                        lobbyGroup.gameObject.GetComponent<LobbyManager>().SetIsRoomLeader(false);
                        lobbyGroup.gameObject.GetComponent<LobbyManager>().leaderName = selectedPanelMgr.room.roomLeader;
                        break;
                    case "FullyOccupied":
                        _notification.text = "The room is full.";
                        break;
                    case "Fail":
                        _notification.text = "Fail to join game room.";
                        break;
                }
            });
        }
    }
}