using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    private string _posName = "FootHold_0";
    public string teamColor;

    public string PosName
    {
        get { return _posName; }
        set { _posName = value; }
    }

  
}
