using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneInfo : MonoBehaviour
{
    private static ARPlaneInfo instance;
    public static ARPlaneInfo Instance
    {
        get
        {
            return instance;
        }
    }

    public ARPlane plane;
    public Vector3 center;

    private void Awake()
    {
        instance = this;
    }
}
