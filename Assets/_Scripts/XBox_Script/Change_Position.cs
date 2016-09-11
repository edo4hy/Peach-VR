using UnityEngine;
using System.Collections;

public class Change_Position : MonoBehaviour
{

    public float speed;

    private float moveHorizontal;
    private float moveVertical;

    private GameObject secondCamera;
    private KeyCode upDownKey;

    void Start(){
        secondCamera = GameObject.Find("SecondCamera");
        upDownKey = KeyCode.JoystickButton4;
    }

    //Position the GameObject - Passes in the Secondary camera which only rotates around the Y. So left,
    // right, forward and back are determined whilst up and down are fixed.
    // Applies transformation to Rigidbody based on button pressed
    void FixedUpdate()
    {
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();

        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");


      // Find the key which has been pressed
        // System.Array values = System.Enum.GetValues(typeof(KeyCode));
        // foreach(KeyCode code in values){
        //   if(Input.GetKeyDown(code)){ print(System.Enum.GetName(typeof(KeyCode), code)); }
        // }

        if(moveHorizontal < 0 && !Input.GetKey(upDownKey)){
          rigidBody.transform.position += (secondCamera.transform.right * -1)* Time.deltaTime * speed;
        }
        if(moveHorizontal > 0 && !Input.GetKey(upDownKey)){
            rigidBody.transform.position += secondCamera.transform.right * Time.deltaTime * speed;
        }
        if(moveVertical > 0  && !Input.GetKey(upDownKey)){
            rigidBody.transform.position += secondCamera.transform.forward * Time.deltaTime * speed;
        }
        if(moveVertical < 0  && !Input.GetKey(upDownKey)){
          rigidBody.transform.position += (secondCamera.transform.forward * -1)* Time.deltaTime * speed;
        }
        if (moveVertical > 0 && Input.GetKey(upDownKey)){
            rigidBody.transform.position += secondCamera.transform.up * Time.deltaTime * speed;
        }
        if(moveVertical < 0 && Input.GetKey(upDownKey)){
            rigidBody.transform.position += (secondCamera.transform.up * -1) * Time.deltaTime * speed;
        }
    }
}
