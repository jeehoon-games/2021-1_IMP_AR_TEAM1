using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace MainMenu
{
    public class ARPlaneSelector : MonoBehaviour
    {
        
        #region public & private instance variable & Init Method
        
        public Canvas planeDetectCvs;
        public Canvas menuCvs;
        public GameObject createRoomMenu;
        public GameObject findRoomMenu;
        public GameObject GameTitle;
        
        private const float MIN_PLANE_SIZE = 0.6f;
        private ARPlaneManager _arPlaneManager;
        private ARPlane _currPlane;
        private ARRaycastManager _arRaycastManager;
        private List<ARRaycastHit> _hitList;
        private GameObject _indicator;

        private Color _redBarColor = new Color(1, 0, 0, 1);
        private Color _orangeBarColor = new Color(1, 0.5f, 0, 1);
        private Color _greenBarColor = new Color(0, 1, 0, 1);
        private TextMeshProUGUI _planeAnnotationBottom;
        private Image _loadingBar;
        private RectTransform _lBarRectTransform;

        private bool _findPlane;
        private bool _onFindProperPlane;
        
        
        void Init()
        {
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            _hitList = new List<ARRaycastHit>();
            _indicator = transform.GetChild(0).gameObject;
            _indicator.SetActive(false);
            _indicator.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 1, 1);
            _planeAnnotationBottom = FindUiInCvs<TextMeshProUGUI>(planeDetectCvs, "PlaneAnnotationBottom");
            _loadingBar = FindUiInCvs<Image>(planeDetectCvs, "LoadingBar");
            _lBarRectTransform = _loadingBar.GetComponent<RectTransform>();
        }
        
        #endregion

        
        
        #region Unity Event Functions
        void Start()
        {
            Init();
        }

        void Update()
        {
            FindProperPlane();
            SaveProperPlane();
        }
        
        #endregion


        #region Utility methods

        private void FindProperPlane()
        {
            if (!_findPlane)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                RaycastHit hit;
                
                if (_arRaycastManager.Raycast(ray, _hitList, TrackableType.Planes))
                {
                    if (!_indicator.activeInHierarchy)
                    {
                        _indicator.SetActive(true);
                    }
                    transform.position = _hitList[0].pose.position;
                    transform.rotation = _hitList[0].pose.rotation;
                }
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("ARPlane"))
                    {
                        _currPlane = hit.collider.gameObject.GetComponent<ARPlane>();
                        float size = _currPlane.size.x * _currPlane.size.y;
                        Color matColor = Color.white;

                        if (size < MIN_PLANE_SIZE)
                        {
                            matColor = _redBarColor;
                            _planeAnnotationBottom.text = "Please find a wide surface.";
                            _onFindProperPlane = false;
                        }
                        else if (_currPlane.normal != Vector3.up)
                        {
                            matColor = _orangeBarColor;
                            _planeAnnotationBottom.text = "Please find a flat surface.";
                            _onFindProperPlane = false;
                        }
                        else
                        {
                            matColor = _greenBarColor;
                            _planeAnnotationBottom.text = "It's a good plane! Please touch and select.";
                            _onFindProperPlane = true;
                        }
                        
                        _lBarRectTransform.localScale = new Vector3(size / (0.5f + size),1.0f, 1.0f);
                        _loadingBar.material.color = matColor;
                        _indicator.GetComponent<MeshRenderer>().material.color = matColor;
                    }
                }
                else
                {
                    _lBarRectTransform.localScale = new Vector3(0, 1, 1);
                    _indicator.GetComponent<MeshRenderer>().material.color = _redBarColor;
                    _planeAnnotationBottom.text = "Please find a flat, wide surface.";
                    _onFindProperPlane = false;
                }
            }
        }

        private void SaveProperPlane()
        {
            if (Input.touchCount > 0)
            {
                Touch t0 = Input.GetTouch(0);
                if (t0.phase == TouchPhase.Began && _currPlane != null && !_findPlane && _onFindProperPlane)
                {
                    _findPlane = true;
                    _indicator.SetActive(false);
                    planeDetectCvs.gameObject.SetActive(false);
                    
                    // set active menu obj
                    if (!createRoomMenu.activeInHierarchy && !findRoomMenu.activeInHierarchy)
                    {
                        createRoomMenu.SetActive(true);
                        findRoomMenu.SetActive(true);
                        GameTitle.SetActive(true);
                        Vector3 planeCenter = _currPlane.center;
                        createRoomMenu.transform.position = planeCenter + new Vector3(-0.25f, 0.05f, 0);
                        findRoomMenu.transform.position = planeCenter + new Vector3(0.25f, 0.05f, 0);
                        GameTitle.transform.position = planeCenter + new Vector3(0, 0.2f, 0);
                    }
                }
            }

            if (_findPlane)
            {
                foreach (ARPlane plane in _arPlaneManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
            }
        }
        
        private T FindUiInCvs<T>(Canvas cvs, string uiName) where T : Component
        {
            T[] uiArr = cvs.GetComponentsInChildren<T>();
            foreach (T ui in uiArr)
            {
                if (ui.name.Equals(uiName))
                {
                    return ui;
                }
            }
            return null;
        }
        
        #endregion
    }
}
