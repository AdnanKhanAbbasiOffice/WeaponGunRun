using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CompanionController : MonoBehaviour
{
    public int CompanionIndex=0;
    PlayerController Player;
    Vector3 tempPosition;
    public float MinX = -1;
    public float MaxX = 1;
    public Vector3 Offset;

    [SerializeField] GameObject BulletPrefab;
    public Transform BullteInstantiationPos;

    [SerializeField] float FireRate=0.5f;
    [SerializeField] float FireRange=10;
    [SerializeField] float FirePower = 10;

    [SerializeField] Transform GunBarrel;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.Instance.playerController;
        tempPosition = transform.position;
        StartCoroutine(Fire());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {


        if (GameManager.Instance.IsGameStart)
        {
            tempPosition = Player.transform.position;
            tempPosition.x = Mathf.Clamp(tempPosition.x, MinX, MaxX);
            transform.position = Vector3.Lerp(transform.position, tempPosition+ Offset, Time.deltaTime * 30f);
        }
    }
    public IEnumerator Fire()
    {
        while (GameManager.Instance.IsGameStart)
        {
            yield return new WaitForSeconds(FireRate);
            FireRate = GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRate;
            FireRange = GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRange;
            FirePower = GameManager.Instance.weaponManager.CurrentWeaponInfo.FirePower;


            if (GameManager.Instance.IsGameStart && GameManager.Instance.IsFireStart)
            {

                GunBarrel.DOLocalRotate(new Vector3(0,0,-360),0.1f).SetEase(Ease.Linear).SetLoops(3,LoopType.Restart).SetRelative(true);

                  GameObject bullet = Instantiate(BulletPrefab, BullteInstantiationPos.position, Quaternion.identity);
                    bullet.GetComponent<BulletCompanion>().MoveForward(FireRange, FirePower);
                // print("Fire");
                transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f);
                //transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f).OnComplete(() =>
                //    {

                        
                //    });

               
            }
        }


    }
}
