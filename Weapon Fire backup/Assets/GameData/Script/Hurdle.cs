using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Hurdle : MonoBehaviour
{
    bool IsCollided;
    public GameObject RewardPrefab;
    [SerializeField] float RewardValue = -15.0f;
    [SerializeField] Transform InstantiatePos;
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
            // GateHitted();
            Destroy(other.gameObject, 0.01f);
        }
        else if (other.GetComponent<PlayerController>() && !IsCollided)
        {
            IsCollided = true;
            GameManager.Instance.PlaySound("EnemyHit");

            GameManager.Instance.weaponManager.CurrentWeapon[0].transform.parent.DOShakeRotation(0.5f,20,5,45).SetEase(Ease.OutQuad).OnComplete(() => {


               // GameManager.Instance.FireStatus(true);


            });
            GameManager.Instance.uiManager.AddCashUpdate(RewardValue);
            GameManager.Instance.FirebaseEvents("earn_virtual_currency", "LevelPlay", RewardValue + "");
            GiveCashReward();
        }

        }
    void GiveCashReward()
    {
        for(int i=0;i<3;i++)
        {
            GameObject reward = Instantiate(RewardPrefab, InstantiatePos.transform.position, Quaternion.identity);
            reward.transform.DOLocalJump(reward.transform.localPosition + new Vector3(Random.Range(-3, 3), 0, Random.Range(16, 20)), 3, 1, 0.5f).SetEase(Ease.Linear);
            reward.transform.parent = null;
            reward.GetComponent<Rigidbody>().useGravity = true;
        }
       

    }
}
