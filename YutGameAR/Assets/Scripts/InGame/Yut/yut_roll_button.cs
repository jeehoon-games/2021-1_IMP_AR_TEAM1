using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yut_roll_button : MonoBehaviour
{
    Button button;
    GameObject yut;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Onclick);
    }
    void Onclick()
    {
        GameObject.FindWithTag("Player").GetComponent<new_yut_roll>().roll();
    }
}
