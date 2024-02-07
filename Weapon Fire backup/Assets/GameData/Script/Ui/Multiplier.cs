using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Multiplier : MonoBehaviour
{
    //[SerializeField] float MultiplierValue = 1.0f;
    [SerializeField] TextMeshProUGUI MultiplierValueText;
    [SerializeField] Image BG;
    public List<CashGate> CashGates = new List<CashGate>();
    bool IsCollided;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void Initialize(int Value,Color color,int cashgatevalue,float FireValue)
    {
        MultiplierValueText.text = "X" + Value;
        BG.color = color;
        
        foreach(CashGate c in CashGates)
        {
            c.Initialize(cashgatevalue, FireValue);
            


        }
      
    }
    private void OnTriggerEnter(Collider other)
    {
       if (other.GetComponent<PlayerController>() && !IsCollided)
        {
            // GetFeatureValue();
            IsCollided = true;
            GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Failure);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
