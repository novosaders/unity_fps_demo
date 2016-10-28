using UnityEngine;
using System.Collections;

public class laserScript : MonoBehaviour {
	public Transform target;            // The position that that laser will be following.
	LineRenderer gunLine;

	// Use this for initialization
	void Start () {
		gunLine = GetComponent <LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
			gunLine.SetPosition (0, transform.position);
			gunLine.SetPosition (1, target.transform.position);
	}
}
