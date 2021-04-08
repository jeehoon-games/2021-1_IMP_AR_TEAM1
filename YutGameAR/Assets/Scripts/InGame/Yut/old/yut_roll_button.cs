using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yut_roll_button : MonoBehaviour
{
    Button button;
    GameObject yut;
    bool first_roll =true;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Onclick);
    }
    void Onclick()
    {
        Debug.Log("hi");
        GameObject.FindGameObjectWithTag("Yut").GetComponent<yutset>().ch_result = 0;
        GameObject.FindGameObjectWithTag("Yut").GetComponent<yutset>().combination = " ";
        if (first_roll)
        {
            GameObject.FindGameObjectWithTag("Yut").GetComponent<yutset>().roll_all_yut();
            first_roll = false;
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Yut").GetComponent<yutset>().check_ground())
            {
                GameObject.FindGameObjectWithTag("Yut").GetComponent<yutset>().roll_all_yut();
            }
        }
    }
}
