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

        // Main menu group game object
        public GameObject mainMenuGroup;

        // Canvas UI objects
        private Image _planeSizeBar;
        private RectTransform _psbRectTransform;    // plane size bar's rect transform component
        private TextMeshProUGUI _planeAnnotationBottom;
        private Color _redColor, _orangeColor, _greenColor;
        
        // Indicator
        private GameObject _indicator;
        private Renderer _indicatorRenderer;
        
        // AR plane, raycast objects
        private const float MIN_PLANE_SIZE = 0.4f;  // critical value of ARPlane size (ARPlane.size.x * ARPlane.size.y)
        private ARPlane _currPlane;
        private ARPlaneManager _arPlaneManager;
        private ARRaycastManager _arRaycastManager;
        private List<ARRaycastHit> _hitList;
        
        private bool _findProperPlane;
        private bool _setProperPlane;
        
        
        void Init()
        {
            // Init canvas
            Transform canvasTransform = transform.Find("FindPlaneCanvas");
            _planeSizeBar = canvasTransform.Find("PlaneSizeBar").GetComponent<Image>();
            _psbRectTransform = _planeSizeBar.GetComponent<RectTransform>();
            _planeAnnotationBottom = canvasTransform.Find("PlaneAnnotationBottom").GetComponent<TextMeshProUGUI>(); 
            _redColor = new Color(1, 0, 0, 1); 
            _orangeColor = new Color(1, 0.5f, 0, 1); 
            _greenColor = new Color(0, 1, 0, 1);
            
            // Init indicator
            _indicator = transform.Find("PlacementIndicator").GetChild(0).gameObject;
            _indicatorRenderer = _indicator.GetComponent<Renderer>();
            _indicatorRenderer.material.color = new Color(0.25f, 0.25f, 1, 1);
            _indicatorRenderer.gameObject.SetActive(false);
            
            // Init AR plane, AR raycast
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
            _hitList = new List<ARRaycastHit>();
            
            Screen.orientation = ScreenOrientation.Landscape;
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
            SetProperPlane();
        }
        
        #endregion


        #region Utility methods
        
        

        private void FindProperPlane()
        {
            if (!_setProperPlane)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                RaycastHit hit;
                
                if (_arRaycastManager.Raycast(ray, _hitList, TrackableType.PlaneWithinPolygon))
                {
                    if (!_indicatorRenderer.gameObject.activeInHierarchy)
                    {
                        _indicatorRenderer.gameObject.SetActive(true);
                    }
                    _indicator.transform.position = _hitList[0].pose.position;
                    _indicator.transform.rotation = _hitList[0].pose.rotation;
                }
                
                if (Physics.Raycast(ray, out hit))
                {
                    _findProperPlane = false;
                    if (hit.collider.gameObject.CompareTag("ARPlane"))
                    {
                        _currPlane = hit.collider.gameObject.GetComponent<ARPlane>();
                        float size = _currPlane.size.x * _currPlane.size.y;
                        Color matColor = Color.white;

                        if (size < MIN_PLANE_SIZE)
                        {
                            matColor = _redColor;
                            _planeAnnotationBottom.text = "Please find a wide surface.";
                        }
                        else if (_currPlane.normal != Vector3.up)
                        {
                            matColor = _orangeColor;
                            _planeAnnotationBottom.text = "Please find a flat surface.";
                        }
                        else
                        {
                            matColor = _greenColor;
                            _planeAnnotationBottom.text = "It's a good plane! Please touch and select.";
                            _findProperPlane = true;
                        }
                        _planeSizeBar.material.color = matColor;
                        _indicatorRenderer.material.color = matColor;
                        _psbRectTransform.localScale = new Vector3(Mathf.Clamp(size, 0, 1), 1.0f, 1.0f);
                    }
                }
                else
                {
                    _indicatorRenderer.material.color = _redColor;
                    _psbRectTransform.localScale = new Vector3(0, 1, 1);
                    _planeAnnotationBottom.text = "Please find a flat, wide surface.";
                }
            }
        }

        private void SetProperPlane()
        {
            if (Input.touchCount > 0)
            {
                Touch t0 = Input.GetTouch(0);
                if (t0.phase == TouchPhase.Began && _findProperPlane && !_setProperPlane)
                {
                    _setProperPlane = true;
                    gameObject.SetActive(false);
                    mainMenuGroup.SetActive(true);

                    Vector3 planeCenter = _currPlane.center;
                    mainMenuGroup.transform.Find("CreateRoomMenu").position = planeCenter + new Vector3(-0.25f, 0.0f, 0);
                    mainMenuGroup.transform.Find("FindRoomMenu").position = planeCenter + new Vector3(0.25f, 0.0f, 0);
                    mainMenuGroup.transform.Find("YutGameTitle").position = planeCenter + new Vector3(0, 0.4f, 0);
                }
            }

            if (_setProperPlane)
            {
                foreach (ARPlane plane in _arPlaneManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
                
                _arPlaneManager.enabled = false;
            }
        }

        public Vector3 GetPlaneCenter()
        {
            return _currPlane.center;
        }
        public void Reset()
        {
            Init();
            _findProperPlane = false;
            _setProperPlane = false;
            _arPlaneManager.enabled = true;
        }
        
        #endregion
    }
}
