using DG.Tweening;
using UnityEngine;

public class CashCollectionControll : MonoBehaviour
{
    private int index = 0;
    private int index1 = 0;
    private int index2 = 0;

    [HideInInspector] internal float pricepercash = 0;
    [SerializeField] internal float totalCash = 0;
    [SerializeField] Transform TargetPosition;
    [SerializeField] Transform StartingPosition;
    public Vector3 Position2d;
    bool IsRewardRewarded;
    private void OnEnable()
    {
        index = 0;
        index1 = 0;
        index2 = 0;
        NextCashFloat();

        pricepercash = totalCash / (transform.childCount);
    }
    Sequence mySequence;
    void NextCashFloat()
    {
        Vector3 temppos;



        for (int i = 0; i < transform.childCount; i++)
        {
            mySequence = DOTween.Sequence();

            temppos = transform.GetChild(index).position;
            transform.GetChild(index).gameObject.SetActive(true);
            transform.GetChild(index).position = StartingPosition.position;

            mySequence.Append(transform.GetChild(index).DOMove(temppos, 0.35f).OnComplete(MoveToTarget).SetEase(Ease.InSine));
            mySequence.PrependInterval(i * 0.025f);
            index++;
        }
    }
    void MoveToTarget()
    {
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.GetChild(index1).DOMove(TargetPosition.position, 1f).OnComplete(AnimateCashs).SetEase(Ease.InSine));
        mySequence.Append(transform.GetChild(index1).DOScale(new Vector3(1, 1, 1), 1));
        mySequence.PrependInterval(index1 * 0.025f);
        index1++;
    }
    void AnimateCashs()
    {



        if (!GetComponent<AudioSource>().isPlaying && GameManager.Instance.sound)
            GetComponent<AudioSource>().Play();

        TargetPosition.parent.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        TargetPosition.parent.DOScale(Vector3.one, 0.02f).OnComplete(() =>
        {
            transform.GetChild(index2).gameObject.SetActive(false);
            index2++;
           // print(transform.GetChild(index2).gameObject.name +"  index : "+ index2  +" childs : "+ transform.childCount);

            if (index2 >= transform.childCount - 1 && !IsRewardRewarded)
            {
                IsRewardRewarded = true;
              //  print("reward finish");
               //  print(transform.GetChild(index2).gameObject.name +"  index : "+ index2  +" childs : "+ transform.childCount);

                if (GameManager.Instance.keyChainManager.ActivatedKeyChainInfo && !GameManager.Instance.keyChainManager.AllKeyChainsUnloacked)
                {
                    if(GameManager.Instance.keyChainManager.ActivatedKeyChainInfo == GameManager.Instance.keyChainManager.currentKeyChainInfo)
                    { 
                  //  if (GameManager.Instance.keyChainManager.ActivatedKeyChainInfo.IsActivated)
                    //{
                        StartCoroutine(GameManager.Instance.keyChainManager.ActivateKeyChainUnloackedPanel());
                   // }
                }
                    else
                    {
                        //print("complete game 1");
                        GameManager.Instance.GameComplete();
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                   // print("complete game 2");
                   // print("name :  "+gameObject.name);
                    GameManager.Instance.GameComplete();
                    gameObject.SetActive(false);
                }
                //  gameObject.SetActive(false);
                GetComponent<AudioSource>().Pause();

            }

            GameManager.Instance.uiManager.AddCashUpdate(pricepercash);
            GameManager.Instance.FirebaseEvents("earn_virtual_currency", "LevelPass", totalCash + "");

        });




        //gameObject.SetActive(false);
    }

    void Deactive()
    {
        gameObject.SetActive(false);
        GetComponent<AudioSource>().Pause();
    }
}
