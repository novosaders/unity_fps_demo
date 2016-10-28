using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour {

	public CursorLockMode wantedMode;
	public Canvas canvas;
	public bool paused = false;
	public GameObject FPSController;


	// Use this for initialization
	void Start () {
		canvas = GetComponent<Canvas> ();
		canvas.enabled = false;
		wantedMode = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Pause ();
		}
	}

	public void Pause() {
		paused = !paused;
		if (paused) {
			canvas.enabled = true;
			Time.timeScale = 0;
			wantedMode = CursorLockMode.None;
			if (FPSController)
				FPSController.SetActive (false);
		} else {
			canvas.enabled = false;
			Time.timeScale = 1;
			wantedMode = CursorLockMode.Locked;
			if (FPSController)
				FPSController.SetActive (true);
		}
	}

	public void Quit(){
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	// Apply requested cursor state
	void SetCursorState ()
	{
		Cursor.lockState = wantedMode;
		// Hide cursor when locking
		Cursor.visible = (CursorLockMode.Locked != wantedMode);
	}
	void OnGUI ()
	{
		GUILayout.BeginVertical ();
		// Release cursor on escape keypress
		if (Input.GetKeyDown (KeyCode.Escape))
			Cursor.lockState = wantedMode = CursorLockMode.None;

		GUILayout.EndVertical ();

		SetCursorState ();
	}
}
