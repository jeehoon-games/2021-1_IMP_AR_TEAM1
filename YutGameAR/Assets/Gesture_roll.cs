using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture_roll : MonoBehaviour
{
    GameObject yutboard;
    // Start is called before the first frame update
    void Start()
    {
        yutboard = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger == ManoGestureTrigger.RELEASE_GESTURE)
        {
            yutboard.GetComponentInChildren<YutManager>().ThrowYut();
            Debug.Log("roll");
        }
    }
}
