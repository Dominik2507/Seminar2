using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeshToMarchingCubes : MonoBehaviour
{
    Mesh objectMesh;
    Mesh generatedMesh;
    GameObject generatedMeshObject;
    public short [,,] voxelGrid;
    public Bounds objectBounding;

    public float step = 0.01f;

    public bool debugMarchingCubes = false;
    public bool redraw = false;
    public bool inverse = false;
    public bool swapMesh = true;

    public short a = 0;
    public short b = 0;
    public short c = 0;
    public short d = 0;
    public short e = 0;
    public short f = 0;
    public short g = 0;
    public short h = 0;

    void Start()
    {
        objectMesh = gameObject.GetComponent<MeshFilter>().mesh;
        objectBounding = objectMesh.bounds;
        generatedMesh = new Mesh();
        

        if(debugMarchingCubes == true)
        {
            voxelGrid = new short[2, 2, 2];
            voxelGrid[0, 0, 0] = a;
            voxelGrid[0, 0, 1] = b;
            voxelGrid[0, 1, 1] = c;
            voxelGrid[0, 1, 0] = d;
            voxelGrid[1, 0, 0] = e;
            voxelGrid[1, 0, 1] = f;
            voxelGrid[1, 1, 1] = g;
            voxelGrid[1, 1, 0] = h;
        }
        else
        {
            MeshToVoxels();
            //DrawPointCloud();
        }

        Debug.Log("Running marching cubes");

        if (swapMesh)
        {
            VoxelsToMesh();
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            filter.mesh.Clear();
            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
            filter.mesh = generatedMesh;
        }
        else
        {
            generatedMeshObject = new GameObject("Marching Cubes mesh");
            VoxelsToMesh();
            MeshFilter filter = generatedMeshObject.AddComponent<MeshFilter>();
            filter.mesh = generatedMesh;
            MeshRenderer meshRenderer = generatedMeshObject.AddComponent<MeshRenderer>();
            if (debugMarchingCubes) meshRenderer.material = new Material(Shader.Find("Standard"));
            else meshRenderer.material = gameObject.GetComponent<MeshRenderer>().material;
            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
            generatedMeshObject.transform.localScale = gameObject.transform.localScale;
            generatedMeshObject.AddComponent<BoxCollider>();
        }

       


        if (debugMarchingCubes)
        {

            GameObject pointCloudObject = new GameObject("PointCloud");

            // Add a MeshFilter component to the point cloud object
            MeshFilter meshFilter = pointCloudObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();

            // Create a new MeshRenderer component and assign a material to it
            MeshRenderer renderer = pointCloudObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Standard"));

            // Set the material color (optional)
            renderer.material.color = Color.blue;

            // Create lists to hold vertices and indices for the point cloud mesh
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            vertices.Add(new Vector3(objectBounding.min.x, objectBounding.min.y, objectBounding.min.z));
            vertices.Add(new Vector3(objectBounding.min.x, objectBounding.min.y, objectBounding.max.z));
            vertices.Add(new Vector3(objectBounding.min.x, objectBounding.max.y, objectBounding.max.z));
            vertices.Add(new Vector3(objectBounding.min.x, objectBounding.max.y, objectBounding.min.z));
            vertices.Add(new Vector3(objectBounding.max.x, objectBounding.min.y, objectBounding.min.z));
            vertices.Add(new Vector3(objectBounding.max.x, objectBounding.min.y, objectBounding.max.z));
            vertices.Add(new Vector3(objectBounding.max.x, objectBounding.max.y, objectBounding.max.z));
            vertices.Add(new Vector3(objectBounding.max.x, objectBounding.max.y, objectBounding.min.z));

            indices.Add(0);
            indices.Add(1);
            indices.Add(2);
            indices.Add(3);
            indices.Add(4);
            indices.Add(5);
            indices.Add(6);
            indices.Add(7);

            meshFilter.mesh.vertices = vertices.ToArray();
            meshFilter.mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);

            // Recalculate the bounds of the mesh
            meshFilter.mesh.RecalculateBounds();
        }

    }

    void Update()
    {

        if (Keyboard.current.rKey.wasPressedThisFrame || redraw)
        {
            if (debugMarchingCubes)
            {
                voxelGrid[0, 0, 0] = a;
                voxelGrid[0, 0, 1] = b;
                voxelGrid[0, 1, 1] = c;
                voxelGrid[0, 1, 0] = d;
                voxelGrid[1, 0, 0] = e;
                voxelGrid[1, 0, 1] = f;
                voxelGrid[1, 1, 1] = g;
                voxelGrid[1, 1, 0] = h;

                generatedMesh.Clear();
                VoxelsToMesh();
            }
            else
            {
                VoxelsToMesh();
                MeshFilter filter = gameObject.GetComponent<MeshFilter>();
                filter.mesh.Clear();
                generatedMesh.RecalculateBounds();
                generatedMesh.RecalculateNormals();
                generatedMesh.RecalculateTangents();
                filter.mesh = generatedMesh;
            }
            redraw = false;
        }

        if (inverse)
        {
            a = (short)(1 - a);
            b = (short)(1 - b);
            c = (short)(1 - c);
            d = (short)(1 - d);
            e = (short)(1 - e);
            f = (short)(1 - f);
            g = (short)(1 - g);
            h = (short)(1 - h);
            inverse = false;
            redraw = true;
        }
    }

    void DrawPointCloud()
    {
        // Create a new empty GameObject to hold the point cloud
        GameObject pointCloudObject = new GameObject("PointCloud");

        // Add a MeshFilter component to the point cloud object
        MeshFilter meshFilter = pointCloudObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();

        // Create a new MeshRenderer component and assign a material to it
        MeshRenderer meshRenderer = pointCloudObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
      
        // Set the material color (optional)
        meshRenderer.material.color = Color.blue;

        // Create lists to hold vertices and indices for the point cloud mesh
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        // Add vertices for points that are inside the mesh
        for (int x = 1; x < voxelGrid.GetLength(0); x++)
        {
            for (int y = 1; y < voxelGrid.GetLength(1); y++)
            {
                for (int z = 1; z < voxelGrid.GetLength(2); z++)
                {
                    if (voxelGrid[x, y, z] == 1)
                    {
                        // Calculate the world position of the point
                        Vector3 worldPosition = transform.TransformPoint(new Vector3(
                            objectBounding.min.x + (x) * step,
                            objectBounding.min.y + (y) * step,
                            objectBounding.min.z + (z) * step));

                        // Add the world position to the list of vertices
                        vertices.Add(worldPosition);

                        // Add the index of the vertex to the list of indices
                        indices.Add(vertices.Count - 1);
                    }
                }
            }
        }

        // Assign the vertices and indices to the mesh
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);

        // Recalculate the bounds of the mesh
        meshFilter.mesh.RecalculateBounds();
    }

    void MeshToVoxels()
    {
        //NormalizeMesh();
        objectBounding = objectMesh.bounds;
        Vector3Int gridSize = new Vector3Int(
            Mathf.CeilToInt(2 + objectBounding.size.x / step),
            Mathf.CeilToInt(2 + objectBounding.size.y / step),
            Mathf.CeilToInt(2 + objectBounding.size.z / step)
        );

        voxelGrid = new short[gridSize.x, gridSize.y, gridSize.z];
        Debug.Log("Trying to create a 3D matrix of dims: " + gridSize.x + "," + gridSize.y + ", " + gridSize.z);

        Vector3 localPoint = Vector3.zero;
        int pointsInMesh = 0;
        for (float x = objectBounding.min.x; x <= objectBounding.max.x; x += step)
            for (float y = objectBounding.min.y; y <= objectBounding.max.y; y += step)
                for (float z = objectBounding.min.z; z <= objectBounding.max.z ; z += step)
                {
                    localPoint.Set(x, y, z);
                    Vector3 point = transform.TransformPoint(localPoint);

                    RaycastHit[] hits = Physics.RaycastAll(point, Vector3.up, float.PositiveInfinity);

                    List<RaycastHit> filteredHits = new List<RaycastHit>();
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.gameObject == gameObject)
                        {
                            filteredHits.Add(hit);
                        }
                    }

                    int indexX = 1 + Mathf.FloorToInt((x - objectBounding.min.x) / step);
                    int indexY = 1 + Mathf.FloorToInt((y - objectBounding.min.y) / step);
                    int indexZ = 1 + Mathf.FloorToInt((z - objectBounding.min.z) / step);

                    int nHits = filteredHits.Count;
                  
                    if(nHits >= 1 && Vector3.Dot(filteredHits[0].normal, Vector3.up) >= 0)
                    {
                        voxelGrid[indexX, indexY, indexZ] = 1;
                        pointsInMesh++;

                    }
                    //else
                    //{
                    //    Debug.Log(indexX + " " + indexY + " " + indexZ + ", " + x + " " + y + " " + z);
                    //    voxelGrid[indexX, indexY, indexZ] = 0;
                    //}
                }
        Debug.Log("Size " + objectBounding.size.x.ToString() + ", " + objectBounding.size.y + ", " + objectBounding.size.z);
        Debug.Log(pointsInMesh.ToString() + " Points in mesh");

        Debug.Log(objectMesh.vertices.ToString());
    }

    //Marching cubes
    void VoxelsToMesh()
    {
        List<Vector3> foundVertices = new();
        List<int> indices = new List<int>();

        for (int x = 0; x < voxelGrid.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < voxelGrid.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < voxelGrid.GetLength(2) - 1; z++)
                {
                    int config = MarchingCubes.GetCubeConfig(voxelGrid, x, y, z);

                    if (config == 0) continue;
                    if(debugMarchingCubes) Debug.Log(config);

                    float x_0 = x * objectBounding.size.x / (voxelGrid.GetLength(0) - 1) + objectBounding.min.x;
                    float x_half = (x + 0.5f) * objectBounding.size.x / (voxelGrid.GetLength(0) - 1) + objectBounding.min.x;
                    float x_1 = (x + 1) * objectBounding.size.x / (voxelGrid.GetLength(0) - 1) + objectBounding.min.x;
                    float y_0 = y * objectBounding.size.y / (voxelGrid.GetLength(1) - 1) + objectBounding.min.y;
                    float y_half = (y + 0.5f) * objectBounding.size.y / (voxelGrid.GetLength(1) - 1) + objectBounding.min.y;
                    float y_1 = (y + 1) * objectBounding.size.y / (voxelGrid.GetLength(1) - 1) + objectBounding.min.y;

                    float z_0 = z * objectBounding.size.z / (voxelGrid.GetLength(2) - 1) + objectBounding.min.z;
                    float z_half = (z + 0.5f) * objectBounding.size.z / (voxelGrid.GetLength(2) - 1) + objectBounding.min.z;
                    float z_1 = (z + 1) * objectBounding.size.z / (voxelGrid.GetLength(2) - 1) + objectBounding.min.z;

                    int currPoints = foundVertices.Count;
                    #region Defining Faces


                    #region Single point drawing
                    if (config == 7 || config == 8) // a
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 7)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 9 || config == 10) // b
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 9)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 11 || config == 12) // c
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 11)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 13 || config == 14) // d
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 13)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 15 || config == 16) // e
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 15)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 17 || config == 18) // f
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 17)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 19 || config == 20) // g
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 19)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    else if (config == 21 || config == 22) // h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 21)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                    }
                    #endregion

                    #region 2 points drawing
                    #region 2 on common edge
                    else if (config == 23 || config == 24) // a-b
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 23)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 25 || config == 26) // a-d
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 25)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 27 || config == 28) // a-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 27)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 29 || config == 30) // g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 29)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 31 || config == 32) // g-f
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 31)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 33 || config == 34) // g-c
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 33)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 35 || config == 36) // b-c
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 35)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 37 || config == 38) // c-d
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 37)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 39 || config == 40) // b-f
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        if (config == 39)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 41 || config == 42) // e-f
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        if (config == 41)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 43 || config == 44) // e-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 43)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    else if (config == 45 || config == 46) // d-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 45)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                    }
                    #endregion

                    #region 2 on face diagonal drawing
                    else if (config == 47 || config == 48) // a-c
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));


                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 47)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 49 || config == 50) // b-d
                    {

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 49)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 51 || config == 52) // e-g
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 51)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 53 || config == 54) // f-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 53)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 55 || config == 56) // a-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 55)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 57 || config == 58) // b-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 57)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 59 || config == 60) // b-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 59)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 61 || config == 62) // c-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 61)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 63 || config == 64) // c-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 63)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 65 || config == 66) // d-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 65)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 67 || config == 68) // a-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 67)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 69 || config == 70) // d-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 69)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    #endregion
                    #region 2 on space diagonal
                    else if (config == 71 || config == 72) // a-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 71)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 73 || config == 74) // b-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 73)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 75 || config == 76) // c-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 75)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    else if (config == 77 || config == 78) // d-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 77)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                    }
                    #endregion

                    #endregion

                    #region 3 points drawing
                    #region 3 on same face
                    else if (config == 79 || config == 80) // a-b-c
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 79)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 81 || config == 82) // a-b-d
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 81)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 83 || config == 84) // a-c-d
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 83)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 85 || config == 86)
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 85)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 87 || config == 88) // e-f-g
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 87)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 89 || config == 90) // e-f-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 89)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 91 || config == 92) // e-g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 91)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 93 || config == 94) // f-g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 93)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 95 || config == 96) // a-b-f
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        if (config == 95)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 97 || config == 98) // a-e-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 97)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 99 || config == 100) // a-b-e
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));

                        if (config == 99)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 101 || config == 102) // b-e-f
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        if (config == 101)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 103 || config == 104) // b-c-g
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 103)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 105 || config == 106) // b-f-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 105)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 107 || config == 108) // b-c-f
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 107)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 109 || config == 110) // c-f-g
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 109)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 111 || config == 112) // c-g-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        if (config == 111)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 113 || config == 114) // c-d-g
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        if (config == 113)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 115 || config == 116) // c-d-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));

                        if (config == 115)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 117 || config == 118) // d-g-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 117)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 119 || config == 120) // a-d-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 119)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 121 || config == 122) // a-d-e
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 121)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 123 || config == 124) // a-e-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 123)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    else if (config == 125 || config == 126) // d-e-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 125)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                        }
                    }
                    #endregion

                    #region 2 on edge one not conected
                    else if (config == 127 || config == 128) // a-b, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 128)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 127)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 129 || config == 130) // a-b, h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 130)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 129)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 131 || config == 132) // b-c, e
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 132)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 131)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }

                    }
                    else if (config == 133 || config == 134) // b-c, h
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 134)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 133)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }

                    }
                    else if (config == 135 || config == 136) // c-d, e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 136)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 135)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 137 || config == 138) // c-d, f
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 138)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 137)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 139 || config == 140) // a-d, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 139)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 139)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 141 || config == 142) // a-d, f
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 141)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 141)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 143 || config == 144) // e-f, c
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        if (config == 143)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 143)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 145 || config == 146) // e-f, d
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        if (config == 145)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 145)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 147 || config == 148) // f-g, d
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 148)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 147)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 149 || config == 150) // f-g, a
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 150)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 149)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 151 || config == 152) // g-h, a
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 152)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 151)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 153 || config == 154) // g-h, b
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 154)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 153)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 155 || config == 156) // e-h, c
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 156)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 155)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 157 || config == 158) // e-h, b
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 158)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 157)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 159 || config == 160) // a-e, c
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 160)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 159)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 161 || config == 162) // a-e, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 162)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 161)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 163 || config == 164) // b-f, d
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        if (config == 163)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 163)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 165 || config == 166) // b-f, h
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        if (config == 165)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 165)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 167 || config == 168) // c-g, a
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 167)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 167)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 169 || config == 170) // c-g, e
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 169)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 169)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 171 || config == 172) // d-h, b
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 172)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 171)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 173 || config == 174) // d-h, f
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 174)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 173)
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 6);
                        }
                        else
                        {
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 175 || config == 176) // a, f, h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        if (config == 175)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }


                        if (config == 175)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }


                        if (config == 175)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 177 || config == 178) // a, c, f
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 177)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        if (config == 177)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 177)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 179 || config == 180) // a, c, h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        if (config == 179)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 179)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 179)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 181 || config == 182) // b, g, e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 181)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 181)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 181)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 183 || config == 184) // b, d, e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 183)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 183)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 183)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 185 || config == 186) // b, d, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        if (config == 185)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 185)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 185)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 187 || config == 188) // c, f, h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        if (config == 187)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 187)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 187)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    else if (config == 189 || config == 190) // d, e, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        if (config == 189)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);
                        }
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 189)
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                        else
                        {
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }

                        if (config == 189)
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 7);
                            indices.Add(currPoints + 8);
                        }
                        else
                        {
                            indices.Add(currPoints + 6);
                            indices.Add(currPoints + 8);
                            indices.Add(currPoints + 7);
                        }
                    }
                    #endregion
                    #endregion

                    #region 4 points drawing

                    #region 4 on one face
                    else if (config == 1 || config == 2)
                    {

                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        if (config == 1)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                        }

                    }
                    else if (config == 3 || config == 4)
                    {

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 4)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                        }

                    }
                    else if (config == 5 || config == 6)
                    {

                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 5)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 1);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 3);
                        }

                    }
                    #endregion

                    #region 3 on one face, one not connected to rest
                    else if (config == 191) // a-b-c, h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));


                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 192) // e-f-g, d
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 193) // a-b-d, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 194) // e-f-h, c
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 195) // a-c-d, f
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 196) // e-g-h, b
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 197) // b-c-d, e
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 198) // f-g-h, a
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 199) // a-b-f, h
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 200) // c-d-g, e
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 201) // a-b-e, g
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 202) // c-d-h, f
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 203) // a-e-f, c
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 204) // d-h-g, b
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 205) // b-f-e, d
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 206) // c-g-h, a
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 207) // b-c-g, e
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 208) // a-d-h, f
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 209) // b-c-f, h
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 210) // a-d-e, g
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 211) // b-g-f, d
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 212) // a-e-h, c
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    else if (config == 213) // c-g-f, a
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));


                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);

                    }
                    else if (config == 214) // d-e-h, b
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));

                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                    }
                    #endregion

                    #region 3 connected to one common vertex

                    else if (config == 215 || config == 216) // a-b-d-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        if (config == 215)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 217 || config == 218) // b-c-f-a
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        if (config == 217)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 219 || config == 220) // c-d-g-b
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        if (config == 219)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                    }
                    else if (config == 221 || config == 222) // d-a-c-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        if (config == 221)
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 1);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 5);
                            indices.Add(currPoints + 4);
                        }
                        else
                        {
                            indices.Add(currPoints);
                            indices.Add(currPoints + 2);
                            indices.Add(currPoints + 1);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 2);

                            indices.Add(currPoints);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 3);

                            indices.Add(currPoints + 3);
                            indices.Add(currPoints + 4);
                            indices.Add(currPoints + 5);
                        }
                    }
                    #endregion

                    #region 3 on face, one connected to not the common vertex
                    else if (config == 223) // a-b-c-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 224) // d-f-g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 225) // a-b-c-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 226) // d-e-f-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 227) // a-b-d-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 228) // c-e-f-g
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 229) // a-b-d-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 230) // c-e-g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 231) // a-c-d-e
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 232) // b-f-g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 233) // a-c-d-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 234) // b-e-f-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half)); 
                        foundVertices.Add(new Vector3(x_0, y_half, z_1)); 
                        foundVertices.Add(new Vector3(x_half, y_0, z_0)); 
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0)); 

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 235) // b-c-d-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 236) // a-e-f-g
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 237) // b-c-d-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 238) // a-e-g-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 239) // a-b-f-g
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 240) // c-d-e-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 241) // a-b-e-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 242) // c-d-f-g
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 243) // a-d-e-f
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 244) // b-c-g-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);

                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 3);
                    }
                    else if (config == 245) // b-c-e-f
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 246) // a-d-g-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 3);

                        indices.Add(currPoints + 0);
                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 5);
                    }
                    #endregion

                    #region 2 diagonal edges
                    else if (config == 247) // a-e, c-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 1);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);

                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 248) // b-f, d-h
                    {
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 1);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);

                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 249) // a-b, g-h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 1);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);

                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 5);


                    }
                    else if (config == 250) // e-f, c-d
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_half, z_1));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 1);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);

                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 251) // a-d, f-g
                    {
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 1);

                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);

                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 5);
                    }
                    else if (config == 252) // b-c, e-h
                    {
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 2);
                        indices.Add(currPoints + 1);

                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);
                        indices.Add(currPoints + 6);

                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 5);
                    }
                    #endregion

                    #region 4 not conected
                    else if (config == 253) // a, c, f, h
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));

                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 8);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));

                        indices.Add(currPoints + 9);
                        indices.Add(currPoints + 10);
                        indices.Add(currPoints + 11);

                    }
                    else if (config == 254) // b, d, e, g
                    {
                        foundVertices.Add(new Vector3(x_0, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_0, z_1));
                        foundVertices.Add(new Vector3(x_0, y_0, z_half));


                        indices.Add(currPoints);
                        indices.Add(currPoints + 1);
                        indices.Add(currPoints + 2);

                        foundVertices.Add(new Vector3(x_0, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_1, z_0));
                        foundVertices.Add(new Vector3(x_0, y_1, z_half));

                        indices.Add(currPoints + 3);
                        indices.Add(currPoints + 4);
                        indices.Add(currPoints + 5);

                        foundVertices.Add(new Vector3(x_1, y_half, z_0));
                        foundVertices.Add(new Vector3(x_half, y_0, z_0));
                        foundVertices.Add(new Vector3(x_1, y_0, z_half));

                        indices.Add(currPoints + 6);
                        indices.Add(currPoints + 7);
                        indices.Add(currPoints + 8);

                        foundVertices.Add(new Vector3(x_1, y_half, z_1));
                        foundVertices.Add(new Vector3(x_half, y_1, z_1));
                        foundVertices.Add(new Vector3(x_1, y_1, z_half));

                        indices.Add(currPoints + 9);
                        indices.Add(currPoints + 10);
                        indices.Add(currPoints + 11);


                    }
                    #endregion
                    #endregion





                    #endregion

                }
            }
        }
        generatedMesh.Clear();
        generatedMesh.vertices = foundVertices.ToArray();
        generatedMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER, MarchingCubes");
    }


}
