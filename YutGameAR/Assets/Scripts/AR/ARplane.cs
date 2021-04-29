using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARplane : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private GameObject yutboard;

    ManoGestureTrigger release;
    GameObject HandUI;
    [SerializeField]
    private GameObject yutboard_Prefab;
    [SerializeField]
    private GameObject yutplate_Prefab;
    


    

    static List<ARRaycastHit> s_hits = new List<ARRaycastHit>();

    private void Awake()
    {
        release = ManoGestureTrigger.RELEASE_GESTURE;
        HandUI = GameObject.Find("BoardUI");
        HandUI.SetActive(false);
        raycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    private void Update()
    {
        if(!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return ;
        }
        if (raycastManager.Raycast(touchPosition, s_hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_hits[0].pose;
            if (yutboard == null)
            {
                
                yutboard_Prefab.SetActive(true);
                yutplate_Prefab.SetActive(true);
                yutboard_Prefab.transform.position = hitPose.position + new Vector3(0, 0.2f, 0);
                yutplate_Prefab.transform.position = hitPose.position + new Vector3(-0.5f, 0.2f, 0);
                //yutboard = Instantiate(yutboard_Prefab, hitPose.position, Quaternion.identity);
                HandUI.SetActive(true);
            }
        }

        if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger == release)
            {
                Application.Quit();
                HandUI.SetActive(false);
                //yutboard.GetComponentInChildren<YutManager>().ThrowYut();
                Debug.Log("roll");
            }
     }
}
