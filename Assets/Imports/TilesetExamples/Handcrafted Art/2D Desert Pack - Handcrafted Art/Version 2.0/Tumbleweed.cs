using UnityEngine;
using System.Collections;

public class Tumbleweed : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0f, 0f, 1.5f));
	
	}
}
