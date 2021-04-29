using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutManager : MonoBehaviour
{
    public Queue<int> resultQueue;
    public int yType;   // -1: 빽도, 1: 도, 2: 개, 3: 걸, 4: 윷, 5: 모
    public bool done;
    ManoGestureTrigger release;
    
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
    }
    
    void Start()
    {
        Init();
    }
    
    

    public void ThrowYut()
    {
        resultQueue.Clear();
        yType = 0;
        done = false;

        for (int i = 0; i < _forceArr.Length; i++)
        {
            _forceArr[i].xTorque = Random.Range(50 ,1000);
            _forceArr[i].yTorque = Random.Range(800, 1000);
            _forceArr[i].zTorque = Random.Range(10,300);
            _forceArr[i].yForce = Random.Range(100,101);
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
        
        for (int i = 0; i < 4; i++)
        {
            yType += resultQueue.Dequeue();
        }

        if (yType == 0)
        {
            yType = 5;
        }

        if (yType == 1)
        {
            foreach (YutController yCont in _yutContArr)
            {
                if (yCont.yid == 1 && yCont.result == 1)
                {
                    yType = -1;
                    break;
                }
            }
        }
        
        done = true;
    }
}
