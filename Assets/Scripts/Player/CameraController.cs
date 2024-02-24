using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public float XSensetivity = 360;
	public float YSensetivity = 270;
	public InputManager input;
	public GameObject player;
	float rotX;
	float rotY;

	Vector3 charRot;
	Vector3 headRot;

	public GameObject skyCam;

    // Update is called once per frame
	void Update()
	{
		rotX += input.CameraX() * Time.deltaTime * XSensetivity;
		if(rotX >= 360)
			rotX -= 360;
		rotY += input.CameraY()*-1 * Time.deltaTime * YSensetivity;
		rotY = Mathf.Clamp(rotY, -90f, 90f);

		headRot.x = rotY;
		headRot.z = 0;
		headRot.y = rotX;

		//charRot.y = rotX;

		this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(headRot), Time.deltaTime*20);

		player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(charRot), Time.deltaTime*20);

		skyCam.transform.rotation = Quaternion.Lerp(skyCam.transform.rotation, Quaternion.Euler(headRot + charRot), Time.deltaTime*20);
	}
}
