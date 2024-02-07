using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CompanionGate : MonoBehaviour
{
    [SerializeField] int CompanionIndex=0;
    [SerializeField] Image CompanionBG;
    bool IsCollided = false;
    // Start is called before the first frame update
    void Start()
    {
        
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
            GameManager.Instance.PlaySound("EnhancementActivate");
            GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Success);
            IsCollided = true;

            GameManager.Instance.FireStatus(false);
            transform.DOMoveY(transform.localPosition.y - 7, 0.4f);
            GameManager.Instance.playerController.ActivatePlayerParticle(0);
            ActivateCompanion();
            GameManager.Instance.weaponManager.CurrentWeapon[0].transform.parent.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 5, 0.5f).OnComplete(() => {
                GameManager.Instance.FireStatus(true);
            });
            Destroy(gameObject, 3f);
        }
    }
    public void ActivateCompanion()
    {

        GameManager.Instance.ActivateCompanion(CompanionIndex);
    }
}
