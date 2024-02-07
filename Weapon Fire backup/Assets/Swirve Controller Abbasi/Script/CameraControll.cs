using UnityEngine;
using DG.Tweening;
public class CameraControll : MonoBehaviour
{
PlayerController Player;
	public GameObject PlayerCamera;
	public GameObject GridCamera;
	float speed;

    float MinX = -3f;


	float MaxX = 3.5f;
	Vector3 tempPosition;
	public Transform MainCameraPos;
	public Transform GridCameraPos;

	public GameObject NewWeponRendererCam;

	// Start is called before the first frame update
	void Start()
	{
		tempPosition = transform.position;
	
	}

        public void Initializ(PlayerController player)
        {
		Player = player;
		speed = Player.CameraSpeed;
		MinX = Player.CameraMinX;
		MaxX = Player.CameraMaxX;
	    }

	private void FixedUpdate()
	{


		if (GameManager.Instance.IsGameStart && !GridCamera.activeSelf)
		{ 
		tempPosition = Player.transform.position;
		tempPosition.x = Mathf.Clamp(tempPosition.x, MinX, MaxX);
		transform.position = Vector3.Lerp(transform.position, tempPosition, Time.deltaTime * speed);
	}
	}

	public void ZoomIn()
    {
		var temprotation = transform.GetChild(0).transform.localEulerAngles;
		temprotation.y -= -2;

		transform.GetChild(0).DOLocalMoveZ(transform.GetChild(0).transform.localPosition.z+1.5f,0.7f);
			transform.GetChild(0).DOLocalRotate(temprotation, 0.7f).SetDelay(0.3f);
	

	}
	public void Rotate()
    {
		GameManager.Instance.IsGameStart = false;

		transform.GetChild(0).DOLocalMove( new Vector3(6f,7.9f,-9), 0.5f);

		var temprotation = transform.GetChild(0).transform.localEulerAngles;
		temprotation.x = 25;
		temprotation.y = -40;
		temprotation.z = 0;

		
		transform.GetChild(0).DOLocalRotate(temprotation, 0.5f).SetDelay(0f).OnComplete(()=> {

			GameManager.Instance.ShowKeyChainPanel();
		});
	
		//GameManager.Instance.GameComplete();
	}
	public void ZoomOut()
	{
		//GameManager.Instance.IsGameStart = false;
		//transform.DOLocalMoveX(0f, 0.5f);
		//transform.GetChild(0).DOLocalMove(new Vector3(0f, 18f, -15), 0.5f);

		//var temprotation = transform.GetChild(0).transform.localEulerAngles;
		//temprotation.x = 25;
		//temprotation.y = 0;
		//temprotation.z = 0;


		//transform.GetChild(0).DOLocalRotate(temprotation, 0.5f).SetDelay(0f).OnComplete(()=> {

       
		//});

		//GameManager.Instance.weaponManager.FindNextWeapon().transform.DOScale(new Vector3(1f,1f,1f),1f).OnComplete(()=> {

	
		//}).SetEase(Ease.InBack).SetLoops(2,LoopType.Yoyo);
		//GameManager.Instance.GameComplete();

		
	}



}
