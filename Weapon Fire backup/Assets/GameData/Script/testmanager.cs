using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmanager : MonoBehaviour
{

    public GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void change(int index)
    {
        foreach(GameObject p in players)
        {
            p.SetActive(false);
        }
        players[index].SetActive(true);
    }
}
