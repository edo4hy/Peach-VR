using UnityEngine;
using System.Collections;

public class Scale_Controllers : MonoBehaviour {


    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }


    Transform ControllerPosition;

    private GameObject selectedGameObject;



    // Get the other controller  
    private SteamVR_TrackedObject otherTrackedObj;
    private GameObject otherController;
    private SteamVR_Controller.Device controller2 { get { if (otherTrackedObj) return SteamVR_Controller.Input((int)otherTrackedObj.index); else return null; } }


    // Control distance 
    private bool startDistanceSet;
    private float startDistance;

    private Vector3 Controller1Pos;
    private Vector3 Controller2Pos;

    public float scaleFactor;

    public float startScale;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        // Get the other controller - 
        if (trackedObj.name.Contains("left"))
        {
            otherController = GameObject.Find("Controller (right)");
        }
        else
        {
            otherController = GameObject.Find("Controller (left)");
        }
        if (otherController)
        {
            otherTrackedObj = otherController.GetComponent<SteamVR_TrackedObject>();
        }

        selectedGameObject = GameObject.Find("KidneyFull");
        startScale = selectedGameObject.GetComponent<Rigidbody>().transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {

        if (controller == null || controller2 == null)
        {
            Debug.Log("Controller not intialized");
            return;
        }

        // set the selected gameObject to equal that selected by Select_GameObject
        //selectedGameObject = Select_GameObject.selectedToTransform;

        
        // If trigger pressed 
        if (controller.GetPress(triggerButton) && controller2.GetPress(triggerButton))
        {

           
             // if start distance is not set then set start distance 
            if (!startDistanceSet)
            {
                
                // Get the position of the controller 
                Controller1Pos = this.transform.position;
                Controller2Pos = otherController.transform.position;

                // set distance between controllers 
                startDistance = Vector3.Distance(Controller1Pos, Controller2Pos);
                startDistanceSet = true;
             

                
            }

            // if start distance is set then calculate difference and scale 
            if (startDistanceSet)
            {
               
                // Update the controller positions 
                Controller1Pos = this.transform.position;
                Controller2Pos = otherController.transform.position;


                // differenc between the start and the current 
                float currentDistance = Vector3.Distance(Controller1Pos, Controller2Pos);
                float difference = startDistance - currentDistance;

                // adjust by scale factor 
                difference = -(difference / scaleFactor);

                // prevent inverting 
                
                if (selectedGameObject.GetComponent<Rigidbody>().transform.localScale.x > 0.0005)
                {
                   

                    // change the localscale depending on if moved apart or closer 
                    selectedGameObject.GetComponent<Rigidbody>().transform.localScale += new Vector3(difference, difference, difference);
                }else if(selectedGameObject.GetComponent<Rigidbody>().transform.localScale.x < 0.0005 && difference > 0)
                {
                    selectedGameObject.GetComponent<Rigidbody>().transform.localScale += new Vector3(difference, difference, difference);

                }

            }

        }
        else if(controller.GetPressUp(triggerButton) || controller.GetPressUp(triggerButton))
        {
            
            startDistanceSet = false;
        }
    }
}
