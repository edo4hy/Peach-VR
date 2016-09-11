using UnityEngine;
using System.Collections;

public class Vive_SOut : MonoBehaviour {

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    public static bool dontTransform;
    bool positionSet;

    // Get the controller which this script is called on 
    private SteamVR_TrackedObject trackedObj;
    private GameObject selectedGameObject;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    // Get the other controller  
    private SteamVR_TrackedObject otherTrackedObj;
    private GameObject otherController;
    private SteamVR_Controller.Device controller2 { get { if (otherTrackedObj) return SteamVR_Controller.Input((int)otherTrackedObj.index); else return null; } }

    // controll variables 
    private bool hasSeperated;
    float movementDirection;
    Vector3 orgianalPos;
    private float movementAmount;

    // Object variables 
    GameObject[] viveSubparts;
    GameObject fullKidney;
    GameObject HMD;

    // Use this for initialization
    void Start () {

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

        // initialise objects 
        viveSubparts = Select_Vive.viveSubParts;
        fullKidney = GameObject.Find("KidneyFull");
        HMD = GameObject.Find("Camera (eye)");
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not connected");
            return;
        }
        if (controller2 == null)
        {
            Debug.Log("Controller 2 not connected");
            return;
        }

        if (fullKidney == null)
        {
            Debug.Log("Kidney not found");

            return;
        }

        orgianalPos = fullKidney.transform.position;


        if (controller.GetPress(gripButton) && controller.GetPress(triggerButton))
        {
            dontTransform = true;

            // if has seperated to it fullest degree then set back to normal
            if (hasSeperated)
            {
                for (int i = 1; i < viveSubparts.Length; i++)
                {
                    movementDirection = i - viveSubparts.Length / 2;

                    if (controller.GetPressDown(triggerButton))
                    {
                        if (viveSubparts[i] != fullKidney)
                        {
                            viveSubparts[i].transform.position = orgianalPos;
                            hasSeperated = false;
                        }
                    }
                }
            }
            else
            {
                // on trigger down move to the next step 
                if (controller.GetPressDown(triggerButton))
                {

                    if (movementAmount < 1.5 && !positionSet)
                    {
                        movementAmount = 2f;
                        positionSet = true;
                    }
                    else if (movementAmount < 3 && !positionSet)
                    {
                        movementAmount = 3;
                        positionSet = true;
                    }else if(movementAmount < 5 && !positionSet)
                    {
                        movementAmount = 5;
                    }
                    else if(!positionSet)
                    {
                        movementAmount = 1;
                        hasSeperated = true;
                        positionSet = true;
                    }

                    stepParts(viveSubparts);
                }
            }
        }

        if (controller.GetPressUp(triggerButton))
        {
            
            dontTransform = false;
            positionSet = false;

        }
    }

    void stepParts(GameObject[] viveSubParts)
    {
        // start at 1 because 0 is always full kidney 

        for (int i = 1; i < viveSubparts.Length; i++)
        {
            movementDirection = i - viveSubparts.Length / 2;
            viveSubparts[i].transform.position += (HMD.GetComponent<Camera>().transform.right * Time.deltaTime * (movementDirection * movementAmount) * 0.5f);
        }
    }

}

