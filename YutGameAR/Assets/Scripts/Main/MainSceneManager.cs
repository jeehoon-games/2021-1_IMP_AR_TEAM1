using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    #region public & private instance variables / Init method
    
    public GameObject CreateRoomMenu;
    public GameObject FindRoomMenu;
    public Canvas MenuCanvas;
    

    private void Init()
    {
        
    }
    
    #endregion
    
    
    #region Unity Event Functions
    
    void Start()
    {
        Init();
    }
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t0 = Input.GetTouch(0);

            if (t0.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(t0.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.name.Equals("CreateRoomMenu"))
                    {
                        
                    }

                    if (hit.collider.gameObject.name.Equals("FindRoomMenu"))
                    {
                        
                    }
                }
            }
        }
    }
    
    #endregion
}
