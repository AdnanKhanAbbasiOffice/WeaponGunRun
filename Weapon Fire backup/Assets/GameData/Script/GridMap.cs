using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public bool IsMapFull;
    public Grid CollidedGrid;
    public List<MouseDrag> AllEnhancementLevelPrefabs = new List<MouseDrag>();
    public List<Grid> AllGrids = new List<Grid>();
    public MouseDrag[] ActiveEnhancementLevels;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("Initialize", 0.5f);
    }


    void Initialize()
    {
        if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
        {
            InstantiateLevel(0, 0,false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateLevel(int levelIndex, int gridID, bool IsCollided)
    {

        Grid grid = AllGrids[gridID];

        if (IsCollided)
        {
            IsMapFull = false;
          //  print("gridID : " + gridID);
            GameManager.Instance.PlaySound("EnhancementLevelMerge");

    
        var InstantiatedLevel = Instantiate(AllEnhancementLevelPrefabs[levelIndex], grid.ItemPositioner.transform.position, Quaternion.identity);
           
                Destroy(Instantiate(grid.MergeParticlePrefabe, grid.ParticlePos.position, Quaternion.identity),3.0f);

            ActiveEnhancementLevels[grid.GridID] = InstantiatedLevel;

            if (grid.StoredLevel)
            {
                MouseDrag storelevel = grid.StoredLevel;
                Destroy(storelevel.gameObject);
            }

          
            grid.StoredLevel = InstantiatedLevel;
            InstantiatedLevel.transform.parent = gameObject.transform;
            InstantiatedLevel._GridMap = this;
            CollidedGrid = grid;
            InstantiatedLevel.PreviousGrid = grid;

        }
        else
        {
          if(IsMapFull)
            {
                return;
            }
           
            for (int i = 0; i < AllGrids.Count; i++)
            {
                if (AllGrids[i].StoredLevel == null)
                {
                  //  print("gridID else : " + gridID);
                    var InstantiatedLevel = Instantiate(AllEnhancementLevelPrefabs[levelIndex], AllGrids[i].ItemPositioner.transform.position, Quaternion.identity);

                    ActiveEnhancementLevels[i] = InstantiatedLevel;
                    AllGrids[i].StoredLevel = InstantiatedLevel;
                    InstantiatedLevel.transform.parent = gameObject.transform;
                    InstantiatedLevel._GridMap = this;
                    CollidedGrid = AllGrids[i];
                    InstantiatedLevel. PreviousGrid = AllGrids[i];
                    IsGridEmpty();
                    return;
                }

            }


       
        }
    
      
  

    }
    public bool IsGridEmpty()
    {
        IsMapFull = true;
        for (int i = 0; i < ActiveEnhancementLevels.Length; i++)
        {
            if (ActiveEnhancementLevels[i] == null)
            {

                IsMapFull = false;
                break;
            }

        
        }
        return IsMapFull;
    }
    public void SetLevelItemPosition(MouseDrag item)
    {
    //    print(item.gameObject);
 if (!CollidedGrid.StoredLevel)
    {
            item.transform.position = CollidedGrid.ItemPositioner.position;
            CollidedGrid.StoredLevel = item;
            item.PreviousGrid = CollidedGrid;
        }

    }

}
