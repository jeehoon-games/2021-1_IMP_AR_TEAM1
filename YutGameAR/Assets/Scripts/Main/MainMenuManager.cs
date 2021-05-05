using System.Collections;
using System.Collections.Generic;
using Core;
using MainMenu;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Main
{
    public class MainMenuManager : MonoBehaviour
    {
        #region public & private instance variables / Init method

        // Create Room, Find Room UI Group
        public GameObject findPlaneGroup;
        public GameObject createRoomGroup;
        public GameObject findRoomGroup;

        // Main menu UI 
        private TextMeshProUGUI _notification;
        private GameObject _createRoomMenuSelection;
        private GameObject _findRoomMenuSelection;
        private int _menuType;
        private int _notificationTimer;
        
        private void Init()
        {
            _notification = transform.Find("NotificationCanvas").Find("Notification").GetComponent<TextMeshProUGUI>();
            _createRoomMenuSelection = transform.Find("CreateRoomMenu").Find("SelectionVisualization").gameObject;
            _findRoomMenuSelection = transform.Find("FindRoomMenu").Find("SelectionVisualization").gameObject;
            _menuType = 0;
            StartCoroutine(NotificationTimer());
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
                MenuSelector(t0);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                findPlaneGroup.SetActive(true);
                findPlaneGroup.GetComponent<ARPlaneSelector>().Reset();
                StopCoroutine(NotificationTimer());
                gameObject.SetActive(false);
            }
        }

        private void MenuSelector(Touch t0)
        {
            if (t0.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(t0.position);
                RaycastHit hit;
                _notificationTimer = 0;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("CreateRoomMenu"))
                    {
                        if (_menuType == 0)
                        {
                            _notification.text = "You chose Create Room Menu! Please touch anywhere on the screen.";
                            _menuType = 1;
                        }
                        else
                        {
                            _menuType = 0;
                        }
                    }

                    if (hit.collider.gameObject.CompareTag("FindRoomMenu"))
                    {
                        if (_menuType == 0)
                        {
                            _notification.text = "You chose Find Room Menu! Please touch anywhere on the screen.";
                            _menuType = 2;
                        }
                        else
                        {
                            _menuType = 0;
                        }
                    }
                }
                else
                {
                    switch (_menuType)
                    {
                        case 0:
                            _notification.text = "Please choose the game menu.";
                            break;
                        case 1:
                            createRoomGroup.SetActive(true);
                            gameObject.SetActive(false);
                            break;

                        case 2:
                            findRoomGroup.SetActive(true);
                            gameObject.SetActive(false);
                            break;
                    }
                }
            }
        }

        public void Reset()
        {
            Init();
            _menuType = 0;
            _notificationTimer = 0;
            _notification.text = "";
            StartCoroutine(NotificationTimer());
        }

        #endregion

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
    }
}