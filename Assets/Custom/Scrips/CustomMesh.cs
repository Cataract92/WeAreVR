using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class CustomMesh
{
    public class Vertex
    {
        public Vector3 Position;
        public List<Face> Faces = new List<Face>();
        public VertexDummy VertexDummy;
        public CustomMesh Mesh;

        public Vertex(Vector3 position, CustomMesh mesh, Face face = null)
        {
            Position = position;
            Mesh = mesh;

            
            VertexDummy = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Vertex"))).GetComponent<VertexDummy>();

            VertexDummy.transform.parent = Mesh.MainObject.transform;

            VertexDummy.Vertex = this;
            VertexDummy.transform.localPosition = position;
            

            Mesh.Vertices.Add(this);

            if (face != null)
            {
                Vertex[] neighbours = face.Vertices;
                face.Remove();
                var v0 = neighbours[0];
                var v1 = neighbours[1];
                var v2 = neighbours[2];

                new Face(mesh, new[] { v0, v1, this });
                new Face(mesh, new[] { v1, v2, this });
                new Face(mesh, new[] { v2, v0, this });
            }

            Mesh.NeedsRedraw = true;
        }

        public void Remove()
        {
            if (Mesh.Faces.Count == 4)
                return;

            List<Vertex> neighbours = new List<Vertex>();
            foreach (var face in Faces)
            {
                face.Remove();
                foreach (var vertex in face.Vertices)
                {
                    neighbours.Add(vertex);
                }
            }

            for (int i = 1; i < neighbours.Count-1; i++)
            {
                new Face(Mesh, new[] {neighbours[0], neighbours[i], neighbours[i + 1]});
            }

            Mesh.NeedsRedraw = true;
        }

        public void Move(Vector3 NewPosition)
        {
            Mesh.NeedsRedraw = true;
        }
    }

    public class Face
    {
        public Vertex[] Vertices = new Vertex[3];
        public CustomMesh Mesh;
        public Material Material;
        public VertexDummy FaceDummy;
        public Vector3 Normal;

        public Dictionary<Vertex,Vector3> InitialVectors = new Dictionary<Vertex, Vector3>();
        public Quaternion Rotation = new Quaternion();

        public Face(CustomMesh mesh, Vertex[] vertices)
        {
            Mesh = mesh;
            Vertices = vertices;
            foreach (var vertex in Vertices)
            {
                vertex.Faces.Add(this);
                InitialVectors.Add(vertex,new Vector3());
            }

            Mesh.Faces.Add(this);

            FaceDummy = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Vertex"))).GetComponent<VertexDummy>();
            FaceDummy.transform.parent = mesh.MainObject.transform;
            FaceDummy.Face = this;

            calculateDummyPosition();

            Mesh.NeedsRedraw = true;
        }

        public void Remove()
        {
            foreach (var vertex in Vertices)
            {
                vertex.Faces.Remove(this);
            }

            Mesh.Faces.Remove(this);

            Mesh.NeedsRedraw = true;
        }

        public void AddVertex()
        {
            var position = (Vertices[0].Position + Vertices[1].Position + Vertices[2].Position) * (1f / 3);

            new Vertex(position, Mesh, this);

            Mesh.NeedsRedraw = true;
        }

        public void calculateDummyPosition()
        {
            FaceDummy.transform.localPosition =
                (Vertices[0].Position + Vertices[1].Position + Vertices[2].Position) / 3f;

            foreach (var vertex in Vertices)
            {
                InitialVectors[vertex] = Quaternion.Inverse(Mesh.MainObject.transform.rotation) * ( vertex.VertexDummy.transform.position - FaceDummy.transform.position);
            }

            Normal = Vector3.Cross(Vertices[1].VertexDummy.transform.position - Vertices[0].VertexDummy.transform.position, Vertices[2].VertexDummy.transform.position - Vertices[0].VertexDummy.transform.position);
            FaceDummy.transform.rotation *= Quaternion.FromToRotation(FaceDummy.transform.forward, Normal);

            Rotation = FaceDummy.transform.rotation;
        }
    }


    public MainObject MainObject;
    public List<Vertex> Vertices = new List<Vertex>();
    public List<Face> Faces = new List<Face>();

    public bool NeedsRedraw { get; private set; } = false;

    public CustomMesh(MainObject mainObject)
    {
        MainObject = mainObject;

        new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), this);
        new Vertex(new Vector3(0.5f, -0.5f, -0.5f), this);
        new Vertex(new Vector3(0.5f, 0.5f, -0.5f), this);
        new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), this);
        new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), this);
        new Vertex(new Vector3(0.5f, -0.5f, 0.5f), this);
        new Vertex(new Vector3(0.5f, 0.5f, 0.5f), this);
        new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), this);


        /*
        Faces.Add(new Face(this,new [] { vertices[0], vertices[1], vertices[2] }));
        Faces.Add(new Face(this, new[] { vertices[0], vertices[2], vertices[3] }));

        Faces.Add(new Face(this, new[] { vertices[1], vertices[5], vertices[6] }));
        Faces.Add(new Face(this, new[] { vertices[1], vertices[6], vertices[2] }));

        Faces.Add(new Face(this, new[] { vertices[5], vertices[4], vertices[7] }));
        Faces.Add(new Face(this, new[] { vertices[5], vertices[7], vertices[6] }));

        Faces.Add(new Face(this, new[] { vertices[4], vertices[0], vertices[3] }));
        Faces.Add(new Face(this, new[] { vertices[4], vertices[3], vertices[7] }));

        Faces.Add(new Face(this, new[] { vertices[4], vertices[5], vertices[1] }));
        Faces.Add(new Face(this, new[] { vertices[4], vertices[1], vertices[0] }));

        Faces.Add(new Face(this, new[] { vertices[3], vertices[2], vertices[6] }));
        Faces.Add(new Face(this, new[] { vertices[3], vertices[6], vertices[7] }));
        */

        new Face(this, new[] { Vertices[2], Vertices[1], Vertices[0] });
        new Face(this, new[] { Vertices[3], Vertices[2], Vertices[0] });

        new Face(this, new[] { Vertices[6], Vertices[5], Vertices[1] });
        new Face(this, new[] { Vertices[2], Vertices[6], Vertices[1] });

        new Face(this, new[] { Vertices[7], Vertices[4], Vertices[5] });
        new Face(this, new[] { Vertices[6], Vertices[7], Vertices[5] });

       new Face(this, new[] { Vertices[3], Vertices[0], Vertices[4] });
        new Face(this, new[] { Vertices[7], Vertices[3], Vertices[4] });

       new Face(this, new[] { Vertices[1], Vertices[5], Vertices[4] });
        new Face(this, new[] { Vertices[0], Vertices[1], Vertices[4] });

        new Face(this, new[] { Vertices[6], Vertices[2], Vertices[3] });
        new Face(this, new[] { Vertices[7], Vertices[6], Vertices[3] });
        
    }

    public Mesh ToMesh()
    {
        Mesh newMesh = MainObject.GetComponent<MeshFilter>().mesh;

        List<Vector3> vectors = new List<Vector3>();

        foreach (var vertex in Vertices)
        {
            vectors.Add(vertex.Position);
        }

        List<int> triangles = new List<int>();
        foreach (var face in Faces)
        {
            foreach (var vertex in face.Vertices)
            {
                triangles.Add(Vertices.IndexOf(vertex));
            }
        }

        newMesh.Clear();

        newMesh.vertices = vectors.ToArray();
        newMesh.triangles = triangles.ToArray();

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        //newMesh.RecalculateTangents();




        NeedsRedraw = false;
        return newMesh;
    }

}
