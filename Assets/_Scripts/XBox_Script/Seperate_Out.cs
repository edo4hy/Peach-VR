using UnityEngine;
using System.Collections;

public class Seperate_Out : MonoBehaviour {

	// Axis of seprateion -
	private GameObject secondCamera;
	private Vector3 seperateOn;
	private bool axisSet;
	private float axis3th;

	// Conotrols
	private KeyCode leftTrigger;
	private KeyCode rightTrigger;
	public float speed;

	private GameObject fullKidney;
	private GameObject[] subParts;
	private Rigidbody[] subRigids;

	private float move_kidney;
	private float move_uretha;
	private float move_vein;
	private float move_artery;

	private Vector3 originalPosition;

	private float resetRate;
	private float nextChangeAfterReset;

	void Start () {

		// Initialize non sub part variables
		secondCamera = GameObject.Find("SecondCamera");
		fullKidney = gameObject;
		//rigid_fullKidney = fullKidney.GetComponent<Rigidbody>();

		// Set conotrols
		leftTrigger = KeyCode.JoystickButton4;
		rightTrigger = KeyCode.JoystickButton5;

		// Set variables for axis set control
		axisSet = false;
		resetRate = 0.5F;
		nextChangeAfterReset = 0.5F;

		subParts = Select_GameObject.subParts;

		subRigids = new Rigidbody[subParts.Length];

		for(int i = 0; i < subParts.Length; i++){
			subRigids[i] = subParts[i].GetComponent<Rigidbody>();
		}

	}


// get the current axis of the camera for the axis on which to sepearte out components on.
	void setSeperateAxis(){
		seperateOn = secondCamera.transform.right;
	}


	// Update is called once per frame
	void FixedUpdate () {
		axis3th = Input.GetAxis("axis3th");

		// If the 3rd axis is changed then call functio which defines the axis to seperaet on and the set it as defined
		if(axis3th != 0 && !axisSet ){
			setSeperateAxis();
			axisSet = true;
		}

		float movementDirection;
		// seperate out the different components of the game object
		// include time check so that user has time to release thier finger from the trigger before a new axis is set
		// needs to start at 1 because the whole kideny is in position 1
		if(axis3th != 0 && !Input.GetKey(leftTrigger) && !Input.GetKey(rightTrigger) && nextChangeAfterReset < Time.time){
			for(int i = 1; i < subRigids.Length; i++){
					Debug.Log(subRigids.Length);
					movementDirection = i - subRigids.Length / 2;
					subRigids[i].transform.position += seperateOn * Time.deltaTime * speed * axis3th * (movementDirection * 5);
			

					//Debug.Log(subRigids[i].name + " :: " + movementDirection + " :: " + seperateOn * Time.deltaTime * speed * axis3th * movementDirection);

			}
		}


		// Reset the seperation so the components come back together to the center of the game object
		// allow axis to be reset
		if(axis3th > 0 && Input.GetKey(rightTrigger)){
			originalPosition = fullKidney.transform.position;

			for(int i = 0; i < subRigids.Length; i++){
				subRigids[i].transform.position = originalPosition;
			}

			axisSet = false;
			nextChangeAfterReset = Time.time + resetRate;
		}

	}
}
