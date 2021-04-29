using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    private string _posName = "FootHold_0";
    private int _point = 1;
    private Vector3 _initPosition;
    public string teamColor;
    public int ID;

    void Start()
    {
        _initPosition = transform.position;    
    }

    public Vector3 InitPosition
    {
        get { return _initPosition; }
    }

    public string PosName
    {
        get { return _posName; }
        set { _posName = value; }
    }

    public int Point
    {
        get { return _point; }
        set { _point = value; }
    }

  
}
