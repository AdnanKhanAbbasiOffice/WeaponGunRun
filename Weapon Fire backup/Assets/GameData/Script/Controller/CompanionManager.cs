using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public CompanionController[] AllCompanions;
  [HideInInspector]  public CompanionController CurrentCompanion;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateCompanion(int index)
    {
       // print("Original companion id  " + index);

        if (index < 0 )
        {
            return;
        }
        if(CurrentCompanion)
        {
            if(CurrentCompanion.CompanionIndex == index)
            {
               // print("Already companion is on Ground");
                return;
            }

        }
        GameManager.Instance.CurrentCompanion = index;
        GameObject companion = Instantiate(AllCompanions[index].gameObject,GameManager.Instance.playerController.transform.position,Quaternion.identity);
        CurrentCompanion = companion.GetComponent<CompanionController>();
        companion.transform.position = GameManager.Instance.playerController.transform.position + CurrentCompanion.Offset;
      
        // GameManager.Instance.SetWeaponAsChild(weapon.transform);
        
       // CurrentWeaponInfo = AllWeaponsInfo[index];

       // InstantiatePurchasedWeaponsEnhancement();
    }
}
