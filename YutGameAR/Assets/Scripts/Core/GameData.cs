using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public string uid;
    public int userNumOfWins;
    public int userNumOfDefeats;
    public int userRankPoint;

    public GameData()
    {
        uid = "";
        userNumOfWins = 0;
        userNumOfDefeats = 0;
        userRankPoint = 500;
    }
}
