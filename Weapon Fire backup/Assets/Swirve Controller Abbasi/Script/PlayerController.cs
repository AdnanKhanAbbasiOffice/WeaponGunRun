using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{

	public static PlayerController Instance;
	[SerializeField] bool IsPlayerAnimated = true;
	[SerializeField]
	Transform PlayerObj;
	
	private Rigidbody rb;
	internal Animator anim;
	public string AnimationName = "Idle";
	[SerializeField] Transform ParticlePos;

	#region Variables


	Transform viewMainTarget;
	Transform viewTarget;

	[Header("PLAYER MOVEMENT VALUES")]

	[SerializeField] [Range(0f, 1.5f)] public float HorizontalSped = 15f;

	[Range(0, 3f)] public float forwardSpeed = 0.1f;
	public float LevelWiseSpeedOffset = 0.05f;
	[SerializeField] float speedoffset;

	[SerializeField] float PushBackSpeed = 0.7f;
	bool IsPushBack = false;

	[SerializeField] float minX = -3f;
	[SerializeField] float maxX = 3f;

	[SerializeField] float TiltValue = 7;
	[SerializeField] float TiltSpeed = 2;

	public float LevelScore;


	[Header("CAMERA CONTROLLER VALUES")]
	public float CameraSpeed = 2;
	public float CameraMinX = -3f;
	public float CameraMaxX = 3.5f;

	[Header("BULLET CONTROLLER VALUES")]




	bool canDrag;
	Vector3 offset;
	Vector3 screenPoint;
	Vector3 cursorPosition;
	public float tempxPosition = 0;

	Vector3 previoustouch;

	Vector3 previoustouchpos;

	public GameObject[] PlayerParticlesPrefab;

	public GameObject WeaponRotation;
	#endregion  Variables

	#region UnityFunction
	private void Awake()
	{

		initial();

	}
	private void initial()
	{
		Instance = this;

		//Camera.main.transform.parent.GetComponent<CameraControll>().Initializ(this);
		//	PlayerObj = transform.GetChild(0);
		viewMainTarget = transform.GetChild(1);
		viewTarget = viewMainTarget.GetChild(0);
		viewMainTarget.parent = null;

		if (IsPlayerAnimated)
		{
			anim = PlayerObj.GetChild(0).GetComponent<Animator>();

		}
		rb = GetComponent<Rigidbody>();

		PlayAnimation(AnimationName);

		speedoffset = GameManager.Instance.currentLevel * LevelWiseSpeedOffset;
		speedoffset = Mathf.Clamp(speedoffset, 0, 1.5f);
	}
	private void FixedUpdate()
	{
		if (!GameManager.Instance.IsGameStart)
		{
			return;
		}

		ForwardMovement();
	}

	private void Update()
	{
		if (!GameManager.Instance.IsGameStart)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			MouseDown();
		}
		else if (Input.GetMouseButton(0))
		{
			_moveFactorX = Input.mousePosition.x - _lastFrameFingerPositionX;
			_lastFrameFingerPositionX = Input.mousePosition.x;
		}
		else if (Input.GetMouseButtonUp(0))
		{

			_moveFactorX = 0f;

			canDrag = false;
			offset = Vector3.zero;
			screenPoint = Vector3.zero;

		}
		if (canDrag)
		{

		}
	}
	#endregion UnityFunction

	#region CustomFunction

	[SerializeField] public float DragThreshold = 0.1f;
	[SerializeField] public float maxSwerveAmount = 1f;

	private float _lastFrameFingerPositionX;
	private float _moveFactorX;
	public float MoveFactorX => _moveFactorX;


	public void Swirve()
	{
		float swerveAmount = Time.deltaTime * HorizontalSped * MoveFactorX;
	//	swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);
		transform.Translate(swerveAmount, 0, 0, Space.World);

		var pos = transform.position;

		pos.x = Mathf.Clamp(transform.position.x, minX, maxX);
		transform.position = pos;
	}
	void ForwardMovement()
	{




		if (Input.GetMouseButton(0) && canDrag)
		{

			if ((previoustouch - Input.mousePosition).magnitude > DragThreshold)
			{



				//cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
				//cursorPosition.x = Mathf.Clamp(cursorPosition.x, minX, maxX);
				//print("cursorPosition.x : " + cursorPosition.x);
				//   if (!IsCollidedWithWall)

				// tempxPosition = Mathf.Lerp(tempxPosition, cursorPosition.x, Time.fixedDeltaTime * HorizontalSped/*(speed + PlayerCar.Instance.CurrentCarIndex * 0.2f)*/);
				Swirve();



			}

		}

		//transform.position = new Vector3(tempxPosition, transform.position.y, transform.position.z);


		Vector3 positionspherechild = viewTarget.position;
		positionspherechild.x = transform.position.x;
		viewTarget.position = positionspherechild;

		viewMainTarget.position = transform.position;

		if (canDrag)
		{

			SmoothLook(viewTarget.position);
		}
		else
		{


			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.zero), Time.deltaTime * 20);



		}
		if (!IsPushBack)
		{
			
			transform.Translate(new Vector3(0, 0, 0.6f) * (forwardSpeed+speedoffset));

		}
	}
	void MouseDown()
	{
		previoustouch = Input.mousePosition;
		previoustouchpos = Input.mousePosition;
		_lastFrameFingerPositionX = Input.mousePosition.x;

		canDrag = true;
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

	}

	private void LateUpdate()
	{
		//previoustouch=Input.mousePosition;
		previoustouchpos = Input.mousePosition;
		//previoustouch = Input.mousePosition;
	}



	void SmoothLook(Vector3 targetPositon)
	{
		var lookPos = targetPositon - transform.position;

		lookPos.Normalize();

		//print(lookPos.normalized.x);
		if (lookPos.x < -0.001f)
		{
			//print("left");
			//IsRightMovement = false;
			//	IsLeftMovement = true;
			Tilt(TiltValue);

		}
		else if (lookPos.x > 0.001f)
		{
			//	print("right");
			//IsLeftMovement = false;
			//IsRightMovement = true;
			Tilt(-TiltValue);

		}




	}



	void Tilt(float tiltAngle)
	{
		// Get the current rotation
		Quaternion currentRotation = transform.rotation;

		// Calculate the new rotation by tilting along the Z-axis
		Quaternion newRotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(tiltAngle, -TiltValue, TiltValue));

		// Interpolate smoothly between the current and new rotation (optional)
		// You can adjust this value for different levels of smoothness
		transform.rotation = Quaternion.Slerp(currentRotation, newRotation, TiltSpeed * Time.deltaTime);
	}
	public void PlayerMoveLimit(float minX_Value, float maxX_Value)
	{
		minX = minX_Value;
		maxX = maxX_Value;

	}

	public void PlayAnimation(string name)
	{
		if (IsPlayerAnimated)
		{


			AnimationName = name;
			anim.SetTrigger(name);
		}

	}

	public void SetWeaponAsChild(Transform weapon)
	{
		var temp = weapon.transform.position;
		temp.x = PlayerObj.position.x;

		weapon.transform.position = temp;

		weapon.parent = PlayerObj;

	}

	public void PushBack()
	{
		GameManager.Instance.PlaySound("EnemyHit");
		GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Failure);
		
		IsPushBack = true;
		transform.DOMoveZ(transform.localPosition.z - 5, PushBackSpeed).OnComplete(() => {

			//	transform.Translate(new Vector3(0, 0, 0.6f) * forwardSpeed);
			IsPushBack = false;

		});
	}

	public void ActivatePlayerParticle(int index)
	{

		GameObject particle = Instantiate(PlayerParticlesPrefab[index], ParticlePos.position, Quaternion.identity);
		particle.transform.parent = ParticlePos;
		//Destroy(particle,0.5f);


	}
	public void AddLevelScoreCashUpdate(float value)
	{
		if(!GameManager.Instance.IsGameStart)
        {
			return;
        }

		LevelScore += value;


		LevelScore = Mathf.Clamp(LevelScore,0,100000000000000);

		// Log an event with a string parameter.
	
	}

	#endregion CustomFunction
}
