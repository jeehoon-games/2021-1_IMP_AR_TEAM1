using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutTree : MonoBehaviour
{ 
    private List<GameObject> _footSet = new List<GameObject>();
    private Dictionary<string, TreeNode> _nodeName = new Dictionary<string, TreeNode>();
    private TreeNode _rootNode;

    
    public List<GameObject> FootSet
    {
        get { return _footSet; }
    }

    public TreeNode RootNode
    {
        get { return _rootNode; }
    }

    public Dictionary<string, TreeNode> NodeName
    {
        get { return _nodeName; }
    }


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 30; i++)
        {
            _footSet.Add(GameObject.Find("FootHold_" + i));
        }
        
        CreateTreeNode(_footSet);
        MakeTree();
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void CreateTreeNode(List<GameObject> footset)
    {
        for (int i = 0; i < footset.Count; i++)
        {
            _nodeName.Add(String.Format("FootHold_{0}", i.ToString()), new TreeNode(footset[i]));
        }
        /*
        for (int i = 0; i < footset.Count; i++)
        {
            Debug.Log(nodeName["treeNode" + i].FootHold.name);
        }
        */
    }

    void MakeTree()
    {
        //부모가 하나인 애들은 LeftParent로 통일
        //부모가 두명인 애들은 그림대로
        //모든 node에서 빠른곳으로 가는 길은 무조건 rightChild
        //다음 교차로끼리의 길이 느린길이면 leftchild

        //0~19 LeftParent and LeftChild
        
        for (int i = 0; i < 19; i++)
        {
            int j = i + 1;
            ConnectLPAndLC(i, j);
        }
        _nodeName["FootHold_1"].LeftParent = _nodeName["FootHold_29"];

        ConnectLPAndLC(19, 29);
        ConnectLPAndRC(10, 23);
        ConnectLPAndRC(23, 24);
        ConnectLPAndRC(24, 20);
        ConnectLPAndRC(20, 27);
        ConnectLPAndRC(27, 28);
        ConnectLPAndRC(5, 21);
        ConnectLPAndRC(21, 22);
        
        ConnectLPAndLC(20, 25);
        ConnectLPAndLC(25, 26);

        ConnectRPAndRC(28, 29);
        ConnectRPAndRC(22, 20);

        ConnectRPAndLC(26, 15);

        for (int i = 1; i < 5; i++)
        {
            int j = i * 5;
            _nodeName["FootHold_" + j].IsIntersection = true;
        }
        _nodeName["FootHold_5"].IsTwoway = true;
        _nodeName["FootHold_10"].IsTwoway = true;
        _nodeName["FootHold_20"].IsTwoway = true;


        _rootNode = _nodeName["FootHold_" + 0];
    }

    //교차로가 아니면서 빠른길들
    void ConnectLPAndRC(int up, int down)
    {
        _nodeName["FootHold_" + up].RightChild = _nodeName["FootHold_" + down];
        _nodeName["FootHold_" + down].LeftParent = _nodeName["FootHold_" + up];
    }

    void ConnectLPAndLC(int up, int down)
    {
        _nodeName["FootHold_" + up].LeftChild = _nodeName["FootHold_" + down];
        _nodeName["FootHold_" + down].LeftParent = _nodeName["FootHold_" + up];
    }

    void ConnectRPAndLC(int up, int down)
    {
        _nodeName["FootHold_" + up].LeftChild = _nodeName["FootHold_" + down];
        _nodeName["FootHold_" + down].RightParent = _nodeName["FootHold_" + up];
    }
    void ConnectRPAndRC(int up, int down)
    {
        _nodeName["FootHold_" + up].RightChild = _nodeName["FootHold_" + down];
        _nodeName["FootHold_" + down].RightParent = _nodeName["FootHold_" + up];
    }

    public class TreeNode
    {
        private GameObject _footHold;
        private TreeNode _rightParent;
        private TreeNode _leftParent;
        private TreeNode _rightChild;
        private TreeNode _leftChild;
        private bool _isIntersection;
        private bool _isTwoway;
        private bool _enable;

        public TreeNode(GameObject foothold)
        {
            _footHold = foothold;
            _rightParent = null;
            _leftParent = null;
            _rightChild = null;
            _leftChild = null;
            _isIntersection = false;
            _isTwoway = false;
            _enable = false;
        }

        public GameObject FootHold
        {
            get { return _footHold; }
            set { _footHold = value; }
        }

        public TreeNode RightParent
        {
            get { return _rightParent; }
            set { _rightParent = value; }
        }

        public TreeNode LeftParent
        {
            get { return _leftParent; }
            set { _leftParent = value; }
        }

        public TreeNode RightChild
        {
            get { return _rightChild; }
            set { _rightChild = value; }
        }

        public TreeNode LeftChild
        {
            get { return _leftChild; }
            set { _leftChild = value; }
        }

        public bool IsIntersection
        {
            get { return _isIntersection; }
            set { _isIntersection = value; }
        }

        public bool IsTwoway
        {
            get { return _isTwoway; }
            set { _isTwoway = value; }
        }

        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; }
        }


    }
}
