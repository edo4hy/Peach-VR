using UnityEngine;
using System.Collections;
using System;

public class Mesh_Shrink : MonoBehaviour {

    GameObject GShrink;
    GameObject testCube;
    GameObject meshBoxContainer;
    GameObject meshOutput;

    GameObject fullKidney;

    Mesh meshBox;
    Vector3[] meshBoxV;

    int boxSize = 5;

    // Face arrays 
    int[] topFace;
    int[] bottomFace;
    int[] leftFace;
    int[] rightFace;
    int[] frontFace;
    int[] backFace;

    Vector3[] furthestEnds;

    // Use this for initialization
    void Start () {

        fullKidney = GameObject.Find("KidneyFull");
        meshBoxContainer = new GameObject();
        meshBoxContainer.AddComponent<MeshFilter>();

        meshOutput = new GameObject();
        meshOutput.AddComponent<MeshFilter>();
        meshOutput.AddComponent<MeshCollider>();


        if (fullKidney == null)
        {
            Debug.Log("Full Kidney has not been found ");
            return;
        }

        if(meshOutput == null)
        {
            Debug.Log("Mesh output has not been found");
            return;
        }

        // create faces for mesh box - this stores the indexes of the vertices whcih make up an face 
        // in a array - Only needs to be called once. 
        createFaceArrays();


        // loop through all children of the fullKidney to find mesh Renderers 
        MeshFilter[] meshFilters = fullKidney.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        foreach (MeshFilter m in meshFilters)
        {
            if (m != null)
            {
                try
                {
                    // Get the gameObject for those which have MeshFilters 
                    GameObject subPart = m.gameObject;

                    // Simply the mesh 
                    meshOutput.GetComponent<MeshFilter>().mesh = simplifyMesh(subPart);

                    // add to the Combine array 
                    combine[i].mesh = meshOutput.GetComponent<MeshFilter>().mesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    i++;
                    
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Index is out of range" + e);
                }
                catch (Exception e)
                {
                    Debug.Log("Exception thrown" + e);
                }
            }
        }

        // Set the mesh to the output. 
        meshOutput.GetComponent<MeshFilter>().mesh = new Mesh();
        meshOutput.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        meshOutput.transform.position = new Vector3(0, 0, 0);

        Mesh meshToCollider = meshOutput.GetComponent<MeshFilter>().mesh;

        GameObject Collider = GameObject.Find("Collider");
        Collider.GetComponent<MeshCollider>().sharedMesh = meshOutput.GetComponent<MeshFilter>().mesh;
        Collider.transform.localScale = new Vector3(120, 120, 120);
        Collider.transform.position = new Vector3(0.15f, 0, 0);


    }


