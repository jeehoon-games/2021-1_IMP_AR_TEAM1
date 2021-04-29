using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEditor;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    #region public & private instance variables / Init method
    
    public GameObject createRoomMenu;
    public GameObject findRoomMenu;
    public GameObject yutBoard;
    public GameObject yutPlate;
    public Canvas MenuCanvas;
    
    private struct Room
    {
        public string roomName;
        public string roomLeader;
    }

    private struct User
    {
        public string uID;
        public string roomName;
    }

    private void Init()
    {
        StartCoroutine(RegisterSocketIOEvent());
    }
    
    #endregion
    
    
    #region Unity Event Functions
    
    void Start()
    {
        Init();
    }
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t0 = Input.GetTouch(0);
            if (t0.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(t0.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("CreateRoomMenu"))
                    {
                        Room room = new Room();
                        room.roomLeader = NetworkCore.Instance.UserData.userNickName;
                        room.roomName = "MyRoom";
                        string roomToJson = JsonUtility.ToJson(room);
                        FMSocketIOManager.instance.Emit("Event_CreateRoom", roomToJson);
                    }

                    if (hit.collider.gameObject.CompareTag("FindRoomMenu"))
                    {
                        User user = new User();
                        user.roomName = "MyRoom";
                        user.uID = NetworkCore.Instance.UserData.uid;
                        string userToJson = JsonUtility.ToJson(user);
                        FMSocketIOManager.instance.Emit("Event_JoinRoom", userToJson);
                    }
                }
            }
        }
    }
    
    #endregion
    
    
    #region SocketIO Event Listener

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
                case "Duplicate":
                    Debug.Log("Room already exist.");
                    break;
                case "Success":
                    Debug.Log("Success to create room");
                    // wating for another user
                    createRoomMenu.SetActive(false);
                    findRoomMenu.SetActive(false);
                    MenuCanvas.gameObject.SetActive(true);
                    break;
            }
        });
        
        FMSocketIOManager.instance.On("Event_JoinRoom_Result", (e) =>
        {
            string data = e.data.Substring(1, e.data.Length - 2);
            switch (data)
            {
                case "CannotFind":
                    Debug.Log("Fail to join room: cannot find room");
                    break;
                case "NoFreeSpace":
                    Debug.Log("Fail to join room: no free space");
                    break;
                case "Success":
                    Debug.Log("Success to join room");
                    // make yut board
                    Vector3 center = createRoomMenu.transform.position;
                    createRoomMenu.SetActive(false);
                    findRoomMenu.SetActive(false);
                    MenuCanvas.gameObject.SetActive(true);
                    
                    yutBoard.SetActive(true);
                    yutPlate.SetActive(true);
                    yutBoard.transform.position = center + new Vector3(0, 0.2f, 0);
                    yutPlate.transform.position = center + new Vector3(-0.5f, 0.2f, 0);
                    break;
            }
        });
        
        FMSocketIOManager.instance.On("Event_ClientJoinRoom", (e) =>
        {
            string data = e.data.Substring(1, e.data.Length - 2);
            Vector3 center = createRoomMenu.transform.position;
            MenuCanvas.gameObject.SetActive(false);
            yutBoard.SetActive(true);
            yutPlate.SetActive(true);
            yutBoard.transform.position = center + new Vector3(0, 0.2f, 0);
            yutPlate.transform.position = center + new Vector3(-0.5f, 0.2f, 0);
        });
        
        FMSocketIOManager.instance.On("Event_RefreshRoom_Result", (e) =>
        {
            
        });
        
        FMSocketIOManager.instance.On("Event_LeaveRoom_Result", (e) =>
        {
            
        });
    }

    #endregion
}
