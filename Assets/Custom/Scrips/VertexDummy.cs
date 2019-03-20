using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class VertexDummy: MonoBehaviour
{
    public CustomMesh.Vertex Vertex;
    public CustomMesh.Edge Edge;
    public CustomMesh.Face Face;

    public Material VertexMaterial;
    public Material FaceMaterial;

    public Quaternion InitialFaceRotation;

    [HideInInspector]
    public bool IsAttached = false;

    CustomPlayer player;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        player = FindObjectOfType<CustomPlayer>();
    }

    
    // Update is called once per frame
    void Update()
    {

        if (IsAttached)
        {
            if (Vertex != null)
            {
                Vertex.Mesh.NeedsRedraw = true;
                Vertex.Position = Quaternion.Inverse(Vertex.Mesh.MainObject.transform.rotation) * (transform.position - Vertex.Mesh.MainObject.transform.position) * 1f / Vertex.Mesh.MainObject.transform.localScale.x;

                foreach (var face in Vertex.Faces)
                {
                    face.calculateDummyPosition();
                }

                foreach (var edge in Vertex.Edges)
                {
                    edge.calculateDummyPosition();
                }
            }

            if (Edge != null)
            {

                Edge.Mesh.NeedsRedraw = true;

                var v0 = Edge.Vertices[0];
                var v1 = Edge.Vertices[1];

                v0.VertexDummy.transform.position = transform.position + (transform.rotation * InitialFaceRotation * Quaternion.AngleAxis(180f, transform.forward)) * Edge.InitialVectors[v0];
                v0.Position = Quaternion.Inverse(Edge.Mesh.MainObject.transform.rotation) * (v0.VertexDummy.transform.position - Edge.Mesh.MainObject.transform.position) * 1f / Edge.Mesh.MainObject.transform.localScale.x;

                v1.VertexDummy.transform.position = transform.position + (transform.rotation * InitialFaceRotation * Quaternion.AngleAxis(180f, transform.forward)) * Edge.InitialVectors[v1];
                v1.Position = Quaternion.Inverse(Edge.Mesh.MainObject.transform.rotation) * (v1.VertexDummy.transform.position - Edge.Mesh.MainObject.transform.position) * 1f / Edge.Mesh.MainObject.transform.localScale.x;

                foreach(var vertex in Edge.Vertices)
                {
                    foreach (var face in vertex.Faces)
                    {
                        face.calculateDummyPosition();
                    }

                    foreach (var edge in vertex.Edges)
                    {
                        if (edge == Edge)
                            continue;

                        edge.calculateDummyPosition();
                    }
                }
            }

            if (Face != null)
            {
                Face.Mesh.NeedsRedraw = true;

                var v0 = Face.Vertices[0];
                var v1 = Face.Vertices[1];
                var v2 = Face.Vertices[2];

                v0.VertexDummy.transform.position = transform.position + (transform.rotation * InitialFaceRotation * Quaternion.AngleAxis(180f, transform.forward))  * Face.InitialVectors[v0];
                v0.Position = Quaternion.Inverse(Face.Mesh.MainObject.transform.rotation) * (v0.VertexDummy.transform.position - Face.Mesh.MainObject.transform.position) * 1f / Face.Mesh.MainObject.transform.localScale.x;

                v1.VertexDummy.transform.position = transform.position + (transform.rotation * InitialFaceRotation * Quaternion.AngleAxis(180f, transform.forward)) * Face.InitialVectors[v1];
                v1.Position = Quaternion.Inverse(Face.Mesh.MainObject.transform.rotation) * (v1.VertexDummy.transform.position - Face.Mesh.MainObject.transform.position) * 1f / Face.Mesh.MainObject.transform.localScale.x;

                v2.VertexDummy.transform.position = transform.position + (transform.rotation * InitialFaceRotation * Quaternion.AngleAxis(180f, transform.forward)) * Face.InitialVectors[v2];
                v2.Position = Quaternion.Inverse(Face.Mesh.MainObject.transform.rotation) * (v2.VertexDummy.transform.position - Face.Mesh.MainObject.transform.position) * 1f / Face.Mesh.MainObject.transform.localScale.x;

                foreach (var vertex in Face.Vertices)
                {
                    foreach (var face in vertex.Faces)
                    {
                        if (face == Face)
                            continue;
                        face.calculateDummyPosition();
                    }

                    foreach (var edge in Face.Edges)
                    {
                        edge.calculateDummyPosition();
                    }
                }
            }
            
        }
    }

    public void DoHighlight()
    {
        if (!IsAttached)
        {
            if (Vertex != null)
                transform.localScale = new Vector3(player.transform.localScale.x / Vertex.Mesh.MainObject.transform.localScale.x, player.transform.localScale.y / Vertex.Mesh.MainObject.transform.localScale.y, player.transform.localScale.z / Vertex.Mesh.MainObject.transform.localScale.z) * 0.03f;
            if (Face != null)
                transform.localScale = new Vector3(player.transform.localScale.x / Face.Mesh.MainObject.transform.localScale.x, player.transform.localScale.y / Face.Mesh.MainObject.transform.localScale.y, player.transform.localScale.z / Face.Mesh.MainObject.transform.localScale.z) * 0.03f;
            if (Edge != null)
                transform.localScale = new Vector3(player.transform.localScale.x / Edge.Mesh.MainObject.transform.localScale.x, player.transform.localScale.y / Edge.Mesh.MainObject.transform.localScale.y, player.transform.localScale.z / Edge.Mesh.MainObject.transform.localScale.z) * 0.03f;
        }

        GetComponent<Renderer>().enabled = true;
    }

    public void UndoHighlight()
    {
        GetComponent<Renderer>().enabled = false;
    }

    protected virtual void OnAttachedToHand(Hand hand)
    {
        transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
    }
}
