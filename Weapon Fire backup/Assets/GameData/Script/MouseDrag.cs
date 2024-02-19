using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MouseDrag : MonoBehaviour
{
    public GridMap _GridMap;
    private Plane daggingPlane;
    private Vector3 offset;

    public Camera mainCamera;
    [SerializeField] float min_YAxis = 0.4f;
    [SerializeField] float max_YAxis = 3.5f;


    [SerializeField] TextMeshProUGUI EnhancementLevelText;
    [SerializeField] GameObject NextLevel;
    [HideInInspector]
    public bool IsInGrid;
    [HideInInspector]
    public bool IsInGunArea;
    [HideInInspector]
    public Grid PreviousGrid;
    public int LevelID = 0;
    bool IsMouseDown;
    // public Camera gridcamera;
    void Start()
    {
        mainCamera = GameManager.Instance._CameraControll.GridCamera.GetComponent<Camera>();
        EnhancementLevelText.text = "Lv." + (LevelID + 1);
    }
    public GameObject CurrentShadow;
    bool IsEnhacementActivated;
    void OnMouseDown()
    {
      //  print("mouse down 1");
        if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
        {

            if (!GameManager.Instance.tutorialController.IsTutorialStep1Done)
            {
              //  print("return");
                return;
            }


        }
      //  print("mouse down 2");
        GameManager.Instance.playerController.WeaponRotation.SetActive(false);
        IsMouseDown = true;
        daggingPlane = new Plane(mainCamera.transform.forward,
                              transform.position);
        Ray camRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(camRay.origin, camRay.direction * 10, Color.green);

        float planeDistance;
        daggingPlane.Raycast(camRay, out planeDistance);

        offset = transform.position - camRay.GetPoint(planeDistance);
        //  print(planeDistance);
        var temp = transform.position;
        temp.y = min_YAxis;
        transform.position = temp;




        PreviousGrid.StoredLevel = null;

        if (GameManager.Instance.weaponManager.CurrentWeaponInfo.EnhancementLevelIds[GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter] != -1)
        {
            EnhancementLevel enhancement = GameManager.Instance.weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements[GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter].GetComponent<Enhancement>().ActiveLevel.GetComponent<EnhancementLevel>();
            // enhancement.GetComponent<Enhancement>().ActivateEnhancement(LevelNO, CurrentWeapon[0], CurrentWeaponInfo);

            if (LevelID <= enhancement.LevelID)
            {
              //  print("already enhancement is activated");
                IsEnhacementActivated = true;
                return;
            }
        }

        if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
        {
            if (GameManager.Instance.tutorialController.IsTutorialStep2Done && !GameManager.Instance.tutorialController.IsTutorialStep3Done)
            {
                CurrentShadow = GameManager.Instance.ActiveWeaponEnhacementShadow(LevelID);
    }
            else
            {
                if(GameManager.Instance.tutorialController.IsTutorialStep3Done)
                {
                    CurrentShadow = GameManager.Instance.ActiveWeaponEnhacementShadow(LevelID);
                }
            }
           
        }
        else
        {

            CurrentShadow = GameManager.Instance.ActiveWeaponEnhacementShadow(LevelID);

        }


    }
    public Vector3 DragSpeed = new Vector3(2.0f,2.0f,2.0f);
    void OnMouseDrag()
    {
       // print("mouse drag 1");
        if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
        {
         
            if (!GameManager.Instance.tutorialController.IsTutorialStep1Done)
            {
            

                return;
            }
       

        }

       // print("mouse drag 2");

        Ray camRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        float planeDistance;
        daggingPlane.Raycast(camRay, out planeDistance);

        transform.position = (camRay.GetPoint(planeDistance) + offset);// *DragSpeed*Time.deltaTime;
   
        var temp = transform.position;
     
       

        temp.y = Mathf.Clamp(transform.position.y, min_YAxis, max_YAxis);
        transform.position = temp;
    }
    private void OnMouseUp()
    {
        IsMouseDown = false;

        Destroy(CurrentShadow);
        CurrentShadow = null;
        InstantiateLevel();
  
        if (IsEnhacementActivated)
        {
            if(GameManager.Instance.weaponManager.CurrentWeaponInfo.EnhancementLevelIds[GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter]>= GameManager.Instance.weaponManager.CurrentWeaponInfo.EnhancementLevelIds.Count-1)
            {

            
            GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter++;
            if (GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter > GameManager.Instance.weaponManager.CurrentWeaponInfo.EnhancementLevelIds.Count - 1)
            {
                GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter = 0;
            }
        }
        }

        IsEnhacementActivated = false;
        GameManager.Instance.playerController.WeaponRotation.SetActive(true);


    }



    public void InstantiateLevel()
    {
        if(IsInGrid)
        {
      
            GameManager.Instance.weaponManager.ShowLevel(GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter);
          
            if (!_GridMap.CollidedGrid.StoredLevel)
            {

                if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
                {
                    if (!GameManager.Instance.tutorialController.IsTutorialStep3Done)
                    {
                      
                        _GridMap.CollidedGrid = PreviousGrid;
                        _GridMap.SetLevelItemPosition(this);
                     
                        return;
                    }
                }

                _GridMap.SetLevelItemPosition(this);
          
            }
            else
            {
                if (!NextLevel)
                {
                    return;
                }


                if (_GridMap.CollidedGrid.StoredLevel.LevelID == LevelID)
                {
 
                    _GridMap.InstantiateLevel(NextLevel.GetComponent<MouseDrag>().LevelID, _GridMap.CollidedGrid.GridID, true);
                    if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
                    {
                        if (!GameManager.Instance.tutorialController.IsTutorialStep2Done)
                        {
                            GameManager.Instance.tutorialController.Steps(2);

                        }
                    }
                        DestroyLevel();
                  
                }
                else if (_GridMap.CollidedGrid.StoredLevel.LevelID != LevelID)
                {
                   
                    _GridMap.CollidedGrid = PreviousGrid;
                    _GridMap.SetLevelItemPosition(this);

                
                }
            }
            IsInGrid = false;
        }
        else
        {
          if(IsInGunArea)
            {
               
                if(IsEnhacementActivated)
                {
                    _GridMap.CollidedGrid = PreviousGrid;
                    _GridMap.SetLevelItemPosition(this);
            
                    return;
                }

                if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
                {
                    if (!GameManager.Instance.tutorialController.IsTutorialStep2Done)
                    {
                      
                        _GridMap.CollidedGrid = PreviousGrid;
                        _GridMap.SetLevelItemPosition(this);
                      
                        return;
                    }
                }
 
                GameManager.Instance.ActivateWeaponEnhancement(LevelID);

                if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
                {
                    if (!GameManager.Instance.tutorialController.IsTutorialStep3Done)
                    {
                        GameManager.Instance.tutorialController.Steps(3);
                     
                  
                    }
                }
                DestroyLevel();
            }
          else
            {
                GameManager.Instance.weaponManager.ShowLevel(GameManager.Instance.weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter);

                _GridMap.CollidedGrid = PreviousGrid;
                _GridMap.SetLevelItemPosition(this);
            }

         
        }

       
     
    }

    public void DestroyLevel()
    {
        for (int i = 0; i < _GridMap.ActiveEnhancementLevels.Length; i++)
        {
            if (_GridMap.ActiveEnhancementLevels[i] == this)
            {
                Destroy(gameObject);
                _GridMap.ActiveEnhancementLevels[i] = null;
            }
        }
    }
  
    private void OnTriggerStay(Collider other)
    {

       

        if (other.tag == "GunArea")
        {
          //  print(other.tag);
            //  max_YAxis = 3.5f;
            //  min_YAxis = 0.4f;
            IsInGunArea = true;
        }
        else if (other.GetComponent<Grid>() && IsMouseDown)
        {

            Grid collidedGrid = other.GetComponent<Grid>();
          //  print(collidedGrid.gameObject);
            
            if (!NextLevel)
            {
                return;
            }

            _GridMap.CollidedGrid = collidedGrid;

            IsInGrid = true;
        
        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GunArea")
        {
          //  print(other.tag);
            //  max_YAxis = 3.5f;
            //  min_YAxis = 0.4f;
            IsInGunArea = false;
        }
        else if (other.GetComponent<Grid>())
        {
         
            if (!NextLevel)
            {
                return;
            }
            _GridMap.CollidedGrid = null;
            IsInGrid = false;


        }
    }

   
}
