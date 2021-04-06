using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutSide : MonoBehaviour
{
    private bool onGround;
    public string sideValue;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            onGround = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            onGround = false;
        }
    }
   public bool isGround()
    {
        return onGround;
    }
    
}
