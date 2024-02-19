using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public List<GameObject> DeactivateUI=new List<GameObject>();

    [SerializeField]public  bool IsTutorialStep1Done = false;
    [SerializeField] public bool IsTutorialStep2Done = false;
    [SerializeField] public bool IsTutorialStep3Done = false;

    public GameObject TutorialStep1;
    public GameObject TutorialStep2;
    public GameObject TutorialStep3;


    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
        {
            //foreach(GameObject g in DeactivateUI)
            //{
             //   g.SetActive(false);
            //}
            DeactivateUI[0].GetComponent<Button>().enabled = false;
            TutorialStep1.SetActive(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Steps(int steps)
    {
        if(steps==1 && !IsTutorialStep1Done)
        {
            TutorialStep1.SetActive(false);
            TutorialStep2.SetActive(true);
            IsTutorialStep1Done = true;
            DeactivateUI[1].GetComponent<Button>().enabled = false;


        }
        else if(steps == 2 && !IsTutorialStep2Done)
        {
          //  print("step 2");

            TutorialStep1.SetActive(false);
            TutorialStep2.SetActive(false);
            TutorialStep3.SetActive(true);
            IsTutorialStep2Done = true;
            //gameObject.SetActive(false);
           // DeactivateUI[0].GetComponent<Button>().enabled = true;
          //  DeactivateUI[1].GetComponent<Button>().enabled = true;

        }
        else if (steps == 3 && !IsTutorialStep3Done)
        {
            //  print("step 3");
           
            
            TutorialStep1.SetActive(false);
            TutorialStep2.SetActive(false);
            TutorialStep3.SetActive(false);

            IsTutorialStep3Done = true;
     
            DeactivateUI[0].GetComponent<Button>().enabled = true;
            DeactivateUI[1].GetComponent<Button>().enabled = true;
            GameManager.Instance.playerController.WeaponRotation.SetActive(true);
            GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel = false;
            gameObject.SetActive(false);

        }

    }

}
