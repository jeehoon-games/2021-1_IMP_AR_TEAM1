using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core
{
    public class Data
    {
        public struct UserData
        {
            public string uid;
            public string userID;
            public string userNickName;
            public string userName;
            public string userBirthday;
        }

        public struct GameData
        {
            public string uid;
            public int userNumOfWins;
            public int userNumOfDefeats;
            public int userRankPoint;
        }
    }    
}
