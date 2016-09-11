using UnityEngine;
using System.Collections;

public class Test_Camera_LookAtScript : MonoBehaviour {

    public GameObject controller;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(controller.transform);
	}
}
