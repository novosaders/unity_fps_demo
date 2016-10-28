using UnityEngine;
using System.Collections;

public class FPSCameraController : MonoBehaviour {
	public float sensitivityX = 5F;
	public float sensitivityY = 5F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	public float range = 100F;
	public float minimumDelta = 10F;
	public Camera leftAye;
	public Camera rightAye;
	public Transform head;
	public float smoothing = 5f;        // The speed with which the camera will be following.

	public Transform target;            // The position that that camera will be following.
	Vector3 leftAyeInitialPosition;
	Vector3 rightAyeInitialPosition;
	Vector3 headInitialPosition;
	Vector3 leftAyeOffset;
	Vector3 rightAyeOffset;
	Quaternion originalRotation = new Quaternion(0,0,0,0);
	Ray lookRay = new Ray ();           // A ray from the gun end forwards.
	RaycastHit lookHit;
	LineRenderer gunLine;
	bool moveAim = false;


	Vector3 offset;                     // The initial offset from the target.


	// Use this for initialization
	void Start () {
		if (!target) {
			GameObject parent = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects () [0];
			GameObject aim = Instantiate (parent);
			target = aim.transform;
		}

		leftAye.transform.LookAt (target);
		rightAye.transform.LookAt (target);

		leftAyeOffset = leftAye.transform.localPosition;
		rightAyeOffset = rightAye.transform.localPosition;

		rightAyeInitialPosition = rightAye.transform.localPosition;
		leftAyeInitialPosition = leftAye.transform.localPosition;

		headInitialPosition = head.transform.localPosition;

		gunLine = GetComponent <LineRenderer> ();
		originalRotation = rightAye.transform.localRotation;
		lookRay.origin = rightAye.transform.position;
		lookRay.direction = Vector3.forward;
	}
	
	// Update is called once per frame
	void Update () {
		rightAyeOffset = leftAyeOffset = head.localPosition - headInitialPosition;

		leftAye.transform.localPosition = leftAyeInitialPosition + leftAyeOffset;
		rightAye.transform.localPosition = rightAyeInitialPosition + rightAyeOffset;

		turnAim ( getAngles () * Vector3.forward);
		leftAye.transform.LookAt (target);
		rightAye.transform.LookAt (target);
	}

	Quaternion getAngles()
	{
		// Read the mouse input axis
		float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
		float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;

		if (Mathf.Pow (deltaX, 2) + Mathf.Pow (deltaY, 2) <= minimumDelta) {
			moveAim = false;
			return originalRotation = rightAye.transform.rotation;
		} else {
			moveAim = true;
		}

		Quaternion xQuaternion = Quaternion.AngleAxis (deltaX, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis (deltaY, Vector3.left);

		return originalRotation = originalRotation * xQuaternion * yQuaternion;
	}

	void turnAim(Vector3 direction)
	{
		if (!moveAim)
			return;
		
		lookRay.origin = rightAye.transform.position;
		lookRay.direction = direction;

		if (Physics.Raycast (lookRay, out lookHit, range)) {
			target.transform.position = lookHit.point;
		} else {
			target.transform.position = lookRay.direction * range;
		}
	}

	void turnCamera(Quaternion rotationAngles)
	{
		leftAye.transform.rotation = rotationAngles;
		rightAye.transform.rotation = rotationAngles;
	}

	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}
