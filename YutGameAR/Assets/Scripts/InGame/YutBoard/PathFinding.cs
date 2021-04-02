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
    private Dictionary<string, YutTree.TreeNode> _enableNode;

    private void init(GameObject piece, List<int> countNum)
    {
        _piece = piece;
        _pieceName = piece.GetComponent<Pieces>().PosName;
        _countNum = countNum;
        _nodeName = GameObject.Find("Main Camera").GetComponent<YutTree>().NodeName;
        _enableNode = new Dictionary<string, YutTree.TreeNode>();
    }

    // calculating the position of the foothold that the piece can go.
    public Dictionary<string, YutTree.TreeNode> PathFind(GameObject piece, List<int> countNum)
    {
        
        init(piece, countNum);
        for(int i = 0; i < countNum.Count; i++)
        {
            Path(_pieceName, _countNum[i]);
        }
        //GameObject.Find("Button").GetComponent<YutThrow>().selectNumber.Clear();
        return _enableNode;
    }

    private void Path(string Name, int count)
    {
        YutTree.TreeNode startNode, nextNode;
        YutTree.TreeNode throughNode = null;
        bool exchange = false;

        startNode = _nodeName[Name];

        //교차로가 아니면서 길이 하나인 node
        if (!_nodeName[Name].IsIntersection /*&& !_nodeName[Name].IsTwoway*/)
        {
            //교차로가 아닌 node들
            if (_nodeName[Name].RightChild == null)
            {
                //startNode = _nodeName[Name];
                for (int i = 0; i < count; i++)
                {
                    nextNode = startNode.LeftChild;
                    if (nextNode.IsIntersection) { throughNode = nextNode; }
                    
                    startNode = nextNode;
                }       
            }
            //21,22,23,24,27,28
            else
            {
                //startNode = _nodeName[Name];
                for (int i = 0; i < count; i++)
                {
                    if (!exchange)
                    {
                        nextNode = startNode.RightChild;
                        if (nextNode.IsIntersection && nextNode.RightParent == startNode)
                        {
                            exchange = true;
                            throughNode = nextNode;
                        }
                        startNode = nextNode;
                    }
                    else
                    {
                        nextNode = startNode.LeftChild;
                        if (nextNode.IsIntersection) { throughNode = nextNode; }
                        startNode = nextNode;
                    }
                }   
            }
            
        }

        //교차로이면서 갈수 있는 길이 2개인 node (5,10,20)
        else if (_nodeName[Name].IsIntersection && _nodeName[Name].IsTwoway)
        {
            for (int i = 0; i < count; i++)
            {
                if (!exchange)
                {
                    nextNode = startNode.RightChild;
                    if (nextNode.IsIntersection && nextNode.RightParent == startNode) { exchange = true; }
                    startNode = nextNode;
                }
                else
                {
                    nextNode = startNode.LeftChild;
                    startNode = nextNode;
                }
            }
            
        }
        
        //교차로이면서 길이 하나인 node (15번)
        else if (_nodeName[Name].IsIntersection && !_nodeName[Name].IsTwoway)
        {
            for (int i = 0; i < count; i++)
            {
                nextNode = startNode.LeftChild;
                startNode = nextNode;
            }
            
        }
        
        //마지막 node
        else if (_nodeName[Name].FootHold.name == "FootHold_29")
        {
            if(count != -1)
            {
                //끝
                
            }
        }

        //_piece.GetComponent<Pieces>().PosName = startNode.FootHold.name;
        markingFootHold(startNode.FootHold.transform.position);
        _enableNode.Add(startNode.FootHold.name, throughNode);
        startNode.Enable = true;

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
