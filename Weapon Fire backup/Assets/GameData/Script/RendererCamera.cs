using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RendererCamera : MonoBehaviour
{
    bool IsStart;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Initialize", 1.5f);
    }
    void Initialize()
    {
        IsStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(IsStart)
        {
            if (GameManager.Instance.uiManager.gamePlay.WeaponNewEnhacemenetLevelPanel.activeSelf)
            {
                if(GameManager.Instance.weaponManager.EnhacementModelForRendererTexture)
                {
                    transform.LookAt(GameManager.Instance.weaponManager.EnhacementModelForRendererTexture.GetComponent<Enhancement>().ActiveLevel.transform.GetChild(0).GetChild(0));
                }
            }
            else if (GameManager.Instance.uiManager.levelComplete.KeyChaneUnloackedPanel.activeSelf)
            {
                if (GameManager.Instance.keyChainManager.KeyChainModelForRendererTexture)
                {
                    transform.LookAt(GameManager.Instance.keyChainManager.KeyChainModelForRendererTexture.transform.GetChild(0).GetChild(0).GetChild(0));
                }
            }
        }
      
    }
    public void Animate()
    {

        if (GameManager.Instance.weaponManager.EnhacementModelForRendererTexture)
        {

            GameManager.Instance.weaponManager.EnhacementModelForRendererTexture.GetComponent<Enhancement>().LevelDrop.gameObject.SetActive(false);
            Transform EnhancementPart = GameManager.Instance.weaponManager.EnhacementModelForRendererTexture.GetComponent<Enhancement>().ActiveLevel.transform.GetChild(0).GetChild(0);
           // EnhacementPosForRendererTexture


           Vector3 partScale= EnhancementPart.localScale;

            EnhancementPart.DOScale(new Vector3(0, 0, 0), 0.01f).OnComplete(()=> {

                Instantiate(GameManager.Instance.weaponManager.Confitti,GameManager.Instance.weaponManager.EnhacementPosForRendererTexture.position,Quaternion.identity);

                EnhancementPart.DOScale(partScale, 0.5f).SetEase(Ease.Linear);
            }) ;
          

           
            Vector3 targetrot = EnhancementPart.localEulerAngles;
            EnhancementPart.DOLocalRotate(new Vector3(30, 30, 0),1.0f).SetLoops(-1,LoopType.Incremental).SetRelative().SetEase(Ease.Linear); 
        }
    }
    public void AnimateKeyChain()
    {

        Transform keychain = GameManager.Instance.keyChainManager.KeyChainModelForRendererTexture.transform.GetChild(0).GetChild(0).GetChild(0);



        //Vector3 keychainScale = keychain.localScale;

        keychain.DOScale(new Vector3(0, 0, 0), 0.01f).OnComplete(() =>
        {

            Instantiate(GameManager.Instance.keyChainManager.Confitti, GameManager.Instance.keyChainManager.KeyChainPosForRendererTexture.position, Quaternion.identity);

            keychain.DOScale(new Vector3(1.2f,1.2f,1.2f), 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Vector3 targetrot = keychain.localEulerAngles;
                keychain.DOLocalRotate(new Vector3(0, 30, 0), 1.2f).SetLoops(-1, LoopType.Incremental).SetRelative().SetEase(Ease.Linear);
            });
        });

    }
}
