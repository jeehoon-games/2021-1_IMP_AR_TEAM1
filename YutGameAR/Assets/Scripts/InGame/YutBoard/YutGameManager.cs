using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutGameManager : MonoBehaviour
{
    private bool _select = false;
    private Vector3 _movePosition;
    private GameObject _selectedPiece;
    private Dictionary<string, YutTree.TreeNode> _enableNode;

    public bool select { get { return _select; } }
    public Vector3 movePosition { get { return _movePosition; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        selectAndMove();
    }

    void selectAndMove()
    {
        // 윷을 던진 상태에서 터치를 하는 경우
        if (Input.GetMouseButtonDown(0) && GameObject.Find("Button").GetComponent<YutThrow>().throwing)
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

                    hit.collider.GetComponent<Renderer>().material.color = Color.red;
                    _select = true;
                    _selectedPiece = hit.collider.gameObject;

                    _enableNode = hit.collider.GetComponent<PathFinding>().PathFind(_selectedPiece, GameObject.Find("Button").GetComponent<YutThrow>().SelectNumber);

                }

                // 말을 터치한 후 갈 발판을 터치한 경우
                else if (hit.collider.gameObject.CompareTag("FootHold") && _select && _enableNode.ContainsKey(hit.collider.name))
                {
                    if(_enableNode[hit.collider.name] == null)
                    {

                        if(_selectedPiece.GetComponent<Pieces>().PosName == "FootHold_0") { StartCoroutine(MoveTo(_selectedPiece,new Vector3(20,0,-20), hit.collider.transform.position)); }
                        else { StartCoroutine(MoveTo(_selectedPiece, hit.collider.transform.position)); }
                        
                    }
                    else { StartCoroutine(MoveTo(_selectedPiece, _enableNode[hit.collider.name].FootHold.transform.position, hit.collider.transform.position)); }
                    
                    //_selectedPiece.GetComponent<Pieces>().posNumber = index;
                    _selectedPiece.GetComponent<Pieces>().PosName = hit.collider.name;
                    _selectedPiece.GetComponent<Renderer>().material.color = new Color(102 / 255f, 123 / 255f, 255 / 255f, 255 / 255f);
                    _select = false;
                    DestroyArrow();
                    GameObject.Find("Main Camera").GetComponent<YutTree>().NodeName[hit.collider.name].Enable = false;
                    GameObject.Find("Button").GetComponent<YutThrow>().throwing = false;
                    GameObject.Find("Button").GetComponent<YutThrow>().SelectNumber.Clear();


                }

                // 그 외의 경우 초기로 돌려줌
                else
                {
                    DestroyArrow();
                    _select = false;
                    _selectedPiece.GetComponent<Renderer>().material.color = new Color(102 / 255f, 123 / 255f, 255 / 255f, 255 / 255f);
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




    IEnumerator MoveTo(GameObject piece, Vector3 throughPos, Vector3 toPos)
    {

        float count = 0, count2 = 0;
        Vector3 wasPos = piece.transform.position;
        throughPos.y = 4.0f;
        toPos.y = 4.0f;
        while (true)
        {
            count += Time.deltaTime;
            piece.transform.position = Vector3.Lerp(wasPos, throughPos, count);
            if (piece.transform.position == throughPos)
            {
                count2 += Time.deltaTime;
                piece.transform.position = Vector3.Lerp(throughPos, toPos, count2);
                if (count2 >= 1)
                {
                    piece.transform.position = toPos;

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
        toPos.y = 4.0f;
        while (true)
        {
            count += Time.deltaTime;
            piece.transform.position = Vector3.Lerp(wasPos, toPos, count);
            if (count >= 1)
            {
                piece.transform.position = toPos;

                break;
            }
            yield return null;
        }
    }
}
