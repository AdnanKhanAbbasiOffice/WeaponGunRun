using UnityEngine;
using System.Collections;


public class STW_InputWalk : MonoBehaviour {

	public bool MobileJoystick = false;

	protected Animator animator;
	protected bool dirW = false;
	protected bool dirA = false;
	protected bool dirD = false;

	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	void OnGUI ()
	{
		if (MobileJoystick)
		{
			dirA = false;
			dirD = false;
			dirW = false;
			if (GUI.RepeatButton(new Rect(10+35,10+0,70,70),"W"))		
				dirW = true;
			if (GUI.RepeatButton(new Rect(10+0,10+70,70,70),"A"))		
				dirA = true;
			if (GUI.RepeatButton(new Rect(10+70,10+70,70,70),"D"))		
				dirD = true;
		}
	}
	
	void Update () 
	{
		if(animator)
		{
			float h = 0;
			float v = 0;
			if (dirA || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				h = -1;
			else if (dirD || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				h = 1;
			if (dirW || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
				v = 1;
			else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				v = -1;
			//set event parameters based on user input
			animator.SetFloat("Speed", v);
			transform.Rotate(0, 100 * Time.deltaTime * h,0);
		}		
	}   		  
}