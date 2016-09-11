using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Vive_Controller : MonoBehaviour {

    // Get reference to the controller buttons
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId dashBack = Valve.VR.EVRButtonId.k_EButton_Dashboard_Back;



    private SteamVR_TrackedObject trackedObj;

    // Get and intialize the controller - Could be in different order and so need to be done like this
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); }}

    // Here using a hash set to store all the object which the controller is currenlty overing object hoveirng over
    // although a little bit in efficient - only a small number will be used at a particular time there are only so
    // many game Objects which can be under the controller at once.
    HashSet<Interactable_Item> objectsHoveringOver = new HashSet<Interactable_Item>();

    // Used as a check to see if there is currently a gameObject collided
    private GameObject pickup;

    private Interactable_Item closestItem;
    private Interactable_Item interactingItem;

	// Use this for initialization
	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Update is called once per frame
	void Update () {

      if(controller == null){

        Debug.Log("Controller not initialized");
        return;
      }

        if (!Vive_Ruler.dontTransformRul)
        {
        
          if(controller.GetPressDown(gripButton) && pickup != null){
                // Need to add a check to makes sure that the gameObjects are not seperated
          
                float minDistance = float.MaxValue;
                float distance;
                // Find the gameObject which is closest to the controlelr
                foreach(Interactable_Item item in objectsHoveringOver){

                // could be negative and we only need relative difference so we square
                    distance = (item.transform.position - transform.position).sqrMagnitude;
                    if(distance < minDistance){
                        minDistance = distance;
                        closestItem = item;
                    }
                }
          
                interactingItem = closestItem;
                if(interactingItem){
                interactingItem.GetComponent<Rigidbody>().drag = 0f;
                // kill all other interactions
                if (interactingItem.IsInteracting()){
                    interactingItem.EndInteraction(this);
                }

                // call the BeginInteraction function on the Interactinig_Item script
                interactingItem.BeginInteraction(this);
              
                }
            }
        }else
        {
            // Stop the interaction if both the controllers are pressed 
            if (interactingItem)
            {
                interactingItem.GetComponent<Rigidbody>().drag = 50f;

                if (interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                }
            }
        }

        // stop the interaction if grip button is released 
        if (controller.GetPressUp(gripButton))
        {
            if (interactingItem)
            {
                interactingItem.GetComponent<Rigidbody>().drag = 50f;

                if (interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                }
            }  
        }
        
    }

    /* ------------------------ Triggers ---------------------------*/

    // Adds gameObject which collide with the controller into the hashSet
    private void OnTriggerEnter(Collider collider){


         // Get the interactable_Item of the collider -
        Interactable_Item colliderItem = collider.GetComponent<Interactable_Item>();
        pickup = collider.gameObject;
           
        if(colliderItem){
          objectsHoveringOver.Add(colliderItem);
        }


    }

    // Remove item from the hast set as it leaves the collider
    private void OnTriggerExit(Collider collider){

    Interactable_Item colliderItem = collider.GetComponent<Interactable_Item>();
    pickup = null;

    if(colliderItem){
      objectsHoveringOver.Remove(colliderItem);
    }
  }
                
}
