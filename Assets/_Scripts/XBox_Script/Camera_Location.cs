using UnityEngine;
using System.Collections;

public class Camera_Location : MonoBehaviour {

    // Use this for initialization

    private GameObject mainCamera;
    private GameObject secondaryCamera;

    private Rigidbody secondRigid;


    void Start () {
        mainCamera = GameObject.Find("Main Camera");
        secondaryCamera = GameObject.Find("SecondCamera");
        secondRigid = secondaryCamera.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    // Set the secondary cameras position to equal the main cameras x & z cameras position but stay fixed on 0 y
    // secondays camera matches y rotation but not x or z rotation. This allows for transformation and rotation to
    // occur with repsect to camera, but along the true axis.
    void Update()
    {
        Vector3 mainCameraPosition = mainCamera.transform.position;
        Quaternion mainCameraRotation = mainCamera.transform.rotation;

        secondRigid.transform.position = new Vector3(mainCameraPosition.x, 0, mainCameraPosition.z);
        secondRigid.transform.rotation = Quaternion.Euler(0, mainCameraRotation.eulerAngles.y, 0);
    }
}
