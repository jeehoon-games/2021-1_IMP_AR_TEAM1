using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YutText : MonoBehaviour
{
    Text text;
    string result;
    // Start is called before the first frame update

    private void Start()
    {
        text = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        result = GameObject.FindGameObjectWithTag("Yut").GetComponent<yutset>().combination;
        text.text = result;
    }
}
