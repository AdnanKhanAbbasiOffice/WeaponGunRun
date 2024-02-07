using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
    public Transform BullteInstantiationPos;
    public Transform EnhancementParent;
    public Transform KeyChainPosParent;
    [SerializeField] internal List<GameObject> EnhancementPosition = new List<GameObject>();
    public List<GameObject> InstantiatedWeaponEnhacements = new List<GameObject>();
    public float DualWeaponGap = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
