using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Vive_Ruler : MonoBehaviour {

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    public static bool dontTransformRul;


    // Get the controller which this script is called on 
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    private GameObject selectedGameObject;


    // Get the other controller  
    private SteamVR_TrackedObject otherTrackedObj;
    private GameObject otherController;
    private SteamVR_Controller.Device controller2 { get { if (otherTrackedObj) return SteamVR_Controller.Input((int)otherTrackedObj.index); else return null; } }

    // controll variables 
    private bool hasSeperated;
    float movementDirection;
    Vector3 orgianalPos;

    // Object variables 
    GameObject backBox;
    public GameObject unit1;
    public GameObject unit5;

  
    public GameObject rulerText;
    GameObject text;
    GameObject HMD;
    GameObject fullKidney;


    public int scaleFactor;
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
        backBox = GameObject.Find("backBox");
        rulerText = GameObject.Find("RulerText");
        HMD = GameObject.Find("Camera (eye)");
        fullKidney = GameObject.Find("KidneyFull");
    }
	
	// Update is called once per frame
	void Update () {

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

        // On Press down 
        if (controller.GetPress(gripButton) && controller2.GetPressDown(gripButton))
        {
            // Get the start positions of the controllers
            Vector3 con1Pos = this.transform.position;
            Vector3 con2Pos = otherController.transform.position;

           // Instantiate the text 
            text = (GameObject)Instantiate(rulerText, con1Pos, Quaternion.identity);

            // set parent and position of text
            text.transform.SetParent(otherController.transform);
            text.transform.position = con2Pos;

            // prevent movement of object
            dontTransformRul = true;
        }

        //On keep pressed
        if (controller.GetPress(gripButton) && controller2.GetPress(gripButton)) {

            if (text)
            {
                // Get controller positions and set offset 
                Vector3 con1Pos = this.transform.position;
                Vector3 con2Pos = otherController.transform.position;
                Vector3 startOffSet = new Vector3(0, 0.05f, 0);

                // set the text position
                text.transform.position = con2Pos + startOffSet;

                // difference between the two positions 
                float distance = Vector3.Distance(con1Pos, con2Pos);

                // adjust for scale factor 
                //distance *= scaleFactor;

                // convert to double and round -- convert to roughly the correct number - but is arbitary 
                distance = (float)Math.Round((double) distance / (fullKidney.transform.localScale.x * 2.5), 0) ;
                text.GetComponent<TextMesh>().text = distance.ToString() + "cm";

                if (HMD)
                {
                    // look at the head set 
                    text.transform.LookAt((2 * text.transform.position) - HMD.transform.position);
                }
            }
        }

        // Destory text on button up 
        if(controller.GetPressUp(gripButton) || controller2.GetPressUp(gripButton))
        {
            Destroy(text);
            dontTransformRul = false;
        }
    }
}
