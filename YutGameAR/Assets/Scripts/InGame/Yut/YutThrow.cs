using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YutThrow : MonoBehaviour
{
    private Button button;
    private bool _throwing = false;
    private int total = 10000;
    private int[] weight = { 384, 1152, 3456, 3456, 1296, 256 };
    private List<int> _selectNumber = new List<int>();


    public bool throwing 
    { 
        get { return _throwing; } 
        set { _throwing = value; }
    }

    public List<int> selectNumber
    {
        get { return _selectNumber; }
        set { _selectNumber = value; }
    }

    // get RandomNumber based on weight
    // weights are the probability of »ªµµ,µµ,°³,°É,À·,¸ð.
    public int RandomNumber()
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
    }
    
    void Start()
    {
        button = GetComponent<Button>();
       
        button.onClick.AddListener(OnClickButton);
    }


    void Update()
    {
    
    }

    void OnClickButton()
    {
        
        _selectNumber.Add(RandomNumber());
        Debug.Log(_selectNumber[0]);
        _throwing = true;
    }
}
