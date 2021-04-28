using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutManager : MonoBehaviour
{
    public Queue<int> resultQueue;
    public int rType;   // 0: 모, 1: 도, 2: 개, 3: 걸, 4: 윷, 5: 빽도
    
    private struct YutForce
    {
        public int xTorque;
        public int yTorque;
        public int zTorque;
        public int yForce;
    };
    private YutForce[] _forceArr;
    private YutController[] _yutContArr;


    private void Init()
    {
        resultQueue = new Queue<int>();
        _forceArr = new YutForce[4];
        _yutContArr = GetComponentsInChildren<YutController>();
        rType = -1;
    }
    
    void Start()
    {
        Init();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ThrowYut();
        }
        
        Debug.Log(rType);
    }

    public void ThrowYut()
    {
        resultQueue.Clear();
        rType = -1;
        
        for (int i = 0; i < _forceArr.Length; i++)
        {
            _forceArr[i].xTorque = Random.Range(200, 500);
            _forceArr[i].yTorque = Random.Range(50, 100);
            _forceArr[i].zTorque = Random.Range(50, 100);
            _forceArr[i].yForce = Random.Range(50, 100);
            _yutContArr[i].Throw(_forceArr[i].xTorque, _forceArr[i].yTorque, _forceArr[i].zTorque, _forceArr[i].yForce, ForceMode.Force);
        }
        StartCoroutine(MakeResult());
    }

    IEnumerator MakeResult()
    {
        while (resultQueue.Count < 4)
        {
            yield return null;
        }

        rType = 0;
        for (int i = 0; i < 4; i++)
        {
            rType += resultQueue.Dequeue();
        }

        if (rType == 1)
        {
            foreach (YutController yCont in _yutContArr)
            {
                if (yCont.yid == 1 && yCont.result == 1)
                {
                    rType = 5;
                    break;
                }
            }
        }
    }
}
