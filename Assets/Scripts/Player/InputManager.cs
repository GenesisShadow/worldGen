using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public bool Up;		//1
	public bool Down;	//2
	public bool Left;	//3
	public bool Right;	//4
	public bool Mine;	//5
	public bool Place;	//6
	public bool Jump;	//7
	public bool Run;	//8

	public bool OnUp;
	public bool OnDown;
	public bool OnLeft;
	public bool OnRight;
	public bool OnMine;
	public bool OnPlace;
	public bool OnJump;
	public bool OnRun;

	int lastlastKeyPressed;
	int lastKeyPressed;
	float timeSincePress;

	public float Xaxis(){
		float f = Left ? -1 : (Right ? 1 : 0);
		return f;
	}
	public float Yaxis(){
		float f = Up ? 1 : (Down ? -1 : 0);
		return f;
	}

	public bool dash(){
		return (lastlastKeyPressed == lastKeyPressed && (lastKeyPressed > 0 && lastKeyPressed < 5));
	}

	public float CameraX(){
		float x = Input.GetAxis("Mouse X");
		return x;
	}
	public float CameraY(){
		float x = Input.GetAxis("Mouse Y");
		return x;
	}

    void Start(){
        LockCursor();
    }

	public void LockCursor(){
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void UnlockCursor(){
		Cursor.lockState = CursorLockMode.None;
	}

    void Update(){
        Up = Input.GetKey(KeyCode.W);
		Down = Input.GetKey(KeyCode.S);
		Left = Input.GetKey(KeyCode.A);
		Right = Input.GetKey(KeyCode.D);
		Jump = Input.GetKey(KeyCode.Space);
		Run = Input.GetKey(KeyCode.LeftShift);
		Mine = Input.GetMouseButton(0);
		Place = Input.GetMouseButton(1);

		OnUp = Input.GetKeyDown(KeyCode.W);
		OnDown = Input.GetKeyDown(KeyCode.S);
		OnLeft = Input.GetKeyDown(KeyCode.A);
		OnRight = Input.GetKeyDown(KeyCode.D);
		OnJump = Input.GetKeyDown(KeyCode.Space);
		OnRun = Input.GetKeyDown(KeyCode.LeftShift);
		OnMine = Input.GetMouseButtonDown(0);
		OnPlace = Input.GetMouseButtonDown(1);

		if(OnLeft){
			lastKeyPressed = 3;
			timeSincePress = 0;
		}else if(OnRight){
			lastKeyPressed = 4;
			timeSincePress = 0;
		}else if(OnUp){
			lastKeyPressed = 1;
			timeSincePress = 0;
		}else if(OnDown){
			lastKeyPressed = 2;
			timeSincePress = 0;
		}else if(OnJump){
			lastKeyPressed = 7;
			timeSincePress = 0;
		}else if(OnRun){
			lastKeyPressed = 8;
			timeSincePress = 0;
		}else if(OnMine){
			lastKeyPressed = 5;
			timeSincePress = 0;
		}else if(OnPlace){
			lastKeyPressed = 6;
			timeSincePress = 0;
		}else{
			if(lastKeyPressed != 0)
				lastlastKeyPressed = lastKeyPressed;
			lastKeyPressed = 0;
			timeSincePress += Time.deltaTime;
		}
		if(timeSincePress > 0.2f)	lastlastKeyPressed = 0;
    }
}
