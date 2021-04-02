using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMove : MonoBehaviour
{
   

    private bool _select = false;
    private Vector3 _movePosition;
    private GameObject _selectedPiece;

    public bool select { get { return _select; } }
    public Vector3 movePosition { get { return _movePosition; } }

    void Update()
    {
        selectAndMove();
        
    }

    

    

    void selectAndMove()
    {
        if (Input.GetMouseButtonDown(0) && GameObject.Find("Button").GetComponent<YutThrow>().throwing)
        {
            Debug.Log("pathfind");
            RaycastHit hit;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if (hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.CompareTag("Piece") && !_select)
                {

                    hit.collider.GetComponent<Renderer>().material.color = Color.red;
                    _select = true;
                    _selectedPiece = hit.collider.gameObject;
                    
                    hit.collider.GetComponent<PathFinding>().PathFind(_selectedPiece, GameObject.Find("Button").GetComponent<YutThrow>().selectNumber);
                    


                }
                else if (hit.collider.gameObject.CompareTag("FootHold") && _select)
                {
                    int index = GameObject.Find("Main Camera").GetComponent<YutTree>().FootSet.IndexOf(hit.collider.gameObject);
                    Debug.Log(index);
                    List<GameObject> set = GameObject.Find("Main Camera").GetComponent<YutTree>().FootSet;
                   
                    
                    StartCoroutine(MoveTo(_selectedPiece, set[index-1].transform.position, set[index].transform.position));
               
                    //_selectedPiece.GetComponent<Pieces>().posNumber = index;
                    _selectedPiece.GetComponent<Renderer>().material.color = new Color(102 / 255f, 123 / 255f, 255 / 255f, 255 / 255f);
                    _select = false;

                    GameObject.Find("Button").GetComponent<YutThrow>().throwing = false;


                }
            }
        }
    }


    

    IEnumerator MoveTo(GameObject piece, Vector3 toPos, Vector3 totoPos)
    {
        
        float count = 0, count2 = 0;
        Vector3 wasPos = piece.transform.position;
        toPos.y = 4.0f;
        totoPos.y = 4.0f;
        while (true)
        {
            count += Time.deltaTime;
            piece.transform.position = Vector3.Lerp(wasPos, toPos, count);
            if(piece.transform.position == toPos)
            {
                count2 += Time.deltaTime;
                piece.transform.position = Vector3.Lerp(toPos, totoPos, count2);
                if (count2 >= 1)
                {
                    piece.transform.position = totoPos;

                    break;
                }
            }
            
         
            
            yield return null;
        }
    }

    
}
