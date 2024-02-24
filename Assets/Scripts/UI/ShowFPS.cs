using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour
{
    public Text fpsText;
	public float deltaTime;
	public static float fps;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		fps = 1.0f / deltaTime;
		fpsText.text = "FPS : " + Mathf.Ceil(fps).ToString();
    }
}
