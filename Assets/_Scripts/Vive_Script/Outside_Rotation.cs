using UnityEngine;
using System.Collections;

public class Outside_Rotation : MonoBehaviour {


    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId dashBack = Valve.VR.EVRButtonId.k_EButton_Dashboard_Back;



    private SteamVR_TrackedObject trackedObj; 

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    
    Transform ControllerPosition;

    private GameObject selectedGameObject;

    private GameObject objectCamera;

    // public static so that when the second controller is used for the first time it does not reset. 
    public static bool startRotationSet;
    public static bool rotationNeedsReseting;
    private Quaternion currentRotation;

    public static int bothControllersPressed = 0;

    private bool isLeftController;
    private bool allowVibrate;
    private float startTime;


    // Get the other controller  
    private SteamVR_TrackedObject otherTrackedObj;
    private GameObject otherController;
    
    private SteamVR_Controller.Device controller2 { get { if (otherTrackedObj) return SteamVR_Controller.Input((int)otherTrackedObj.index); else return null; } }
    private bool secondControllerConnected;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        // Get the other controller - 
        if (trackedObj.name.Contains("left")){
            otherController = GameObject.Find("Controller (right)");
            isLeftController = true;
        }else
        {
            otherController = GameObject.Find("Controller (left)");
            isLeftController = false;
        }
        if (otherController)
        {
            otherTrackedObj = otherController.GetComponent<SteamVR_TrackedObject>();
            
        }

        startTime = Time.time + 0.5f;

    }

    // Update is called once per frame
    void Update()
    {

        if (controller == null)
        {
            Debug.Log("Controller not intialized");
            return;
        }

        // set the selected gameObject to equal that selected by Select_GameObject
        selectedGameObject = GameObject.Find("KidneyFull");

        if (controller2 == null)
        {
            secondControllerConnected = false;
        }
        else
        {
            secondControllerConnected = true;
        }

        // Vibrate at the start to identify the left controller 
        if (isLeftController)
        {
            if(Time.time < startTime)
            {
                controller.TriggerHapticPulse(1500, touchpad);
                controller.TriggerHapticPulse(1500, dashBack);
            }
        }

        // when both controllers are down set the public static variable to increase to 2 inhibiting rotation 
        if (controller.GetPressDown(triggerButton))
        {
            bothControllersPressed++;
        }

        // check not seperating out 
        if (!Vive_SOut.dontTransform)
        {
            // If trigger pressed - and the second controllers trigger is not pressed or it not connected 
            if (controller.GetPress(triggerButton) && (!secondControllerConnected || (secondControllerConnected && !controller2.GetPress(triggerButton))))
            {
                if (bothControllersPressed != 2)
                {
                    // Get the position of the controller 
                    Vector3 ControllerPos = this.transform.position;

                    // Get the position of the game object
                    if (selectedGameObject)
                    {

                        Vector3 GameObjectCentre = selectedGameObject.transform.position;

                        // Loop through the children of the gameObject to find the Camera - 
                        int numberOfchildren = selectedGameObject.transform.childCount;
                        for (int i = 0; i < numberOfchildren; i++)
                        {
                            if (selectedGameObject.transform.GetChild(i).GetComponent<Camera>() == true)
                            {
                                if (isLeftController)
                                {
                                    if (selectedGameObject.transform.GetChild(i).GetComponent<Camera>().name.Contains("Left"))
                                    {
                                        objectCamera = selectedGameObject.transform.GetChild(i).gameObject;
                                    }
                                }
                                else
                                {
                                    if (selectedGameObject.transform.GetChild(i).GetComponent<Camera>().name.Contains("Right"))
                                    {
                                        objectCamera = selectedGameObject.transform.GetChild(i).gameObject;
                                    }
                                }
                            }
                        }


                        if (objectCamera)
                        {
                            // difference betweent them as a vector 
                            Vector3 dif = GameObjectCentre - ControllerPos;

                            // convert vector into angle 
                            float angle = Vector3.Angle(dif, objectCamera.transform.forward);

                            // Rotate the gameObject 
                            if (!startRotationSet)
                            {
                                // first time set the current rotation to the angle to the controller
                                currentRotation = Quaternion.AngleAxis(angle, objectCamera.transform.forward);
                                startRotationSet = true;
                            }

                            if (rotationNeedsReseting)
                            {
                                // future setting use the currentRotation * the output from the controller position 
                                currentRotation = selectedGameObject.transform.rotation;
                                currentRotation = Quaternion.AngleAxis(angle, objectCamera.transform.forward) * currentRotation;
                                rotationNeedsReseting = false;
                            }

                            //Debug.Log(Quaternion.AngleAxis(angle, objectCamera.transform.forward)  + " : " + currentRotation + " : " + Quaternion.AngleAxis(angle, objectCamera.transform.forward) * currentRotation);

                            selectedGameObject.transform.rotation = Quaternion.AngleAxis(angle, objectCamera.transform.forward) * currentRotation;

                        }
                    }
                }
            }
            if (controller.GetPressUp(triggerButton))
            {
                // on drop trigger allows the rotation to be reset
                rotationNeedsReseting = true;
                bothControllersPressed--;

            }
        }
    }
}
