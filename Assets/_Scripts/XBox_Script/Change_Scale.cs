using UnityEngine;
using System.Collections;

public class Change_Scale : MonoBehaviour {


    public float scaleSpeed;
    private Rigidbody rigidBody;

    void Start(){

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        scaleSpeed = Input.GetAxis("axis7th")/100;
      if(scaleSpeed != 0){
        rigidBody = Select_GameObject.selectedToTransform.GetComponent<Rigidbody>();
        rigidBody.transform.localScale += new Vector3(scaleSpeed, scaleSpeed, scaleSpeed);
      }


    }
}
