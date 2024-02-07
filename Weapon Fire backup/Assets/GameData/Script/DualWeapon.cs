using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DualWeapon : MonoBehaviour
{
    [SerializeField] Image DualWeaponBG;
    bool IsCollided = false;
    [SerializeField] int TotalDualGates=1;
   
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Initialize",0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>())
        {

            GameManager.Instance.PlaySound("GateHit");
            GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);

            Destroy(other.gameObject, 0f);
        }
        else if (other.GetComponent<PlayerController>() && !IsCollided)
        {

            IsCollided = true;
            GameManager.Instance.PlaySound("DualGun");
		GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Success);

            transform.DOMoveY(transform.localPosition.y - 5, 0.4f);
     
            GameManager.Instance.ActivateDualWeapon(TotalDualGates);
 
            Destroy(gameObject, 3f);
        }
    }

    public void Initialize()
    {

        DualWeaponBG .material= GameManager.Instance.weaponManager.CurrentWeaponInfo.DualWeaponMat;
    }
   
    float Gap=0;
  

}
