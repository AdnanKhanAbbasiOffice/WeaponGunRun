using UnityEngine;

public class Cash : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionEffect;
    private bool isMagnetMoveToPlayer = false;

    private void Update()
    {
        //if (GameController.Instance.IsMagnetPick)
        //    if (Vector3.Distance(GameController.Instance.mainAngel.transform.position, transform.position) < 5 && !isMagnetMoveToPlayer)
        //    {
        //        isMagnetMoveToPlayer = true;
        //        //transform.DOMove(Player.Instance.animAladin.transform.position, 0.4f).OnComplete(collectStar).SetEase(Ease.InSine);
        //    }

        //if (isMagnetMoveToPlayer)
        //{
        //    transform.position = Vector3.Lerp(transform.position, GameController.Instance.mainAngel.transform.position + new Vector3(0, 1, 0), Time.deltaTime * 5);

        //    if (Vector3.Distance(GameController.Instance.mainAngel.transform.position, transform.position) < 1.5f)
        //    {
        //        collectStar();
        //        //transform.DOMove(Player.Instance.animAladin.transform.position, 0.4f).OnComplete(collectStar).SetEase(Ease.InSine);
        //    }
        //}
    }

    public void collectStar()
    {

        GameManager.Instance.uiManager.UpdateCashCountTxt(Camera.main.WorldToScreenPoint(transform.position), 1);
        if (ExplosionEffect != null)
            Instantiate(ExplosionEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        gameObject.SetActive(false);
    }
}
