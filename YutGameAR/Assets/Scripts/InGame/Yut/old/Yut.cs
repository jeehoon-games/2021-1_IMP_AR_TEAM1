using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yut : MonoBehaviour
{
    Rigidbody rb;
    bool hasLanded;
    bool Throw;

    public 

    Vector3 initPosition;

    public string YutValue;
    public string Name;
    public YutSide[] yutSides;
    public bool sideground;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -4, 0);
        initPosition = transform.position;
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.IsSleeping()&&!hasLanded && Throw)
        {
            Debug.Log(Physics.gravity);
            hasLanded = true;
            Valuecheck();
        }
    }

    public void rollyut()
    {
        Reset();
        if (!Throw && !hasLanded)
        {
            Throw =true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(300, 600), Random.Range(300, 600), Random.Range(300, 600));
            rb.AddForce(10,10,10);
            Debug.Log("hi");

        }
    }
    private void Reset()
    {
        transform.position = initPosition;
        Throw = false;
        hasLanded = false;
        rb.useGravity=false;
        sideground = false;
        YutValue = " ";
    }
    

    void Valuecheck()
    {
        YutValue = " ";
        foreach (YutSide side in yutSides)
        {
            if (side.isGround())
            {
                YutValue = side.sideValue;
                sideground = true;
                Debug.Log(Name+" "+YutValue+"is rolled");
            }
        }
       
    }
}