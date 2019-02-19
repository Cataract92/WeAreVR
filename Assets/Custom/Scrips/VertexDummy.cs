using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexDummy: MonoBehaviour
{
    public CustomMesh.Vertex Vertex;
    public CustomMesh.Face Face;

    public Quaternion InitialFaceRotation;

    [HideInInspector]
    public bool IsAttached = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAttached)
        {
            if (Vertex != null)
            {
                Vertex.Position = Quaternion.Inverse(Vertex.Mesh.MainObject.transform.rotation) * (transform.position - Vertex.Mesh.MainObject.transform.position);
                foreach (var face in Vertex.Faces)
                {
                    face.calculateDummyPosition();
                }
            }

            if (Face != null)
            {
                var v0 = Face.Vertices[0];
                var v1 = Face.Vertices[1];
                var v2 = Face.Vertices[2];


                v0.VertexDummy.transform.position = transform.position + transform.rotation /* * InitialFaceRotation */  * Face.Rotation *  (Face.Mesh.MainObject.transform.rotation * Face.InitialVectors[v0]);
                v0.Position = Quaternion.Inverse(Face.Mesh.MainObject.transform.rotation) * (v0.VertexDummy.transform.position - Face.Mesh.MainObject.transform.position);

                v1.VertexDummy.transform.position = transform.position + transform.rotation /* * InitialFaceRotation */ * Face.Rotation * (Face.Mesh.MainObject.transform.rotation * Face.InitialVectors[v1]);
                v1.Position = Quaternion.Inverse(Face.Mesh.MainObject.transform.rotation) * (v1.VertexDummy.transform.position - Face.Mesh.MainObject.transform.position);

                v2.VertexDummy.transform.position = transform.position + transform.rotation /* * InitialFaceRotation */ * Face.Rotation * (Face.Mesh.MainObject.transform.rotation * Face.InitialVectors[v2]);
                v2.Position = Quaternion.Inverse(Face.Mesh.MainObject.transform.rotation) * (v2.VertexDummy.transform.position - Face.Mesh.MainObject.transform.position);

                foreach (var vertex in Face.Vertices)
                {
                    foreach (var face in vertex.Faces)
                    {
                        if (face != Face)
                            face.calculateDummyPosition();
                    }
                }
            }

        }
    }

    public void DoHighlight()
    {
        GetComponent<Renderer>().enabled = true;
    }

    public void UndoHighlight()
    {
        GetComponent<Renderer>().enabled = false;
    }
}
