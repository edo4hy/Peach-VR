using UnityEngine;
using System.Collections;

public class Select_GameObject : MonoBehaviour {

	// Used in Rotate - Seperate Out - Fade_GameObject
	public static GameObject selectedToTransform;
	public static GameObject[] subParts;

	private GameObject selectedPart;

	private int selectedCount;

	private KeyCode leftTrigger;
	private KeyCode rightTrigger;
	private KeyCode xboxA;

	private GameObject 	glowingOrb;
	private Color currentColour;
	public Color purple;

	public GameObject fullKidney;

	// Use this for initialization
	void Start () {

		fullKidney = gameObject;

		// Save all the child gameObjects (subParts of kidney Model) in an array, +1 to add the whole kidney
		subParts = new GameObject[(transform.childCount) + 1];
		subParts[0] = gameObject;

		for(int i = 1; i < (transform.childCount) + 1; i++){
			subParts[i] = transform.GetChild(i-1).gameObject;
		}
		selectedCount = 0;
		// get the glowing orb for the colour selected
		glowingOrb = GameObject.Find("glowingOrb");

		// set the controls
		leftTrigger = KeyCode.JoystickButton4;
		rightTrigger = KeyCode.JoystickButton5;
		xboxA = KeyCode.JoystickButton0;
	}

	public float switchRate = 0.5F;
	private float nextSwitch = 0.5F;

	// Update is called once per frame
	void FixedUpdate () {

		// Move left and right through the child selections
		if(Input.GetKey(xboxA) && Input.GetKey(leftTrigger)  && Time.time > nextSwitch){
			
			selectedCount += 1;
			nextSwitch = Time.time + switchRate;
		}

		if(Input.GetKey(xboxA) && Input.GetKey(rightTrigger) && Time.time > nextSwitch){
			selectedCount -= 1;
			nextSwitch = Time.time + switchRate;
		}


		/// Loop selectedCount back around
		if(selectedCount > subParts.Length - 1){
			selectedCount = 0;
		}
		if(selectedCount < 0){
			selectedCount = subParts.Length - 1;
		}

		//Debug.Log(subParts[selectedCount]);
		selectedToTransform = subParts[selectedCount];

		// Call to updateColour which updates the colour
		updateColour(selectedCount, subParts);

		// Reset to be the whole Kidney moving at once
		if(Input.GetKey(leftTrigger) && Input.GetKey(rightTrigger)){
			selectedCount = 0;
		}
	}


// Recurisviely called to get the child of parent so mesh renderer can be drilled down to
	GameObject findRenderer(GameObject parent){
		parent = parent.transform.GetChild(0).gameObject;
		return parent;
	}

// Selects the colour for the orb at the bottom left of the screen
	void updateColour(int selectedCount, GameObject[] subParts){

		if(selectedToTransform != fullKidney){
			// Get the parent Renderer and GameObject
			Renderer componentRenderer = subParts[selectedCount].GetComponent<Renderer>();
			GameObject componentChild = subParts[selectedCount];

			// Drill down throught he compontent folders until we get to the basic mesh which contains the renderer
			while(componentRenderer == null){
				componentChild = findRenderer(componentChild);
				componentRenderer = componentChild.GetComponent<Renderer>();
			}

			// set the orbs color to match the componentes color
			glowingOrb.GetComponent<Light>().color = componentRenderer.material.color;
		}else{
			glowingOrb.GetComponent<Light>().color = Color.black;
		}
	}
}
