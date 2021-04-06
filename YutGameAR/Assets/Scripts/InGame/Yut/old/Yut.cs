using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yut : MonoBehaviour
{
    Rigidbody rb;
    bool hasLanded;
    bool Throw;

    Vector3 initPosition;

    public string YutValue;
    public string Name;
    public YutSide[] yutSides;
    public bool sideground;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rollyut();
        }
        if (rb.IsSleeping()&&!hasLanded && Throw)
        {
            hasLanded = true;
            Valuecheck();
        }
        else if (rb.IsSleeping() && !hasLanded && YutValue == " ")
        {
            rollagain();
        }
    }
    void rollyut()
    {
        if (!Throw && !hasLanded)
        {
            Throw=true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100));
        }
        else if (Throw && hasLanded)
        {
            Reset();
        }
    }
    private void Reset()
    {
        transform.position = initPosition;
        Throw = false;
        hasLanded = false;
        rb.useGravity=false;
        YutValue = " ";
    }
    private void rollagain()
    {
        Reset();
        Throw = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100));
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
