using UnityEngine;
using System.Collections;

public class Fade_GameObject : MonoBehaviour {

	private GameObject[] Parts;
	private Material material;

	private GameObject selectedSubPart;
	private GameObject fullKidney;

	private KeyCode button_Y;
	private KeyCode rightTrigger;
	private KeyCode leftTrigger;

	private float isTransparent;

	private float switchRate = 0.5F;
	private float nextSwitch = 0.5F;

	// Use this for initialization
	void Start () {
		fullKidney = gameObject;
		button_Y = KeyCode.JoystickButton3;
		rightTrigger = KeyCode.JoystickButton5;
		leftTrigger = KeyCode.JoystickButton4;
	}

	// Update is called once per frame
	void Update () {

		// Retrieve the selected GameObject stored within the static variable.
		selectedSubPart = Select_GameObject.selectedToTransform;
		Debug.Log(selectedSubPart);

		// get all the children of the GameObject to make opaque - have to access the consituent meshes as the mesh render is attached to them
		Parts = new GameObject[selectedSubPart.transform.childCount];
		for(int i = 0; i < selectedSubPart.transform.childCount; i++){
			Parts[i] = selectedSubPart.transform.GetChild(i).gameObject;
		}

	// only try and get sub parts if the full kidney is not selected
		if(selectedSubPart != fullKidney){
			isTransparent = Parts[0].GetComponent<Renderer>().material.color.a;

			// Call the makeTransparent function on the effectTransparency script which loop through all the sub parts and makes them transparent
			// the transparency is passed through based on the transparency before
			// calling the script which is attached to the full kidney game Object
			if(Input.GetKey(button_Y) && Time.time > nextSwitch){
				if(isTransparent == 1.0){
						fullKidney.GetComponent<Effect_Transparency>().makeTransparent(Parts, 0.5f);
				}else if(isTransparent == 0.5){
							fullKidney.GetComponent<Effect_Transparency>().makeTransparent(Parts, 0.3f);
				}else if(isTransparent == 0.3f){
							fullKidney.GetComponent<Effect_Transparency>().makeTransparent(Parts, 0.1f);
				}else{
					 	fullKidney.GetComponent<Effect_Transparency>().makeOpaque(Parts);
				}
					nextSwitch = Time.time + switchRate;
			}

/** -----------------  Reset ---------------------------------------------**/
			// Reset transparency of the gameObject which is currently selected -
			if(Input.GetKey(button_Y) && Input.GetKey(rightTrigger) && !Input.GetKey(leftTrigger)){
				fullKidney.GetComponent<Effect_Transparency>().makeOpaque(Parts);
			}
		}  // Note final reset outside the check not full kidney - so can reset on full kidney

		// Reset the transparency on all the game Obejcts
		if(Input.GetKey(button_Y) && Input.GetKey(rightTrigger) && Input.GetKey(leftTrigger)){
			for(int j = 0; j < fullKidney.transform.childCount; j++){
				// Get each of the child gameObjects to the fullkidney
				GameObject currentPart = fullKidney.transform.GetChild(j).gameObject;

				// Get allthe parts of that GameObject and store them in an GameObject array
				Parts = new GameObject[currentPart.transform.childCount];
				for(int i = 0; i < currentPart.transform.childCount; i++){
					Parts[i] = currentPart.transform.GetChild(i).gameObject;
				}

				// Apply makeOpaque passing the Parts gameObject array in
				fullKidney.GetComponent<Effect_Transparency>().makeOpaque(Parts);
			}
		}
	}

}
