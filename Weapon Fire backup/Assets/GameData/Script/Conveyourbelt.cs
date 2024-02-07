using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Conveyourbelt : MonoBehaviour
{
   [SerializeField]MeshRenderer Renderer;

    [SerializeField] float Speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        Renderer= GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
       
        float offsetY = Time.time * Speed;

        // Update the material's offset
        Renderer.material.mainTextureOffset = new Vector2(0, offsetY);
    }
}
