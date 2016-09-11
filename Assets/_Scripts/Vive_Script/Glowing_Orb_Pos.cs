using UnityEngine;
using System.Collections;


public class Glowing_Orb_Pos : MonoBehaviour {

    private GameObject cameraObject;
    private GameObject glowingOrb;

    public Vector3 screenPosition = new Vector3(0, 0, 20);


	// Use this for initialization
	void Start () {

        glowingOrb = gameObject;
        cameraObject = GameObject.Find("Camera (eye)");
        
        if(glowingOrb != null)
        {
            if (cameraObject)
            {

                //glowingOrb.transform.position = cameraObject.GetComponent<Camera>().ScreenToWorldPoint(screenPosition);
                //Vector3 screenPoint = cameraObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(-0.709f, -0.331f, 0.921f));
                //glowingOrb.transform.position = screenPoint;
            }
        }
	}

    // Update is called once per frame
    void Update()
    {
       
    }
}
