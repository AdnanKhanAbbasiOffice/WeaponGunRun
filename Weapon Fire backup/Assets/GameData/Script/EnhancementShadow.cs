using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancementShadow : MonoBehaviour
{
    public Transform LevelDropShadow;
    private void Awake()
    {
        LevelDropShadow.gameObject.SetActive(false);
        if (GameManager.Instance.levelManager[GameManager.Instance.currentLevel].IsTutorialLevel)
        {
            if (GameManager.Instance.tutorialController.IsTutorialStep2Done && !GameManager.Instance.tutorialController.IsTutorialStep3Done)
            {
                LevelDropShadow.gameObject.SetActive(true);
            }
}
     
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
