using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] bool IsShowUI = true;
    [SerializeField] bool IsIdle=true;
   
    [SerializeField] bool IsLeftToRightMove;
    [SerializeField] bool IsRightToLefttMove;


    public GameObject RewardPrefab;

    bool IsPushedBack;
    [HideInInspector]
    public Animator anim;
    public string CurrentAnimation = "Idle";
    [SerializeField] float FireValue = 1.0f;
    [SerializeField] float EnemyHealth = 1.0f;
 
    [SerializeField] float Speed = 1;
    public GameObject DeathParticlePrefab;
    public Transform DeathParticlePos;

    [SerializeField] TextMeshProUGUI EnemyValueText;
   

    // Start is called before the first frame update
    void Start()
    {
       anim = GetComponent<Animator>();
        EnemyValueText.text = EnemyHealth.ToString();
        if (IsIdle)
        {
            CurrentAnimation = "Idle";
            IsLeftToRightMove = false ;
            IsRightToLefttMove = false;
        }
        else
        {
            CurrentAnimation = "Walk";
            if(IsLeftToRightMove)
            {
                var pos = transform.localPosition;
                pos.x = -3;
                transform.localPosition = pos;

                transform.DOMoveX(3.0f, Speed).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo).SetSpeedBased(true);
            }
            else if(IsRightToLefttMove)
            {
                var pos = transform.localPosition;
                pos.x = 3;
                transform.localPosition = pos;

                transform.DOMoveX(-3.0f, Speed).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo).SetSpeedBased(true);

            }
            else
            {
                CurrentAnimation = "Idle";
            }

            if(!IsShowUI)
            {
                EnemyHealth = 1;
            }

        }
        PlayAnimation(CurrentAnimation);
    }

    // Update is called once per frame
    void Update()
    {
     

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() || other.GetComponent<BulletCompanion>())
        {
            if (other.GetComponent<Bullet>())
            {
                GameManager.Instance.PlaySound("EnemyHit");
            }

            GateHitted();
            Destroy(other.gameObject, 0.0f);
        }
        else if (other.GetComponent<PlayerController>())
        {
            if(!IsPushedBack)
            {
                GameManager.Instance.playerController.PushBack();
                IsPushedBack = true;
            }
            else
            {
                
                Destroy(gameObject, 3f);
            }
        

        

        
        }
    }
    public void GateHitted()
    {
        EnemyHealth -= FireValue;
        EnemyValueText.text = EnemyHealth.ToString();
        if (EnemyHealth <= 0)
        {
           
            if (DeathParticlePrefab)
            {
                Instantiate(DeathParticlePrefab, DeathParticlePos.position, Quaternion.identity);

            }

            GiveCashReward();
            Destroy(gameObject, 0f);
        }
        else
        {
            PlayAnimation("Hit");
        }

    }
    public void PlayAnimation(string AnimationName)
    {

        anim.SetTrigger(AnimationName);
        CurrentAnimation = AnimationName;
    }
    void GiveCashReward()
    {
        GameObject reward = Instantiate(RewardPrefab, transform.position, Quaternion.identity);
        reward.transform.DOLocalJump(reward.transform.localPosition + new Vector3(0, 0, Random.Range(2, 6)), 2, 1, 0.3f).SetEase(Ease.Linear);
        reward.transform.parent = null;
        reward.GetComponent<Rigidbody>().useGravity = true;
    }
}
