using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSlicer : MonoBehaviour
{
    public Transform edgeStart;
    public Transform edgeEnd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("TRIGGER, voxel slicer");
        if (other.TryGetComponent<MeshToMarchingCubes>(out MeshToMarchingCubes mesh)) SliceMesh(mesh);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER, voxel slicer");
        if(other.TryGetComponent<MeshToMarchingCubes>(out MeshToMarchingCubes mesh)) SliceMesh(mesh);
    }


    private void SliceMesh(MeshToMarchingCubes mesh)
    {
        Debug.Log("Start Slice mesh");
        float t = 0;
        bool sliced = false;
        while ( t <  1f)
        {
            Vector3 position = Vector3.Lerp(edgeStart.position, edgeEnd.position, t);

            // transform to position in local space of mesh transform
            position = mesh.transform.InverseTransformPoint(position);

            int indexX = 1 + Mathf.FloorToInt((position.x - mesh.objectBounding.min.x) / mesh.step);
            int indexY = 1 + Mathf.FloorToInt((position.y - mesh.objectBounding.min.y) / mesh.step);
            int indexZ = 1 + Mathf.FloorToInt((position.z - mesh.objectBounding.min.z) / mesh.step);

            if (indexX > 0 && indexY > 0 && indexZ > 0 && indexX < mesh.voxelGrid.GetLength(0) && indexY < mesh.voxelGrid.GetLength(1) && indexZ < mesh.voxelGrid.GetLength(2))
            {
                Debug.Log("Succesful slice on " + indexX + "," + indexY + "," + indexZ);

                if(mesh.voxelGrid[indexX, indexY, indexZ] == 1)
                {
                    mesh.voxelGrid[indexX, indexY, indexZ] = 0;
                    sliced = true;
                }
            }
            t += 0.01f;
        }

        if (sliced) mesh.redraw = true;
    }

}
