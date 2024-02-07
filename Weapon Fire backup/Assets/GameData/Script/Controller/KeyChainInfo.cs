using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeyChain", menuName = "KeyChain/NewKeyChain")]
public class KeyChainInfo : ScriptableObject
{
    public bool IsActivated;
    public int KeyChainIndex = 0;
    public GameObject KeyChain;
    public float FillPercentage;
    public Sprite KeyChainSprite;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
