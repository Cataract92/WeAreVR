using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MainObject : MonoBehaviour
{
    public static float Radius = 0.15f;

    private static VertexDummy _highlight;
    public static VertexDummy Highlight
    {
        get { return _highlight; }
        set
        {
            if (_highlight != null)
                _highlight.UndoHighlight();
            _highlight = value;
            if (_highlight != null)
                _highlight.DoHighlight();
        }
    }
    public CustomMesh Mesh;

    // Start is called before the first frame update
    void Start()
    {
        Mesh = new CustomMesh(this);

        StartCoroutine(Countdown());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Mesh.NeedsRedraw)
        {*/
        GetComponent<MeshFilter>().mesh = Mesh.ToMesh();
            Destroy(GetComponent<MeshCollider>());
            gameObject.AddComponent<MeshCollider>();
        //}
        
    }

    private IEnumerator Countdown()
    {
        float duration = 3f; // 3 seconds you can change this 
        //to whatever you want
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        new CustomMesh.Vertex(new Vector3(1, 1, -2), Mesh, Mesh.Faces[0]);
    }
}
