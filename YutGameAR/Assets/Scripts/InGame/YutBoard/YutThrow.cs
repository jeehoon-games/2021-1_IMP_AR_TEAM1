using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class YutThrow : MonoBehaviour
{
    public GameObject yutPlate;
    //private Button button;
    private bool _throwing;
    //private int total = 10000;
    //private int[] weight = { 384, 1152, 3456, 3456, 1296, 256 };
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

    // get RandomNumber based on weight
    // weights are the probability of 빽도,도,개,걸,윷,모.
    /*&public int RandomNumber()
    {
        int currentWeight = 0;
        int selectNum = 0;

        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));
        for (int i = 0; i < 6; i++)
        {
            currentWeight += weight[i];
            if(selectNum <= currentWeight)
            {
                if(i == 0)
                {
                    return -1;
                }
                return i;
            }
        }
        return 0;
    }*/
    
    void Start()
    {
        release = ManoGestureTrigger.RELEASE_GESTURE;
        _yutMgr = yutPlate.GetComponent<YutManager>();
        _selectNumber = new List<int>();
    }

    void Update()
    {
        /*
        if(Input.GetMouseButtonDown(0) && !_throwing)
        {
            _throwing = false;
            _yutMgr.ThrowYut();
            StartCoroutine(MakeResult());
        }
        */
        
        if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger == release && !_throwing)
        {
            _throwing = false;
            _yutMgr.ThrowYut();
            StartCoroutine(MakeResult());
        }
        /*
        if (Input.touchCount > 0 && !_throwing)
        {   
            Touch t0 = Input.GetTouch(0);
            if (t0.phase == TouchPhase.Began)
            {
                _throwing = false;
                _yutMgr.ThrowYut();
                StartCoroutine(MakeResult());
            }
        } 
        */
    }

    /*void OnClickButton()
    {
        
        button.GetComponentInChildren<Text>().text = "Throw!";
        int step = RandomNumber();
        _selectNumber.Add(step);
        
        Debug.Log("kjh           "+_selectNumber[0]);
        if(step == 4 || step == 5)
        {
            Debug.Log("onemore!");
            _throwing = false;
            button.GetComponentInChildren<Text>().text = "One More!";
        }
        else
        {
            _throwing = true;
        }
    }*/

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
            /*
            _yutMgr.ThrowYut();
            StartCoroutine(MakeResult());
            */
            _throwing = false;
        }
        else
        {
            _throwing = true;
        }
    }
}
