using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GameObject Arrow;

    private GameObject _piece;
    private string _pieceName;
    private List<int> _countNum = new List<int>();
    private Dictionary<string,YutTree.TreeNode> _nodeName;

    private void init(GameObject piece, List<int> countNum)
    {
        _piece = piece;
        _pieceName = piece.GetComponent<Pieces>().PosName;
        _countNum = countNum;
        _nodeName = GameObject.Find("Main Camera").GetComponent<YutTree>().NodeName;
    }

    // calculating the position of the foothold that the piece can go.
    public void PathFind(GameObject piece, List<int> countNum)
    {
        
        init(piece, countNum);
        Path(_pieceName, _countNum[0]);
        
        
        
        
        //GameObject.Find("Button").GetComponent<YutThrow>().selectNumber.Clear();

    }

    private void Path(string Name, int count)
    {
        YutTree.TreeNode startNode, nextNode;
        
        //교차로가 아니면서 길이 하나인 node
        if (!_nodeName[Name].IsIntersection && !_nodeName[Name].IsTwoway)
        {
            if(_nodeName[Name].RightChild == null)
            {
                startNode = _nodeName[Name];
                for(int i = 0; i<count; i++)
                {
                    nextNode = startNode.LeftChild;
                    startNode = nextNode;
                }
                _piece.GetComponent<Pieces>().PosName = startNode.FootHold.name;
                
                markingFootHold(startNode.FootHold.transform.position);
            }
        }
        
    }

    private void markingFootHold(Vector3 postion)
    {
        Instantiate(Arrow, new Vector3(postion.x, postion.y + 5, postion.z), Quaternion.identity);
    }









    // marking the foothold that can go.
    /*
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
    */
}
