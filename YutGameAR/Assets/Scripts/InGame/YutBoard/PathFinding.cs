using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinding : MonoBehaviour
{
    public GameObject Arrow;

    private GameObject _piece;
    private string _pieceName;
    private List<int> _countNum = new List<int>();
    private List<int> _distictNum = new List<int>();
    private Dictionary<string,YutTree.TreeNode> _nodeName;
    private Dictionary<string, YutTree.TreeNode> _enableNode = new Dictionary<string, YutTree.TreeNode>();

    private void Init(GameObject piece, List<int> countNum)
    {
        _piece = piece;
        _pieceName = piece.GetComponent<Pieces>().PosName;
        _countNum = countNum;
        _nodeName = GameObject.Find("YutGameManager").GetComponent<YutTree>().NodeName;
        _enableNode.Clear();
    }


    // calculating the position of the foothold that the piece can go.
    public Dictionary<string, YutTree.TreeNode> PathFind(GameObject piece, List<int> countNum)
    {
        Init(piece, countNum);
        _distictNum = countNum.Distinct().ToList();
        for (int i = 0; i < _distictNum.Count; i++)
        {
            if(_distictNum[i] == -1)
            {
                BackPath(_pieceName, _distictNum[i]);
            }
            else
            {
                Path(_pieceName, _distictNum[i]);
            }
            
        }
        return _enableNode;
    }

    private void Path(string Name, int count)
    {
        YutTree.TreeNode startNode, nextNode;
        YutTree.TreeNode throughNode = null;
        bool exchange = false;

        startNode = _nodeName[Name];
        
        //Not Intersection And route is one
        if (!_nodeName[Name].IsIntersection)
        {

            //Not Intersection
            if (_nodeName[Name].RightChild == null)
            {
                //startNode = _nodeName[Name];
                for (int i = 0; i < count; i++)
                {
                    if (CheckTheEnd(startNode)) { break; }
                    nextNode = startNode.LeftChild;
                    if (nextNode.IsIntersection) { throughNode = nextNode; }
                    startNode = nextNode;
                }
                if (startNode.FootHold.name.Equals("FootHold_5"))
                {
                    throughNode = null;
                }
            }
            //21,22,23,24,27,28 FootHold
            else
            {
                
                //startNode = _nodeName[Name];
                for (int i = 0; i < count; i++)
                {
                    if (CheckTheEnd(startNode)) { break; }
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

        //Intersection And route is two (5,10,20)
        else if (_nodeName[Name].IsIntersection && _nodeName[Name].IsTwoway)
        {
            for (int i = 0; i < count; i++)
            {
                if (CheckTheEnd(startNode)) { break; }
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

        //Intersection And route is one (15번)
        else if (_nodeName[Name].IsIntersection && !_nodeName[Name].IsTwoway)
        {
            for (int i = 0; i < count; i++)
            {
                if (CheckTheEnd(startNode)) { break; }
                nextNode = startNode.LeftChild;
                startNode = nextNode;
            }
            
        }
        MarkingFootHold(startNode.FootHold.transform.position);
        _enableNode.Add(startNode.FootHold.name, throughNode);
        startNode.Step = count;

    }

    // the path of the piece when it is BackDo(빽도)
    private void BackPath(string Name, int count)
    {
        YutTree.TreeNode startNode, nextNode;


        startNode = _nodeName[Name];

        if(Name.Equals("FootHold_20") || Name.Equals("FootHold_29"))
        {
            //나중에 추가할 예정 빽도
        }
        else
        {
            nextNode = startNode.LeftParent;
            startNode = nextNode;
        }
            
        
        MarkingFootHold(startNode.FootHold.transform.position);
        _enableNode.Add(startNode.FootHold.name, null);
        startNode.Step = count;
    }

    private bool CheckTheEnd(YutTree.TreeNode theEnd)
    {
        if (theEnd.FootHold.name.Equals("FootHold_30"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Marking the available FootHold
    private void MarkingFootHold(Vector3 postion)
    {
        Instantiate(Arrow, new Vector3(postion.x, postion.y, postion.z), Quaternion.identity);
    }

}
