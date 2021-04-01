using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GameObject Arrow;

    private int _piecePos = 0;
    private List<int> _countNum = new List<int>();

    private void init(int piecePos, List<int> countNum)
    {
        _piecePos = piecePos;
        _countNum = countNum;
    }

    // calculating the position of the foothold that the piece can go.
    public void enableFootHold(int piecePos, List<int> countNum)
    {
        init(piecePos, countNum);
        
        
        List<int> enable = new List<int>();
        if (countNum.Count == 1)
        {

            enable.Add(piecePos + countNum[0]);

        }
        markingFootHold(enable);
        GameObject.Find("Button").GetComponent<YutThrow>().selectNumber.Clear();

    }











    // marking the foothold that can go.
    public void markingFootHold(List<int> num)
    {
        List<GameObject> Set = GameObject.Find("Main Camera").GetComponent<GameManager>().FootSet;
        Vector3 Pos;
        for (int i = 0; i < num.Count; i++)
        {
            Pos = Set[num[i]].transform.position;
            Instantiate(Arrow, new Vector3(Pos.x, Pos.y + 5, Pos.z), Quaternion.identity);
        }
    }
}
