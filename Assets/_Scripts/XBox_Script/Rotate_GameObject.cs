using UnityEngine;
using System.Collections;

public class Rotate_GameObject : MonoBehaviour {

// Degrees rotate
	private float aroundX;
	private float aroundY;
	private float aroundZ;

	private float axis4th;
	private float axis5th;

	private KeyCode altAxis;
	private Vector3 center;

	private GameObject secondCamera;
	public float speed;

	private GameObject currentGameObject;
	private Rigidbody currentRigidbody;

	private KeyCode leftTrigger;
	private KeyCode rightTrigger;
	private float axis3th;

	private GameObject fullModel;
	private GameObject[] subParts;

	void Start(){
		secondCamera = GameObject.Find("SecondCamera");
		altAxis = KeyCode.JoystickButton4;

		leftTrigger = KeyCode.JoystickButton4;
		rightTrigger = KeyCode.JoystickButton5;

		fullModel = gameObject;
	}

	// Update is called once per frame
	void FixedUpdate () {

		if(Select_GameObject.selectedToTransform != null){

			// update the current game object to equal the selectedToTransform game Object selected in the Select_GameObject script 
			currentGameObject = Select_GameObject.selectedToTransform;
			currentRigidbody = currentGameObject.GetComponent<Rigidbody>();

			// Map the game controls onto the degree rotation of each of the axis, add the axis to the degree
			axis4th = Input.GetAxis("axis4th");
			axis5th = Input.GetAxis("axis5th");

			axis3th = Input.GetAxis("axis3th");

			// update center so it rotate around itself
			center = currentRigidbody.transform.position;

			// Axis chnage links aroundY in degrees, center from gameObject and axis from camera so rotate on perspective
			if(!Input.GetKey(altAxis) && axis4th != 0){
				aroundY += axis4th;
				Vector3 axis1 = secondCamera.transform.up;
				currentRigidbody.transform.RotateAround(center, axis1, axis4th * Time.deltaTime * speed);
			}

			if(!Input.GetKey(altAxis) && axis5th != 0){
				aroundX += axis5th;
				Vector3 axis2 = secondCamera.transform.right;
				currentRigidbody.transform.RotateAround(center, axis2, axis5th * Time.deltaTime * speed);
			}

			if(Input.GetKey(altAxis) && axis5th != 0 ){
				aroundZ += axis5th;
				Vector3 axis3 = secondCamera.transform.forward;
				currentRigidbody.transform.RotateAround(center, axis3, axis5th * Time.deltaTime * speed);
			}

			if(Input.GetKey(leftTrigger) && Input.GetKey(rightTrigger) && axis3th != 0){
				// reset the game object that you have currently selected
				currentRigidbody.transform.rotation = new Quaternion(0, 0, 0, 0);

				// if the game object is the entire kidney then reset all the position of all the
				if(currentGameObject= fullModel){
					// loop through all the children and children and reset their rotations
					for(int i = 0; i < fullModel.transform.childCount; i++){
						 currentGameObject.transform.GetChild(i).gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
					}
				}
			}
		}
	}
}
