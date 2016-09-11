using UnityEngine;
using System.Collections;

public class Vive_Select_GameObject : MonoBehaviour {

    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller {  get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {

        if(controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if (controller.GetTouchDown(touchPad))
        {
             Vector2 touch_Out = controller.GetAxis(touchPad);
        }

	
	}
}
