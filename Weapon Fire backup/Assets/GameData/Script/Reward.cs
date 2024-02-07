using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    [SerializeField] float RewardValue = 5.0f;
    public GameObject RewardeParticle;
    public Transform ParticlePos;
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
         if (other.GetComponent<PlayerController>())
        {
            // GetFeatureValue();
            GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Success);
            //   GameManager.Instance.totalCash += RewardValue;
            GameManager.Instance.uiManager.AddCashUpdate(RewardValue);
            GameManager.Instance.FirebaseEvents("earn_virtual_currency", "LevelPlay", RewardValue + "");

            Instantiate(RewardeParticle, ParticlePos.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
