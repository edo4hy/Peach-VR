using UnityEngine;
using System.Collections;
using Valve.VR;


public class Select_Vive : MonoBehaviour {

    public static GameObject viveSelected;
    private GameObject fullKidney;
    public static GameObject[] viveSubParts;
    public static int viveSelectedCount; 

    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId axisPressed = Valve.VR.EVRButtonId.k_EButton_Axis4;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    public float switchRate = 0.5F;
    private float nextSwitch = 0.5F;

    private bool controllerIsLeft;

    private GameObject glowingOrb;

    // The second controller 
    private SteamVR_TrackedObject otherTrackedObj;
    private GameObject otherController;
    private SteamVR_Controller.Device controller2 { get { if (otherTrackedObj) return SteamVR_Controller.Input((int)otherTrackedObj.index); else return null; } }

    // Use this for initialization
    void Start () {

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        fullKidney = GameObject.Find("KidneyFull");
        glowingOrb = GameObject.Find("glowingOrb");
        
        // Create Array -- added 1 to allow fullKidney to be included - but because of the 
        // camera Gameobjects which is remove the 2 is also. 
        // then collider 1
        viveSubParts = new GameObject[(fullKidney.transform.childCount) -2 ];
        viveSubParts[0] = fullKidney;

        // start on 1 because of fullKidney add
        int arrayCount = 1;
        // add children to array
        for(int i = 1; i < (fullKidney.transform.childCount) + 1; i++)
        {
            // must skip out the camera Game object from array 
            GameObject currentChild = fullKidney.transform.GetChild(i - 1).gameObject;
            if(currentChild.GetComponent<Camera>() == null)
            {
                if(currentChild != GameObject.Find("Collider"))
                {
                    if (currentChild != null)
                    {
                        viveSubParts[arrayCount] = currentChild;
                        arrayCount++;
                    }
                }
            }
        }

        // set vive select count 
        viveSelectedCount = 0;

        // get the other controller by name depending of the name of this controller. 
        if (trackedObj.name.Contains("left"))
        {
            controllerIsLeft = true;
            otherController = GameObject.Find("Controller (right)");
        }else
        {
            otherController = GameObject.Find("Controller (left)");
        }
        // getting the tracke object off the other controller 
        if (otherController)
        {
            otherTrackedObj = otherController.GetComponent<SteamVR_TrackedObject>();
        }
    }


    // Update is called once per frame
    void Update () {
	    if(controller == null)
        {
            Debug.Log("Controller Not Connected");
            return;
        }

        
        // left move down the order 
        if (controller.GetPressDown(touchpad))
        {
           
            if(controller.GetAxis(touchpad).x > 0.2 && controller.GetAxis(touchpad).y < 0.6 && controller.GetAxis(touchpad).y > -0.6)
            {
                viveSelectedCount += 1;
                controller.TriggerHapticPulse(1500, touchpad);

            }
            if (controller.GetAxis(touchpad).x < 0.2 && controller.GetAxis(touchpad).y < 0.6 && controller.GetAxis(touchpad).y > -0.6)
            {
                viveSelectedCount -= 1;
                controller.TriggerHapticPulse(3000, touchpad);
                
            }

            nextSwitch = Time.time + switchRate;
        }

        // Loop round on the count 
        if (viveSelectedCount > viveSubParts.Length - 1)
        {
            viveSelectedCount = 0;
        }
        if (viveSelectedCount < 0)
        {
            viveSelectedCount = viveSubParts.Length - 1;
        }
       
        viveSelected = viveSubParts[viveSelectedCount];

        // Call to updateColour which updates the colour
        updateColour(viveSelectedCount, viveSubParts);

        // reset to the whole kidney 
        if (controller2 != null)
        {
            if(controller.GetPress(touchpad) && controller2.GetPress(touchpad))
            {
              viveSelectedCount = 0;
            }
        }
    }

    // Recurisviely called to get the child of parent so mesh renderer can be drilled down to
    GameObject findRenderer(GameObject parent)
    {
        if(parent.transform.childCount != 0 || parent.transform.childCount != null)
        {
            parent = parent.transform.GetChild(0).gameObject;
            return parent;
        }
        return null;
    }

    // Selects the colour for the orb at the bottom left of the screen
    void updateColour(int selectedCount, GameObject[] subParts)
    {

        if (viveSelected != fullKidney)
        {
            if(subParts[selectedCount] != null)
            {
            
                // Get the parent Renderer and GameObject
                Renderer componentRenderer = subParts[selectedCount].GetComponent<Renderer>();
                GameObject componentChild = subParts[selectedCount];

                // Drill down throught he compontent folders until we get to the basic mesh which contains the renderer
                while (componentRenderer == null)
                {
                    componentChild = findRenderer(componentChild);
                    componentRenderer = componentChild.GetComponent<Renderer>();
                }

                // set the orbs color to match the componentes color
                glowingOrb.GetComponent<Light>().color = componentRenderer.material.color;
            }
        }
        else
        {
            glowingOrb.GetComponent<Light>().color = Color.black;
        }
    }
}
