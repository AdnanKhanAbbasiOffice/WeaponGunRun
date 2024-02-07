using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="NewWeapon",menuName = "Weapon/NewWeapon")]
public class WeaponInfo : ScriptableObject
{
    [SerializeField] internal GameObject WeaponPrefab;
    [SerializeField] internal GameObject WeaponModel;
    [SerializeField] internal GameObject BulletPrefab;
    [SerializeField] internal string WeaponName="Weapon";

   // [Tooltip("Instantiation Delay of Bullet")]
    [SerializeField] internal float FireRate = 0.6f;
    [SerializeField] internal float FireRateIncrement = 0.09f;
    [SerializeField] internal float FireRateLowerLimit = 0.3f;
    [SerializeField] internal float FireRateUpperLimit = 0.6f;
    
    // [Tooltip("Range Of Bullet")]
    [SerializeField] internal float FireRange = 1.0f;
    [SerializeField] internal float FireRangeIncrement = 1.0f;
    [SerializeField] internal float FireRangeLowerLimit = 1.0f;
    [SerializeField] internal float FireRangeUpperLimit = 5.0f;
  
    // [Tooltip("Speed Of Bullet")]
    [SerializeField] internal float FirePower = 40.0f;
    [SerializeField] internal float FirePowerIncrement = 15.0f;
    [SerializeField] internal float FirePowerLowerLimit = 40.0f;
    [SerializeField] internal float FirePowerUpperLimit = 100.0f;


    [Header("UI Values")]
    [SerializeField] internal float FireRateUI = 0.0f;
    [SerializeField] internal float FireRangeUI = 0.0f;
    [SerializeField] internal float FirePowerUI = 0.0f;

    [SerializeField] internal float FireRateIncrementUI = 0.0f;
    [SerializeField] internal float FireRangeIncrementUI = 0.0f;
    [SerializeField] internal float FirePowerIncrementUI = 0.0f;


    [SerializeField] internal List<GameObject> WeaponEnhancementPrefabs = new List<GameObject>();
    [SerializeField] internal List<GameObject> WeaponEnhancementShadowPrefabs = new List<GameObject>();
    [SerializeField] internal List<GameObject> InstantiatedWeaponEncements = new List<GameObject>();
    [SerializeField] internal List<int> EnhancementLevelIds = new List<int>();

    // [SerializeField] internal GameObject[] WeaponEncementPrefabs;
    [SerializeField] internal int WeaponEnhancementCounter = 0;

    public Material DualWeaponMat;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  

}
