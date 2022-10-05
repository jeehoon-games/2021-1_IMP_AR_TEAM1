using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class CreateRoomManager : MonoBehaviour
    {
        public GameObject mainMenuGroup;
        public GameObject lobbyGroup;
        
        private TMP_InputField _roomNameInputField;
        private TMP_Dropdown _gameModeSelector;
        private TextMeshProUGUI _notification;
        private Button _createRoomBtn;

        private int _notificationTimer;
        
        private struct GameRoom
        {
            public string roomName;
            public string roomLeader;
            public int roomType;

            public GameRoom(string roomName, string roomLeader, int roomType)
            {
                this.roomName = roomName;
                this.roomLeader = roomLeader;
                this.roomType = roomType;
            }
        }

        void Init()
        {
            Transform cvsTransform = transform.Find("CreateRoomCanvas");
            _roomNameInputField = cvsTransform.Find("RoomNameField").GetComponent<TMP_InputField>();
            _gameModeSelector = cvsTransform.Find("GameModeDropdown").GetComponent<TMP_Dropdown>();
            _notification = cvsTransform.Find("CreateRoomNotify").GetComponent<TextMeshProUGUI>();
            _createRoomBtn = cvsTransform.Find("CreateRoomBtn").GetComponent<Button>();
            _createRoomBtn.onClick.AddListener(delegate { OnCreateRoomBtnClick(); });
            StartCoroutine(NotificationTimer());
            StartCoroutine(RegisterSocketIOEvent());
        }
        
        void Start()
        {
            Init();
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
        
        void OnCreateRoomBtnClick()
        {
            if (_gameModeSelector.value > 0)
            {
                // gameModeSelector.value 0: Not Selected, 1: 1 vs 1, 2: 2 vs 2
                GameRoom room = new GameRoom(_roomNameInputField.text, NetworkCore.Instance.UserData.userNickName,
                    _gameModeSelector.value);
                FMSocketIOManager.instance.Emit("Event_CreateRoom", JsonUtility.ToJson(room));
            }
            else
            {
                _notificationTimer = 0;
                _notification.text = "Please select the game mode."; 
            }
            _notificationTimer = 0;
        }

        IEnumerator NotificationTimer()
        {
            while (true)
            {
                _notificationTimer += 1;
                if (_notificationTimer >= 90)
                {
                    _notification.text = "";
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
            
            FMSocketIOManager.instance.On("Event_CreateRoom_Result", (e) =>
            {
                string data = e.data.Substring(1, e.data.Length - 2);

                switch (data)
                {
                    case "Success":
                        StopCoroutine(NotificationTimer());
                        gameObject.SetActive(false);
                        lobbyGroup.gameObject.SetActive(true);
                        lobbyGroup.gameObject.GetComponent<LobbyManager>().SetIsRoomLeader(true);
                        lobbyGroup.gameObject.GetComponent<LobbyManager>().roomName = _roomNameInputField.text;
                        lobbyGroup.transform.Find("TeamRed").position = ARPlaneInfo.Instance.center + new Vector3(-0.25f, 0.0f, 0);
                        lobbyGroup.transform.Find("TeamBlue").position = ARPlaneInfo.Instance.center + new Vector3(0.25f, 0.0f, 0);
                        break;
                    case "Duplicate":
                        _notificationTimer = 0;
                        _notification.text = "Room name already exist.";
                        break;
                    case "Fail":
                        _notificationTimer = 0;
                        _notification.text = "Fail to create game room";
                        break;
                }
            });
        }
    }
}
