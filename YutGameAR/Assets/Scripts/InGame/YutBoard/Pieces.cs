using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pieces : MonoBehaviour
{
    private string _posName = "FootHold_0";
    private int _point = 1;
    private Vector3 _initPosition;
    private TextMeshPro _text;
    public string teamColor;

    public int ID;

    void Start()
    {
        
        _initPosition = transform.position;
        _text = GetComponentInChildren<TextMeshPro>();
    }

    public void SetRidingText(string ridingText)
    {
        _text.text = ridingText;
    }

    public void HideText(bool hide)
    {
        _text.enabled = hide;
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