    // Create the box of vertices which will are adjusted to the size of the gameObject and then shrunk around it 
   public Mesh SetUpMeshBox(GameObject toShrink, Vector3[] furthestEnds, GameObject meshBoxContainer)
    {

        if (toShrink == null)
        {
            Debug.Log("GameObject passed into SetUpMeshBox is null");
            return null;
        }

        if (furthestEnds == null)
        {
            Debug.Log("Vector array passed into SetUpMeshBox is null");
            return null;
        }

        if (meshBoxContainer == null)
        {
            Debug.Log("The Output object passed into SetUpMeshBox is null");
            return null;
        }

        // Set any null values to 0 
        for (int i = 0; i < furthestEnds.Length; i++)
        {
            if (furthestEnds[i] == null)
            {
                furthestEnds[i] = Vector3.zero;
            }
        }

        // Calculate the range for each of the axis. 
        float xRange = 0;
        float yRange = 0;
        float zRange = 0;

        if (furthestEnds.Length != 0)
        {
            xRange = furthestEnds[0].x - furthestEnds[1].x;
            yRange = furthestEnds[2].y - furthestEnds[3].y;
            zRange = furthestEnds[4].z - furthestEnds[5].z;
        }

        // Calculate the step amount 
        float xStep = xRange / boxSize;
        float  yStep = yRange / boxSize;
        float  zStep = zRange / boxSize;


        // number of verticies - boxsize cubed. - No overlap on edges 
        int numberOfVertices = ((boxSize * boxSize) + ((boxSize - 2) * boxSize) + ((boxSize - 2) * (boxSize - 2))) * 2;

        int size2D = boxSize * boxSize;

        // the new vertices of the mesh are stored here 
        Vector3[] meshBoxV = new Vector3[numberOfVertices];

        float halfMeshBox = meshBoxV.Length / 2;

        // Set the start point for the Cube - Start on the negative side of each axis - Bottom Back left 
        float sfX = furthestEnds[1].x;
        float sfY = furthestEnds[3].y;
        float sfZ = furthestEnds[5].z;


        int vertexInCount = 0;

        /*
        * Order the sides of the cube are added to the meshBox array 
        * Top > Bottom > Left > Right > Front > Back 
        * Top VC = 0 ->> (boxSize * boxSize ) - 1 ~~~~~~~~  (4) 0 -- 15
        * Bottom VC = (boxSize * boxSize ) --> (boxSize * boxSize ) + ((boxSize * boxSize ) - 1)  ~~~~~~~  (4) 16 -- 31
        */

        // create the top of the mesh verticies and store them in the meshBox array. 
        // loops through the x and z
        for (int x = 0; x < boxSize; x++)
        {
            for (int z = 0; z < boxSize; z++)
            {
                // top mesh 
                meshBoxV[vertexInCount] = new Vector3(sfX, furthestEnds[2].y, sfZ);

                // bottom mesh - 
                meshBoxV[vertexInCount + size2D] = new Vector3(sfX, furthestEnds[3].y + yStep, sfZ);
                vertexInCount++;
                sfZ += zStep;
            }
            // reset the inner loop. 
            sfZ = furthestEnds[5].z;
            sfX += xStep;
        }


        /* ~~~~~~~~~~~~~~~~~~~~~~~ Set Vertex Right Left  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        // set the vertexCount to the first empty space. 
        vertexInCount = vertexInCount + size2D;

        // reset the 
        sfY = furthestEnds[2].y - yStep;
        sfZ = furthestEnds[5].z;

        // calculate adjustion factor as top and bottom lines already filled it is 2 line less. 
        int adjustionFactor = (boxSize * boxSize) - (boxSize * 2);


        // Add the left right vertices to the Mesh 
        // Left =   (4) 32 --> 39  - add boxSize * boxSize - (boxSize * 2) for top and bottom subtraction 
        // Right =  (4) 40 --> 47 
        // we dont need to add the top or the bottom vertices as these have been added by the top mesh creation 
        // so Y runs from 1 to boxSize -1
        for (int y = 1; y < boxSize - 1; y++)
        {
            for (int z = 0; z < boxSize; z++)
            {
                // left
                meshBoxV[vertexInCount] = new Vector3(furthestEnds[1].x , sfY, sfZ);
                //right
                meshBoxV[vertexInCount + adjustionFactor] = new Vector3(furthestEnds[0].x - xStep, sfY, sfZ);
                vertexInCount++;
                sfZ += zStep;
            }
            // reset the inner loop 
            sfZ = furthestEnds[5].z;
            sfY -= yStep;

        }

        /* ~~~~~~~~~~~~~~~~~~~~~~~ Set Vertex Front Back  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        // set the vertexCount to the first empty space. 
        vertexInCount = vertexInCount + adjustionFactor;

        // reset the 
        sfY = furthestEnds[2].y - yStep;
        sfX = furthestEnds[0].x - xStep - xStep;

        // calculate adjustion factor as top and bottom lines already filled it is 2 line less. 
        int adjustionFactorFB = (boxSize * boxSize) - (boxSize * 2) - ((boxSize - 2) * 2);


        // Add the front back vertices to the Mesh 
        // excludes top bottom left and right 
        for (int y = 1; y < boxSize - 1; y++)
        {
            for (int x = 1; x < boxSize - 1; x++)
            {
                // front 
                meshBoxV[vertexInCount] = new Vector3(sfX, sfY, furthestEnds[5].z);
                // back 
                meshBoxV[vertexInCount + adjustionFactorFB] = new Vector3(sfX, sfY, furthestEnds[4].z - zStep);
                vertexInCount++;
                sfX -= xStep;
            }
            // reset the inner loop  
            sfX = furthestEnds[0].x - xStep - xStep;
            sfY -= yStep;
        }


        // Create new mesh and set the vertices to the mesh
        Mesh meshBox = new Mesh();
        meshBox.vertices = meshBoxV;

        ////// generate cubes to show mesh verticies working -- Debugging 
        //for (int i = 0; i < meshBox.vertices.Length; i++)
        //{

        //    var cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube1.transform.position = meshBoxContainer.transform.position;
        //    cube1.transform.position += meshBox.vertices[i];
        //    cube1.transform.localScale = new Vector3(1f, 1f, 1f);

        //    cube1.transform.parent = meshBoxContainer.transform;

        //    // Debug the cubes with color 
        //    //if (i == 2)
        //    //{
        //    //    cube1.GetComponent<MeshRenderer>().material.color = Color.blue;
        //    //}
        //    //else if (i == 0)
        //    //{
        //    //    cube1.GetComponent<MeshRenderer>().material.color = Color.red;
        //    //}
        //    //else if (i == 11 || i==10)
        //    //{
        //    //    cube1.GetComponent<MeshRenderer>().material.color = Color.green;
        //    //}
        //}

        return meshBox;
    }


    // Set up the array of faces which tringles are construted between 
   public void createFaceArrays()
    {

        try
        {
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /** Create the face arrays    */
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            // Variables 
            int boxSize_2 = boxSize * boxSize;
            int boxSize_2_sub = boxSize_2 - boxSize;

            topFace = new int[boxSize_2];
            bottomFace = new int[boxSize_2];
            leftFace = new int[boxSize_2];
            rightFace = new int[boxSize_2];
            frontFace = new int[boxSize_2];
            backFace = new int[boxSize_2];

            // Top face ******************************
            for (int i = 0; i < boxSize_2; i++)
            {
                topFace[i] = i;
            }

            // Bottom face ******************************
            int bfC = 0;
            int botC = boxSize_2 + boxSize - 1;
            int lineStartB = botC;

            for (int i = 0; i < boxSize; i++)
            {
                botC = lineStartB;
                for (int j = 0; j < boxSize; j++)
                {

                    bottomFace[bfC++] = botC--;
                }
                lineStartB += boxSize;
            }
            ////////////////////////////////////////////////////////
            //// Left face ******************************

            //// start line down on the left side 
            int loopStart = boxSize_2 * 2;
            int loopCount = loopStart;

            int llC = 0;
            int topCount = boxSize - 1;
            int bottomCount = 0;
            int midCountLeft = (boxSize_2 * 2) + boxSize * (boxSize - 2) - 1;
            int midCountInner;


            for (int i = 0; i < boxSize; i++)
            {

                // Debug.Log(bottomFace[bottomCount]);
                //Debug.Log(bottomFace[bottomCount]);
                leftFace[llC++] = bottomFace[bottomCount++];

                //Debug.Log(leftFace[llC]);
                midCountInner = midCountLeft;
                for (int j = 0; j < boxSize - 2; j++)
                {
                    leftFace[llC++] = midCountInner;
                    midCountInner -= boxSize;

                }
                midCountLeft--;

                leftFace[llC++] = topFace[topCount--];
            }

            ////////////////////////////////////////////////////////
            //// Right face ******************************

            //// start line down on the left side 
            int rrC = 0;
            int topCountR = boxSize_2 - boxSize;
            int bottomCountR = boxSize_2 - 1;
            int midCountR = (boxSize_2 * 2) + boxSize * (boxSize - 2) + boxSize * (boxSize - 2) - boxSize;
            int midCountInnerR;

            for (int i = 0; i < boxSize; i++)
            {

                rightFace[rrC++] = bottomFace[bottomCountR--];
                // Debug.Log(leftFace[llC]);
                midCountInnerR = midCountR;
                for (int j = 0; j < boxSize - 2; j++)
                {
                    rightFace[rrC++] = midCountInnerR;
                    midCountInnerR -= boxSize;
                }
                midCountR++;
                rightFace[rrC++] = topFace[topCountR++];
            }


            //////////////////////////////////////////////////////////
            // Front Face //////////////////////////////////////////
            int ffC = 0;
            int topCountF = 0;
            int bottomCountF = boxSize_2 + boxSize;
            int midCountF = (boxSize_2 * 2) + ((boxSize_2 - (boxSize * 2)) * 2) + ((boxSize - 2) * (boxSize - 2)) - (boxSize - 2);
            int midCountInnerF;


            for (int i = 0; i < boxSize; i++)
            {
                // Add the left edge 
                if (i == 0)
                {

                    for (int j = boxSize_2 - boxSize; j < boxSize_2; j++)
                    {
                        frontFace[ffC++] = leftFace[j];

                    }
                    // Add the right edge   
                }
                else if (i == boxSize - 1)
                {
                    for (int j = 0; j < boxSize; j++)
                    {
                        frontFace[ffC++] = rightFace[j];
                    }
                }
                else
                {
                    // middle case 
                    frontFace[ffC++] = bottomCountF;
                    bottomCountF += boxSize;

                    midCountInnerF = midCountF;
                    for (int j = 0; j < boxSize - 2; j++)
                    {
                        frontFace[ffC++] = midCountInnerF;
                        midCountInnerF -= (boxSize - 2);
                    }

                    midCountF++;

                    frontFace[ffC++] = bottomCount;
                    bottomCount += boxSize;
                }
            }

            ////////////////////////////////////////////////////
            /// Back Face ////////////////////////////////////////
            /// 

            // Front Face 
            int bckFC = 0;
            int topCountB = boxSize_2 - 1 - boxSize;
            int bottomCountB = boxSize_2 + boxSize_2 - boxSize - 1;
            int midCountB = (boxSize_2 * 2) + ((boxSize_2 - (boxSize * 2)) * 2) + ((boxSize - 2) * (boxSize - 2)) + ((boxSize - 2) * (boxSize - 2)) - 1;
            int midCountInnerB;

            for (int i = 0; i < boxSize; i++)
            {
                // Add the left edge 
                if (i == 0)
                {

                    for (int j = boxSize_2 - boxSize; j < boxSize_2; j++)
                    {
                        backFace[bckFC++] = rightFace[j];

                    }
                    // Add the right edge   
                }
                else if (i == boxSize - 1)
                {
                    for (int j = 0; j < boxSize; j++)
                    {
                        backFace[bckFC++] = leftFace[j];
                    }
                }
                else
                {
                    // middle case 

                    backFace[bckFC++] = bottomCountB;
                    bottomCountB -= boxSize;

                    midCountInnerB = midCountB;
                    for (int j = 0; j < boxSize - 2; j++)
                    {
                        backFace[bckFC++] = midCountInnerB;
                        midCountInnerB -= (boxSize - 2);
                    }

                    midCountB--;

                    backFace[bckFC++] = topFace[topCountB];
                    topCountB -= boxSize;
                }
            }

                // Test Faces

                //for (int i = 0; i < backFace.Length; i += 4)
                //{
                //    Debug.Log( " : " + backFace[i] + " : " + backFace[i + 1] + " : " + backFace[i + 2] + ": " + backFace[i+3]);
                //    //Debug.Log(" : " + leftFace[i] + " : " + leftFace[i + 1] + " : " + leftFace[i + 2] + ": " + leftFace[i + 3]);
                //}
        }catch(IndexOutOfRangeException e)
        {
            Debug.Log("Index is out of range" + e);
        }
        catch (Exception e)
        {
            Debug.Log("Exception thrown" + e);
        }
    }


   // Create the traingle between the vertices in a face 
   public int triangleFace(int boxSize_2_sub, int boxSize_2, int[] triangleStore, int tStoreCount, int[] face)
    {
        int currentArrayIndex = 0;
        int lineShift = 0;

        // add the forward triangles on the face 

        for (int i = 0; i < boxSize_2_sub; i++)
        {

            if (currentArrayIndex == boxSize - 1 + lineShift)
            {
                //Debug.Log(currentArrayIndex);
                currentArrayIndex++;
                lineShift += boxSize;
            }
            else
            {
                // Check the count is within range of the array 
                try
                {
                    // Add the current Vertext - 1 + current vertext + 1 to the right of current vertext
                    triangleStore[tStoreCount++] = face[currentArrayIndex];
                    triangleStore[tStoreCount++] = face[currentArrayIndex + 1];
                    triangleStore[tStoreCount++] = face[currentArrayIndex + boxSize];
                    currentArrayIndex++;
                    //tStoreCount += 3;

                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Index out of range" + e);

                }
                catch (Exception e)
                {
                    Debug.Log("Exception thrown" + e);
                }
            }
        }


        // add the backwards triangles on the face. 
        int currentBackArrayIndex = boxSize_2 - 1;
        int lineShiftBack = 0;

        for (int i = boxSize_2; i > boxSize; i--)
        {
            if (currentBackArrayIndex == boxSize_2_sub - lineShiftBack)
            {
                currentBackArrayIndex--;
                lineShiftBack += boxSize;
            }
            else
            {
                try
                {
                    // Add current, minus 1 - one to the left. 
                    triangleStore[tStoreCount++] = face[currentBackArrayIndex];
                    triangleStore[tStoreCount++] = face[currentBackArrayIndex - 1];
                    triangleStore[tStoreCount++] = face[currentBackArrayIndex - boxSize];
                    currentBackArrayIndex--;
                    //tStoreCount += 3;
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Index is out of range" + e);
                }
                catch (Exception e)
                {
                    Debug.Log("Exception thrown" + e);
                }
            }
        }

        return tStoreCount;
    }

    public Mesh shrinkMesh(GameObject shrinkObj, GameObject meshOutput, int boxSize, Vector3[] furthestEnds, Mesh meshBox)
    {

        // Check to see if the gameObject exist and that they have meshFilters attached 
        if (shrinkObj == null)
        {
            Debug.Log("Game Object to be shrunk passed into ShrinkMesh is null");
            return null;
        }

        if (meshOutput == null)
        {
            Debug.Log("The meshOutput passed into shrinkMesh is null");
            return null;
        }

        if (meshOutput.GetComponent<MeshFilter>() == null)
        {
            Debug.Log("The output gameObject does not have a meshFilter attached");
            return null;
        }

        if (shrinkObj.GetComponent<MeshFilter>() == null)
        {
            Debug.Log("The input gameObject does not have a meshFitler attached");
            return null;
        }

        if (furthestEnds == null)
        {
            Debug.Log("The furthestEnds array is null");
            return null;
        }

        if(meshBox == null)
        {
            Debug.Log("Mesh box is null");
            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////

        Vector3[] newVertSet = new Vector3[meshBox.vertexCount];

        try
        {
            // store all to shrink verts in array
            Vector3[] shrinkObjVerts = shrinkObj.GetComponent<MeshFilter>().mesh.vertices;

            int iCount = 0;
            //Debug.Log(shrinkObjVerts.Length);
            for (int i = 0; i < meshBox.vertexCount; i++)
            {
                // get the vector3 of the meshBox vert
                Vector3 currentVert = meshBox.vertices[i];

                float closestDistance = 1000;

                // intialized to the mesh values 
                Vector3 selectedVert = meshBox.vertices[i];

                Vector3 shrinkObjCurVert = new Vector3(0, 0, 0);

                // loop through all of the verticie in the shrink object 
                for (int j = 0; j < shrinkObjVerts.Length; j++)
                {

                    // get the objects vertex and normalize it 
                    shrinkObjCurVert = shrinkObjVerts[j];


                    // if distance from mesh vertex is less then set that as the closest vertex 
                    if ((Vector3.Distance(shrinkObjCurVert, currentVert)) < closestDistance)
                    {

                        closestDistance = Vector3.Distance(shrinkObjCurVert, currentVert);
                        selectedVert = shrinkObjVerts[j];
                    }
                }

                // set the vertices to the meshBox. 
                newVertSet[i] = selectedVert;
                meshBox.vertices[i] = selectedVert;
                // Debug.Log(newVertSet[i]);
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log("Index is out of range" + e);
        }
        catch (Exception e)
        {
            Debug.Log("Exception thrown" + e);
        }


        // set the created mesh to equal the rotation, position and scale of the object which has been shrunk wrapped. 
        meshBox.vertices = newVertSet;
        meshOutput.GetComponent<MeshFilter>().mesh = meshBox;
        meshOutput.transform.position = shrinkObj.transform.position;
        meshOutput.transform.rotation = shrinkObj.transform.rotation;
        meshOutput.transform.localScale = shrinkObj.transform.localScale;

        return meshOutput.GetComponent<MeshFilter>().mesh;
    }


    public Vector3[] findFurthestEnds(GameObject toShrink)
    {
        if (toShrink == null)
        {
            Debug.Log("Object passed into findFurthestEnds is null");
            return null;
        }

        if (toShrink.GetComponent<MeshFilter>() == null)
        {
            Debug.Log("Object passed inot findFurthestEdns does not have a meshFilter attached");
            return null;
        }

        // 6 for each face of the cube 
        Vector3[] furthestEnds = new Vector3[6];

        Vector3[] toShrinkVert = toShrink.GetComponent<MeshFilter>().mesh.vertices;

        Vector3 currentVert;

        try
        {
            // Loop through all vertices and find the furhtest
            //   0  1   2   3   4   5
            //   X+ X-  Y+  Y-  Z+  Z-
            for (int i = 0; i < furthestEnds.Length; i++)
            {
                for (int j = 0; j < toShrinkVert.Length; j++)
                {
                    currentVert = toShrinkVert[j];

                    // X --------------------------
                    if (currentVert.x > furthestEnds[0].x)
                    {
                        furthestEnds[0] = toShrinkVert[j];
                    }
                    if (currentVert.x < furthestEnds[1].x)
                    {
                        furthestEnds[1] = toShrinkVert[j];
                    }
                    // Y --------------------------
                    if (currentVert.y > furthestEnds[2].y)
                    {
                        furthestEnds[2] = toShrinkVert[j];
                    }
                    if (currentVert.y < furthestEnds[3].y)
                    {
                        furthestEnds[3] = toShrinkVert[j];
                    }
                    // Z --------------------------
                    if (currentVert.z > furthestEnds[4].z)
                    {
                        furthestEnds[4] = toShrinkVert[j];
                    }
                    if (currentVert.z < furthestEnds[5].z)
                    {
                        furthestEnds[5] = toShrinkVert[j];
                    }
                }
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log("Index is out of range" + e);
        }
        catch (Exception e)
        {
            Debug.Log("Exception thrown" + e);
        }

        return furthestEnds;

    }


    // Method which is called on each subpart of a structure and calls all appropriate methods to set up
    // the meshBox and then call the shrinking of the mesh. 
    Mesh simplifyMesh(GameObject subPartShrink)
    {
        // box size variables 
        int boxSize_2 = boxSize * boxSize;
        int boxSize_2_sub = boxSize_2 - boxSize;

        // pass through the game object 
        GShrink = subPartShrink; 
       

        if (!GShrink)
        {
            Debug.Log("GameObject to shrink has not been found");
            return null;
        }

        // Get the furthest ends of each of the sides of the mesh to Shrink 
        furthestEnds = findFurthestEnds(GShrink);


        // Set up the meshBox 
        meshBox = SetUpMeshBox(GShrink, furthestEnds, meshBoxContainer);
        meshBoxV = meshBox.vertices;

        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /** Add the triangles to the Mesh   */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        // formular to calculate the number of triangles - 6 for 6 faces of cube 
        int triPerFace = (boxSize - 1) * (boxSize - 1) * 2;
        int totalTriangles = triPerFace * 6;

        int totalTriangleIndex = totalTriangles * 3;

        int[] triangleStore = new int[totalTriangleIndex];
        int tStoreCount = 0;

        // add the triangles to the faces of the cube. 
        tStoreCount = triangleFace(boxSize_2_sub, boxSize_2, triangleStore, tStoreCount, topFace);
        tStoreCount = triangleFace(boxSize_2_sub, boxSize_2, triangleStore, tStoreCount, bottomFace);
        tStoreCount = triangleFace(boxSize_2_sub, boxSize_2, triangleStore, tStoreCount, leftFace);
        tStoreCount = triangleFace(boxSize_2_sub, boxSize_2, triangleStore, tStoreCount, rightFace);
        tStoreCount = triangleFace(boxSize_2_sub, boxSize_2, triangleStore, tStoreCount, frontFace);
        tStoreCount = triangleFace(boxSize_2_sub, boxSize_2, triangleStore, tStoreCount, backFace);

        // set the triangles to the mesh. 
        meshBox.triangles = triangleStore;

       // meshBoxContainer.GetComponent<MeshFilter>().mesh = meshBox;

       return shrinkMesh(GShrink, meshOutput, boxSize, furthestEnds, meshBox);

    }
}
