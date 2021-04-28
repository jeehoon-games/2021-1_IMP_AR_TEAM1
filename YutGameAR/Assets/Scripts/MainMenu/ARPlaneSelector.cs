using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace MainMenu
{
    public class ARPlaneSelector : MonoBehaviour
    {
        public Text info;
        public GameObject yutPlate;
        
        private ARPlaneManager _arPlaneManager;
        private ARRaycastManager _arRaycastManager;
        private List<ARRaycastHit> _hitList;
        private ARPlane _currPlane;
        
        void Start()
        {
            _arPlaneManager = GetComponent<ARPlaneManager>();
            _arRaycastManager = GetComponent<ARRaycastManager>();
            _hitList = new List<ARRaycastHit>();
        }
        
        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch t0 = Input.GetTouch(0);
                Ray ray = Camera.main.ScreenPointToRay(t0.position);
                RaycastHit hit;
                if (t0.phase == TouchPhase.Began && Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("ARPlane"))
                    {
                        _currPlane = hit.collider.gameObject.GetComponent<ARPlane>();
                        float size = _currPlane.size.x * _currPlane.size.y;

                        if (size > 0.25f && _currPlane.normal == Vector3.up)
                        {
                            Instantiate(yutPlate, _currPlane.center, Quaternion.identity);
                            _currPlane.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 16);
                        }
                        else
                        {
                            _currPlane.GetComponent<Renderer>().material.color = new Color(255, 0, 0, 16);
                        }
                    }
                }
                //_arRaycastManager.Raycast(ray, _hitList, TrackableType.PlaneWithinPolygon);
            }
        }
    }
}
