using UnityEngine;
using System.Collections;

public class Create_Mesh_Collider : MonoBehaviour {

    

	// Use this for initialization
	void Start () {
        

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters != null)
        {
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.active = false;
                i++;
            }
            //transform.GetComponent<MeshFilter>().mesh = new Mesh();
            //transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            //transform.gameObject.active = true;

            MeshCollider cullider = transform.GetComponent<MeshCollider>();
            if (cullider)
            {
                 if(combine != null)
                {
                    Mesh combineMesh = new Mesh();
                    combineMesh.CombineMeshes(combine);
                    Debug.Log(combineMesh.vertexCount);
                    cullider.sharedMesh = combineMesh;
                }
                
            }
                
        }
      
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
