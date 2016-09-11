using UnityEngine;
using System.Collections;
 using System.Linq;
 using System.Collections.Generic;

public class Model_Controller : MonoBehaviour {


  private GameObject medulla;
  private GameObject fullKidney;

  // make the kid a child of the med
  private GameObject kid;
  private GameObject med;


	// Use this for initialization
	void Start () {
    // get the parent class in which all the loaded meshes will be stored
    fullKidney = gameObject;

    // Use LoadAll to get all the meshes which are stored within the Kidney_bone-meshes folder
    GameObject[] instance = Resources.LoadAll("Kidney_bone_meshes", typeof(GameObject)).Cast<GameObject>().ToArray();


    // Loop through all the meshes instantiating them and adding them to the parent GameObject
    for(int i = 0; i < instance.Length; i++){
      GameObject tempGameObject = Instantiate(instance[i]);

      // Add the gameObject and remove the use of gravity
      //tempGameObject.AddComponent<Rigidbody>().useGravity = false;
      // Make the gameObject a child of a full kidney gameObject
      tempGameObject.transform.parent = fullKidney.transform;

      var meshName = tempGameObject.name;


      if(meshName.Contains("art")){

        loadMeshComponents("colour_art", tempGameObject);

      }else if(meshName.Contains("bon")){

        loadMeshComponents("colour_bone", tempGameObject);

      }else if(meshName.Contains("kid")){
        // set a variable so that kid can be made a parent of med
        kid = tempGameObject;
        loadMeshComponents("colour_kid", tempGameObject);

      }else if(meshName.Contains("med")){

        loadMeshComponents("colour_med", tempGameObject);
        // make kid a prent of med
        tempGameObject.transform.parent = kid.transform;
      }else if(meshName.Contains("tum")){

        loadMeshComponents("colour_tum", tempGameObject);

      }else if(meshName.Contains("uri")){

        loadMeshComponents("colour_uri", tempGameObject);

      }else if(meshName.Contains("ven")){

        loadMeshComponents("colour_ven", tempGameObject);

      }else{

      }
    }

        fullKidney.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

	}

  void loadMeshComponents(string colourName, GameObject tempGameObject){

    // Load the colour form the ColorStore gameObject
    GameObject componentColour = GameObject.Find(colourName);
    Material gMaterial =  componentColour.GetComponent<Renderer>().material;
    // pass in the current parent gameObject and change all children holdinng meshes to material
    changeMeshColour(tempGameObject, gMaterial);
  }

  // Change the colour of all children of the gameObject - take into account if there
  // is an inner folder
  void changeMeshColour(GameObject tempGameObject, Material gMaterial ){

    // variables to hold the number of children to control the loops
    int num_children;
    int num_children2;

    // Set variable to the number of children
    num_children = tempGameObject.transform.childCount;

    // if number of children equals 1 then it could be a folder so get the children of that gameObject
    if(num_children == 1){

      // get children of gameObject and store in second variable
      num_children2 = tempGameObject.transform.GetChild(0).gameObject.transform.childCount;

      // if greater than 0 then it is a folder
      if(num_children2 != 0){
        // set number to second number and set the gameObject to equal folder so that its children can be loop through
        num_children = num_children2;
        tempGameObject = tempGameObject.transform.GetChild(0).gameObject;
      }
    }

    // Loop through all the children of the selected parent and update the material by getting
    // the meshRenderer and setting material to the passed in material
    for(int j = 0; j < num_children; j++){
      tempGameObject.transform.GetChild(j).gameObject.GetComponent<Renderer>().material = gMaterial;
    }
  }


}
