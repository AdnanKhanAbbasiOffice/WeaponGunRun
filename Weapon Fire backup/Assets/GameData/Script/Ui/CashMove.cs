using DG.Tweening;
using UnityEngine;

public class CashMove : MonoBehaviour
{

    internal Vector3 startingPosition;
    // Start is called before the first frame update
    private void OnEnable()
    {
        transform.position = startingPosition;
        transform.DOMove(GameManager.Instance.uiManager.gamePlay.cashTargetPositon.position, 0.15f + Random.Range(0.2f, 0.4f)).OnComplete(AnimateCoins).SetEase(Ease.InSine);
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    void AnimateCoins()
    {
        GameManager.Instance.uiManager.gamePlay.cashTargetPositon.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        GameManager.Instance.uiManager.gamePlay.cashTargetPositon.DOScale(Vector3.one, 0.5f);
        gameObject.SetActive(false);
    }
}
