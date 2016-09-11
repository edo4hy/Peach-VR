using UnityEngine;
using System.Collections;


// Reference script on how to set up the controller and the architecture of the page
// https://github.com/b0ard/YoutubeVive/commit/c81f35bbe3f7904bc4890c7c9713fafa1229f534
public class Interactable_Item : MonoBehaviour {

	private Rigidbody rigidbody;
	private bool currentlyInteracting;

	private Vive_Controller attached_viveController;

	private Transform interactionPoint;

	private float velocityFactor = 20000f;
	private Vector3 posDelta;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		interactionPoint = new GameObject().transform;
		velocityFactor /= rigidbody.mass; // heavier objects are harder to move
	}

    // Update is called once per frame
    void Update()
    {

        if (attached_viveController && currentlyInteracting)
        {
            // the wand position - the interaction point so the gameObject is held at the point it is grabbed.
            // this works out the difference between the hand and the gameObject - so that you then move towards the hand.
            posDelta = attached_viveController.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            //Debug.Log(attached_viveController.transform.position - interactionPoint.position + " :::  " + attached_viveController.transform.position);
            //this.rigidbody.position = attached_viveController.transform.position - interactionPoint.position;
        }
    }

	// Start the interaction - set the transform and rotaiton of the interaction point
	// This is called when collided and button is pressed
	public void BeginInteraction(Vive_Controller viveController){
        
            attached_viveController = viveController;
            interactionPoint.transform.position = viveController.transform.position;
            interactionPoint.SetParent(transform, true);

            currentlyInteracting = true;
        
	}

	// end interaction - called when the button is released
	public void EndInteraction(Vive_Controller viveController){

		if(viveController == attached_viveController){
			attached_viveController = null;
			currentlyInteracting = false;

            // ****** Added this to remove the child -- Might not be needed 
            interactionPoint.SetParent(transform, false);
        }
	}

	public bool IsInteracting()
    {
        return currentlyInteracting;
    }
}
