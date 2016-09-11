using UnityEngine;
using System.Collections;

public class Fade_GameObject3 : MonoBehaviour {

	private GameObject fullKidney;
	private float transCurrentLevel;
	private GameObject selectedSubPart;

	private KeyCode button_Y;
	private KeyCode rightTrigger;
	private KeyCode leftTrigger;

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
	void FixedUpdate () {

			selectedSubPart = Select_GameObject.selectedToTransform;
			if(Input.GetKey(button_Y) && Time.time > nextSwitch){
				findMeshes(selectedSubPart);
				nextSwitch = Time.time + switchRate;
			}

	}

	void findMeshes(GameObject currentGameObject){
		// check to see if the gameObject has a mesh Renderer - if it does then affect the change on it
		// if it doesn't then look to see if it has children
		// loop through the children calling this method -

		//Debug.Log(currentGameObject.GetComponent<Renderer>());
		// if the current Game Object has a Renderer then change the transpency through switch
		if(currentGameObject.GetComponent<Renderer>() != null){

			// get the current transparency
			transCurrentLevel = currentGameObject.GetComponent<Renderer>().material.color.a;
			// based on transCurrentLevel
			if(currentGameObject != null){
				if(transCurrentLevel == 1.0){
		
					fullKidney.GetComponent<Effect_Transparency>().makeTransparent(currentGameObject, 0.5f);

				}else if(transCurrentLevel == 0.5){
					fullKidney.GetComponent<Effect_Transparency>().makeTransparent(currentGameObject, 0.3f);
				}else if(transCurrentLevel == 0.3f){
					fullKidney.GetComponent<Effect_Transparency>().makeTransparent(currentGameObject, 0.1f);
				}else{

					fullKidney.GetComponent<Effect_Transparency>().makeOpaque(currentGameObject);
				}
			}

		}else{

			if(currentGameObject.transform.childCount != null){
				for(int i = 0; i < currentGameObject.transform.childCount; i++){

						if(currentGameObject.transform.GetChild(i).gameObject != null){

							findMeshes(currentGameObject.transform.GetChild(i).gameObject);
						}

					}
			}
			}
		}
	}
