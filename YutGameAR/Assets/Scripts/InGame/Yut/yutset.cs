using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yutset : MonoBehaviour
{
    public Yut[] yut;
    bool grounded=false;
    private int frontcount=0;
    private int sbackcount = 0;
    private int backcount=0;
    void Update()
    {
            allground();
        
    }
    void checkCombination()
    {
        if (grounded == true)
        {
            foreach (Yut instantyut in yut)
            {
                if (instantyut.YutValue == "front")
                {
                    frontcount++;
                }
                if (instantyut.YutValue == "back")
                {
                    backcount++;
                }
                if (instantyut.YutValue == "sback")
                {
                    sbackcount++;
                }
            }
            if (sbackcount == 1 && frontcount == 3)
            {
                Debug.Log("»ªµµ");
            }
            else if (backcount == 1&&frontcount==3)
            {
                Debug.Log("µµ");
            }
            else if (frontcount == 2)
            {
                Debug.Log("°³");
            }
            else if (frontcount==1)
            {
                Debug.Log("°É");
            }
            else if (frontcount == 4)
            {
                Debug.Log("¸ð");
            }
            else if (backcount == 3 && sbackcount == 1)
            {
                Debug.Log("À·");
            }
            frontcount = 0;
            backcount = 0;
            sbackcount = 0;
            grounded = false;
        }
    }
    bool allground()
    {
        foreach (Yut instantyut in yut)
        {
            if (!instantyut.sideground)
            {
                return false;
            }
        }
        grounded = true;
        checkCombination();
        return true;
    }
   
}
