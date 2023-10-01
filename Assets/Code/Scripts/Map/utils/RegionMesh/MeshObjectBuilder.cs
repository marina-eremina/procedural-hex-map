using System.Collections.Generic;
using UnityEngine;

public class MeshObjectBuilder
{
    public GameObject BuildMeshObject(Mesh mesh, Material material, string gameObjectName = null)
    {
        if (string.IsNullOrEmpty(gameObjectName) || mesh == null)
            return null;

        GameObject meshObject = new GameObject(gameObjectName);
        meshObject.transform.localPosition = Vector3.zero;

        MeshRenderer renderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter filter = meshObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        renderer.material = material;

        return meshObject;
    }
}
