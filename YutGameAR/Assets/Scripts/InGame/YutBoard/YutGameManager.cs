using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutGameManager : MonoBehaviour
{
    private bool _select = false;
    private GameObject _selectedPiece;
    private Dictionary<string, YutTree.TreeNode> _enableNode;

    private int BlueScore = 0;
    private YutThrow YutComponent;
    private YutTree TreeComponent;
    private GameObject[] PiecesSet;

    private struct UserInfo
    {
        public string userColor;
        public bool userTurn;
    }

    public bool Select { get { return _select; } }


    // Start is called before the first frame update
    void Start()
    {
        YutComponent = GetComponent<YutThrow>();
        TreeComponent = GetComponent<YutTree>();
        PiecesSet = GameObject.FindGameObjectsWithTag("Piece");
        
    }

    // Update is called once per frame
    void Update()
    {
        selectAndMove();
    }

    void selectAndMove()
    {
        // 윷을 던진 상태에서 터치를 하는 경우
        if (Input.GetMouseButtonDown(0) && YutComponent.Throwing)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            //DestroyArrow();
            
            if (hit.collider.gameObject != null)
            {
                                
                // 터치하는 것이 말일 경우
                if (hit.collider.gameObject.CompareTag("Piece") && !_select)
                {
                    //hit.collider.GetComponent<Renderer>().material.color = Color.red;
                    _select = true;
                    _selectedPiece = hit.collider.gameObject;
                    _enableNode = hit.collider.GetComponent<PathFinding>().PathFind(_selectedPiece, YutComponent.SelectNumber);
                }

                // 말을 터치한 후 갈 발판을 터치한 경우
                else if (hit.collider.gameObject.CompareTag("FootHold") && _select && _enableNode.ContainsKey(hit.collider.name))
                {
                    
                    // when the pieces didn't start
                    if (_enableNode[hit.collider.name] == null)
                    {
                        Debug.Log("kjh    111111111  a"+ _enableNode[hit.collider.name]);
                        if (_selectedPiece.GetComponent<Pieces>().PosName == "FootHold_0") { StartCoroutine(MoveTo(_selectedPiece, TreeComponent.NodeName["FootHold_29"].FootHold.transform.position, hit.collider.transform.position)); }
                        else { StartCoroutine(MoveTo(_selectedPiece, hit.collider.transform.position)); }
                    }
                    // else
                    else { StartCoroutine(MoveTo(_selectedPiece, _enableNode[hit.collider.name].FootHold.transform.position, hit.collider.transform.position)); }

                    //The pieces go to the same FootHold.
                    //There are two cases when you are on the same team and when you are on a different team.
                    for (int i = 0; i < PiecesSet.Length; i++)
                    {
                        
                        if (hit.collider.name.Equals(PiecesSet[i].GetComponent<Pieces>().PosName))
                        {
                            Pieces ps = PiecesSet[i].GetComponent<Pieces>();
                            if (_selectedPiece.GetComponent<Pieces>().teamColor.Equals(ps.teamColor))
                            {
                                ps.Point += 1;
                                Debug.Log(ps.Point + "포인트");
                                _selectedPiece.GetComponent<Pieces>().Point = 0;
                                _selectedPiece.GetComponent<Pieces>().PosName = "FootHold_0";
                                PiecesSet[i].SetActive(false);
                            }
                            else
                            {
                                if(ps.Point > 1)
                                {
                                    for (int j = 0; j< PiecesSet.Length; j++)
                                    {
                                        if (!PiecesSet[j].activeInHierarchy)
                                        {
                                            PiecesSet[j].SetActive(true);
                                            Pieces ps2 = PiecesSet[j].GetComponent<Pieces>();
                                            if (ps2.teamColor.Equals(ps.teamColor) && ps2.Point == 0)
                                            {
                                                //ps2.PosName = "FootHold_0";
                                                PiecesSet[j].transform.position = ps2.InitPosition;
                                                ps2.Point = 1;
                                                Debug.Log(ps2.PosName);
                                                Debug.Log(ps2.Point);
                                                //PiecesSet[j].SetActive(true);
                                            }
                                        }
                                        
                                    }
                                }
                                
                                PiecesSet[i].transform.position = ps.InitPosition;
                                ps.PosName = "FootHold_0";
                                ps.Point = 1;
                            }
                        }
                    }
                        
                    
                    
                    _selectedPiece.GetComponent<Pieces>().PosName = hit.collider.name;
                    //_selectedPiece.GetComponent<Renderer>().material.color = new Color(102 / 255f, 123 / 255f, 255 / 255f, 255 / 255f);
                    _select = false;
                    DestroyArrow();
                    

                    // 여러번 움직일 때
                    if(YutComponent.SelectNumber.Count > 1)
                    {
                        YutComponent.SelectNumber.Remove(TreeComponent.NodeName[hit.collider.name].Step);
                    }
                    else
                    {
                        YutComponent.Throwing = false;
                        YutComponent.SelectNumber.Clear();
                    }
                }

                // 그 외의 경우 초기로 돌려줌
                else
                {
                    DestroyArrow();
                    _select = false;
                    //_selectedPiece.GetComponent<Renderer>().material.color = new Color(102 / 255f, 123 / 255f, 255 / 255f, 255 / 255f);
                }
            }
            
        }
        /*
        if(Input.GetMouseButtonDown(0))
        {
            if (_select)
            {
                DestroyArrow();
                _select = false;
                _selectedPiece.GetComponent<Renderer>().material.color = new Color(102 / 255f, 123 / 255f, 255 / 255f, 255 / 255f);
            }
            
        }
        */
    }

    // Destroy the arrows when the piece is out of focus.
    void DestroyArrow()
    {
        GameObject[] arrow = GameObject.FindGameObjectsWithTag("Arrow");
        for(int i = 0; i< arrow.Length; i++)
            Destroy(arrow[i]);
    }

    void GoalInSetting(GameObject piece)
    {
        Destroy(piece);
        BlueScore += 1;
        Debug.Log("BlueScore : " + BlueScore);
    }




    IEnumerator MoveTo(GameObject piece, Vector3 throughPos, Vector3 toPos)
    {

        float count = 0, count2 = 0;
        Vector3 wasPos = piece.transform.position;
        throughPos.y += 0.02f;
        toPos.y += 0.02f;
        while (true)
        {
            count += Time.deltaTime;
            piece.transform.LookAt(throughPos);
            piece.transform.Rotate(0, 180, 0);
            piece.transform.position = Vector3.Lerp(wasPos, throughPos, count);
            if (piece.transform.position == throughPos)
            {
                count2 += Time.deltaTime;
                piece.transform.position = Vector3.Lerp(throughPos, toPos, count2);
                if (count2 >= 1)
                {
                    piece.transform.LookAt(toPos);
                    piece.transform.Rotate(0, 180, 0);
                    piece.transform.position = toPos;
                    
                    // the piece reached the finish line 
                    if (toPos.x == TreeComponent.NodeName["FootHold_30"].FootHold.transform.position.x)
                    {
                        GoalInSetting(piece);

                    }

                    break;
                }
            }
            yield return null;
        }
    }

    IEnumerator MoveTo(GameObject piece, Vector3 toPos)
    {

        float count = 0;
        Vector3 wasPos = piece.transform.position;
        toPos.y += 0.02f;
        while (true)
        {
            count += Time.deltaTime;
            piece.transform.LookAt(toPos);
            piece.transform.Rotate(0, 180, 0);
            piece.transform.position = Vector3.Lerp(wasPos, toPos, count);
            if (count >= 1)
            {
                piece.transform.position = toPos;

                if (toPos.x == TreeComponent.NodeName["FootHold_30"].FootHold.transform.position.x)
                {
                    GoalInSetting(piece);

                }

                break;
            }
            yield return null;
        }
    }

    FMSocketIOManager
}
