using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexDummy: MonoBehaviour
{
    public CustomMesh.Vertex Vertex;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vertex.Position = transform.position -Vertex.Mesh.MainObject.transform.position;
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
