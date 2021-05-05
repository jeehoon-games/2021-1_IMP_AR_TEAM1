using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class RoomPanelManager : MonoBehaviour
    {
        public string roomName;
        public FindRoomManager.Room room;
        
        private TextMeshProUGUI _roomNameTMP;
        private TextMeshProUGUI _currPlayerCntTMP;
        private Button _playButton;


        void Init()
        {
            _roomNameTMP = transform.Find("RoomNameTMP").GetComponent<TextMeshProUGUI>();
            _currPlayerCntTMP = transform.Find("CurrPlayerCntTMP").GetComponent<TextMeshProUGUI>();
            _playButton = transform.Find("PlayButton").GetComponent<Button>();
            _playButton.onClick.AddListener(delegate { OnPlayBtnClick(); });
        }

        void Awake()
        {
            Init();
        }

        void OnPlayBtnClick()
        {
            FindObjectOfType<FindRoomManager>().selectedPanelMgr = this;
        }

        public void SetRoomInfo(string roomName, FindRoomManager.Room room)
        {
            this.roomName = roomName;
            this.room = room;
            _roomNameTMP.text = room.roomLeader + "'s Room: " + roomName;
            _currPlayerCntTMP.text = room.currUserCnt + " / " + room.maxUserCnt;
        }
    }   
}
