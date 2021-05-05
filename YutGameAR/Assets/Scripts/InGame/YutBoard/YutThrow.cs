using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class YutThrow : MonoBehaviour
{
    public GameObject yutPlate;
    private bool _throwing;
    private List<int> _selectNumber;
    private YutManager _yutMgr;
    public Text text;
    ManoGestureTrigger release;

    public bool Throwing 
    { 
        get { return _throwing; } 
        set { _throwing = value; }
    }
    public List<int> SelectNumber
    {
        get { return _selectNumber; }
        set { _selectNumber = value; }
    }
    
    void Start()
    {
        release = ManoGestureTrigger.RELEASE_GESTURE;
        _yutMgr = yutPlate.GetComponent<YutManager>();
        _selectNumber = new List<int>();
    }

    void Update()
    {
        if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger == release && !_throwing)
        {
            _throwing = false;
            _yutMgr.ThrowYut();
            StartCoroutine(MakeResult());
        }
    }

    IEnumerator MakeResult()
    {
        while (!_yutMgr.done)
        {
            yield return null; 
        }
        text.text = _yutMgr.yType + " 칸";
        _selectNumber.Add(_yutMgr.yType);
        if (_yutMgr.yType == 4 || _yutMgr.yType == 5)
        {
            _throwing = false;
        }
        else
        {
            _throwing = true;
        }
    }
}
