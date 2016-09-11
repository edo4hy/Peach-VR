using UnityEngine;
using System.Collections;
using System;

public class Vive_Fade : MonoBehaviour
{


    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    private SteamVR_TrackedObject trackedObj;

    public float clickRate;

    private Vector2 startPosition;
    private double startAngle;


    private Vector2 currentPosition;
    private double currentAngle;

    private float transparency;

    private GameObject fullKidney;
    private GameObject selectedSubPart;


    // controls for scroll down 
    private float currentY;
    private float startY;

    private float scrollRate;


    private float currentYScale;
    private float startYScale;

    private float scaleMarker;


    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        clickRate = 50;
        transparency = 1;

        fullKidney = GameObject.Find("KidneyFull");

        scrollRate = 0.4f;
    }


    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("controller not connected");
            return;
        }

        if (fullKidney == null)
        {
            Debug.Log("Kidney has not been found");
            return;
        }


        // get the subpart which has been selected by the two controllers 
        selectedSubPart = Select_Vive.viveSelected;

        if(selectedSubPart != fullKidney)
        {

            // get the current transparency of the mesh 
            transparency = findMeshesT(selectedSubPart);

            //Debug.Log("vive selected " + selectedSubPart);
            // find the intial touch position 
            if (controller.GetTouchDown(touchpad))
            {
                startPosition = controller.GetAxis(touchpad);
                startPosition.Normalize();

                // convert to Y scale so runs from 0 (top) to 2 (bottom) 
                startY = startPosition.y;
                if (startY > 0)
                {
                    startYScale = 1 - startY;
                }
                else
                {
                    startYScale = 1 + Math.Abs(startY);

                }
            }

            // when scrolling with touch down 
            if (controller.GetTouch(touchpad))
            {
                currentPosition = controller.GetAxis(touchpad);
                currentPosition.Normalize();

                // convert to Y scale so runs from 0 (top) to 2 (bottom) 
                currentY = currentPosition.y;
                if (currentY > 0)
                {
                    currentYScale = 1 - currentY;
                }
                else
                {
                    currentYScale = 1 + Math.Abs(currentY);

                }

                // difference between the start position and the current 
                float dif = startYScale - currentYScale;

                // if difference is greater than scrollRate increase transparency 
                if (dif > scrollRate)
                {
                    controller.TriggerHapticPulse(1500, touchpad);
                    startYScale = currentYScale;

                    if (transparency < 0.95f)
                    {
                        transparency += 0.2f;
                    }
                }

                // if difference is less than scrollRate then decrease transparency 
                if (dif < -scrollRate)
                {
                    controller.TriggerHapticPulse(1500, touchpad);
                    startYScale = currentYScale;
                    if (transparency > 0.05f)
                    {
                        transparency -= 0.2f;
                    }
                }

                findMeshes(selectedSubPart, transparency);
            }
        }
    }

    void findMeshes(GameObject currentGameObject, float transparency)
    {
        // check to see if the gameObject has a mesh Renderer - if it does then affect the change on it
        // if it doesn't then look to see if it has children
        // loop through the children calling this method -

        //Debug.Log(currentGameObject.GetComponent<Renderer>());
        // if the current Game Object has a Renderer then change the transpency through switch
        if (currentGameObject.GetComponent<Renderer>() != null)
        {

            // based on transCurrentLevel
            if (currentGameObject != null)
            {
                if(transparency < 0.90)
                {
                    fullKidney.GetComponent<Effect_Transparency>().makeTransparent(currentGameObject, transparency);

                }else
                {
                    fullKidney.GetComponent<Effect_Transparency>().makeOpaque(currentGameObject);
                }
            }
            else
            {
                fullKidney.GetComponent<Effect_Transparency>().makeOpaque(currentGameObject);
            }
        }
        else
        {
            if (currentGameObject.transform.childCount != null)
            {
                for (int i = 0; i < currentGameObject.transform.childCount; i++)
                {

                    if (currentGameObject.transform.GetChild(i).gameObject != null)
                    {

                        findMeshes(currentGameObject.transform.GetChild(i).gameObject, transparency);
                    }
                }
            }
        }
    }


    // over load the find mesh method to return just the current transparncy of the game obejct which has been passed
    // in as a parameter 
    float findMeshesT(GameObject currentGameObject)
    {
        // used to recusively find the current transparency of the gameObject which is selected 

        //Debug.Log(currentGameObject.GetComponent<Renderer>());
        // if the current Game Object has a Renderer then change the transpency through switch
        if(currentGameObject != null)
        {
            if (currentGameObject.GetComponent<Renderer>() != null)
            {

                // based on transCurrentLevel
                if (currentGameObject != null)
                {
                    return currentGameObject.GetComponent<Renderer>().material.color.a;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                if (currentGameObject.transform.childCount != null)
                {

                    GameObject objectStore = null;
                    for (int i = 0; i < currentGameObject.transform.childCount; i++)
                    {
                        if (currentGameObject.transform.GetChild(i).gameObject != null)
                        {
                            objectStore = (currentGameObject.transform.GetChild(i).gameObject);
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    if (objectStore)
                    {
                        return findMeshesT(objectStore);
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 2;
                }
            }
        }
     return 2;
    }
}


/*

if (controller.GetTouch(touchpad))
{
    currentPosition = controller.GetAxis(touchpad);
    currentPosition.Normalize();

    currentY = currentPosition.y;
    if(currentY > 0)
    {
        currentYScale = 1 - currentY;
    }else
    {
        currentYScale = 1 + Math.Abs(currentY);

    }


    float currentTransparency = findMeshes(selectedSubPart);
    if (currentYScale < scaleMarker - scrollRate)
    {
        transparency = currentTransparency + 0.2f;
        controller.TriggerHapticPulse(1500, touchpad);
        scaleMarker -= 0.2f;
    }

    if (currentYScale > scaleMarker + scrollRate)
    {
        transparency = currentTransparency - 0.2f;
        controller.TriggerHapticPulse(1500, touchpad);
        scaleMarker += 0.2f;
    }
} */

/************** Circle ******************
if (controller.GetTouchDown(touchpad))
{
    startPosition = controller.GetAxis(touchpad);
    startPosition.Normalize();

    startAngle = Mathf.Acos(startPosition.x) * Mathf.Rad2Deg;
    if (startPosition.y < 0)
    {
        startAngle = 360 - startAngle;
    }
}



if (controller.GetTouch(touchpad))
{
currentPosition = controller.GetAxis(touchpad);
currentPosition.Normalize();


// work out the cos-1 and convert from Radians to degrees
currentAngle = Mathf.Acos(currentPosition.x) * Mathf.Rad2Deg;


// vibrate the touch pad 

// when at r1 180 convert to 360 
if (currentPosition.y < 0)
{
    currentAngle = 360 - currentAngle;
}



// Decrease transparency - CounterClockwise 
// check to see if the startAngle is close to 0 
if (startAngle > 360 - clickRate)
{
    // decrease when getting close to 360 
    double newClickAngle = startAngle + clickRate - 360;
    if (currentAngle > newClickAngle && currentAngle < newClickAngle + 20)
    {
        controller.TriggerHapticPulse(1500, touchpad);
        startAngle = newClickAngle;
        if (transparency > 0.15f)
        {
            transparency -= 0.1f;
            findMeshes(selectedSubPart, transparency);
        }
    }
}
else
{
    // Normal case decrease 
    if (currentAngle > startAngle + clickRate)
    {
        controller.TriggerHapticPulse(1500, touchpad);
        startAngle = startAngle + clickRate;

        if (transparency > 0.15f)
        {
            transparency -= 0.1f;
            findMeshes(selectedSubPart, transparency);
        }
    }
}

// Increase transparency - ClockWise 
if (startAngle < 0 + clickRate)
{
    // increasing when getting close to 0 
    double newClickAngle = startAngle - clickRate + 360;
    if (currentAngle < newClickAngle && currentAngle > newClickAngle - 20)
    {
        controller.TriggerHapticPulse(500, touchpad);
        startAngle = newClickAngle;
        if (transparency < 0.95)
        {
            transparency += 0.1f;
            findMeshes(selectedSubPart, transparency);
        }
    }
}
else
{
    // normal case increase
    if (currentAngle < startAngle - clickRate)
    {

        controller.TriggerHapticPulse(500, touchpad);
        startAngle = startAngle - clickRate;
        if (transparency < 0.95)
        {
            transparency += 0.1f;
            findMeshes(selectedSubPart, transparency);
        }
    }
}

//}
* /************** Circle ******************/
