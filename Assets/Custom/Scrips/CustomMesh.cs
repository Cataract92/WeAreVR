using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CustomMesh
{
    public class Vertex
    {
        public Vector3 Position;
        public List<Face> Faces = new List<Face>();
        public List<Edge> Edges = new List<Edge>();
        public VertexDummy VertexDummy;
        public CustomMesh Mesh;

        public Vertex(Vector3 position, CustomMesh mesh, Face face = null, VertexDummy dummy = null)
        {
            Position = position;
            Mesh = mesh;

            if (dummy == null)
            {
                VertexDummy = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Vertex"))).GetComponent<VertexDummy>();
                VertexDummy.transform.parent = Mesh.MainObject.transform;
                VertexDummy.transform.localPosition = position;
            } else
            {
                VertexDummy = dummy;
                VertexDummy.Face.FaceDummy = null;
            }

            
            VertexDummy.Vertex = this;
            VertexDummy.Face = null;
            VertexDummy.GetComponent<MeshRenderer>().material = VertexDummy.VertexMaterial;

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

            Mesh.Vertices.Remove(this);

            List<Vertex> neighbours = new List<Vertex>();

            List<Face> faces = new List<Face>(Faces);

            foreach (var face in faces)
            {
                foreach (var vertex in face.Vertices)
                {
                    if (!neighbours.Contains(vertex) && vertex != this)
                        neighbours.Add(vertex);
                }
                face.Remove();
            }

            if (extendTriangulation(neighbours,0))
            {
                GameObject.Destroy(VertexDummy);
            } else
            {
                Debug.LogError("Triangulation failed. Your bad...");
            }

            Mesh.NeedsRedraw = true;
        }


        private bool extendTriangulation(List<Vertex> neighbours,int depth)
        {
            Debug.Log(depth);

            foreach(var v0 in neighbours)
            {
                foreach (var v1 in neighbours)
                {
                    if (v0 == v1)
                        continue;

                    foreach (var v2 in neighbours)
                    {
                        if (v0 == v2 || v1 == v2)
                            continue;

                        var f = new Face(Mesh, new[] { v0, v1, v2 });

                        if (!Mesh.isValid())
                        {
                            if (neighbours.Count > 3)
                            {
                                List<Vertex> remaining = new List<Vertex>(neighbours);
                                Vertex deletingVertex = null;
                                List<Vertex> tmp = new List<Vertex>();
                                List<Vertex> intersect = new List<Vertex>();

                                foreach(var tf in v0.Faces)
                                    foreach (var v in tf.Vertices)
                                        if (!tmp.Contains(v) && v0 != v)
                                            tmp.Add(v);

                                foreach (var v in remaining)
                                    if (tmp.Contains(v))
                                        intersect.Add(v);

                                if (intersect.Count == 2)
                                    deletingVertex = v0;

                                intersect.Clear();
                                tmp.Clear();


                                foreach (var tf in v1.Faces)
                                    foreach (var v in tf.Vertices)
                                        if (!tmp.Contains(v) && v1 != v)
                                            tmp.Add(v);

                                foreach (var v in remaining)
                                    if (tmp.Contains(v))
                                        intersect.Add(v);

                                if (intersect.Count == 2)
                                    deletingVertex = v1;

                                intersect.Clear();
                                tmp.Clear();

                                foreach (var tf in v2.Faces)
                                    foreach (var v in tf.Vertices)
                                        if (!tmp.Contains(v) && v2 != v)
                                            tmp.Add(v);

                                foreach (var v in remaining)
                                    if (tmp.Contains(v))
                                        intersect.Add(v);

                                if (intersect.Count == 2)
                                    deletingVertex = v2;

                                intersect.Clear();
                                tmp.Clear();

                                remaining.Remove(deletingVertex);

                                if (extendTriangulation(remaining, depth++))
                                    return true;

                            } else
                            {
                                f.Remove();
                                return false;
                            }
                        } else
                        {
                            return true;
                        }

                        f.Remove();
                    }
                }
            }

            return false;
        }

        public void Move(Vector3 NewPosition)
        {
            Mesh.NeedsRedraw = true;
        }
    }

    public class Face
    {
        public Vertex[] Vertices = new Vertex[3];
        public List<Edge> Edges = new List<Edge>();
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

            var list = new List<Edge>(Mesh.Edges);
            foreach (var edge in list)
            {
                if ((edge.Vertices[0] == vertices[0] || edge.Vertices[0] == vertices[1]) && (edge.Vertices[1] == vertices[0] || edge.Vertices[1] == vertices[1]))
                    Edges.Add(edge);
                

                if ((edge.Vertices[0] == vertices[1] || edge.Vertices[0] == vertices[2]) && (edge.Vertices[1] == vertices[1] || edge.Vertices[1] == vertices[2]))
                    Edges.Add(edge);

                if ((edge.Vertices[0] == vertices[2] || edge.Vertices[0] == vertices[0]) && (edge.Vertices[1] == vertices[2] || edge.Vertices[1] == vertices[0]))
                    Edges.Add(edge);
            }

            list = new List<Edge>(Edges);
            foreach (var edge in list)
            {
                if (!((edge.Vertices[0] == vertices[0] || edge.Vertices[0] == vertices[1]) && (edge.Vertices[1] == vertices[0] || edge.Vertices[1] == vertices[1])))
                    Edges.Add(new Edge(mesh, new[] { vertices[0], vertices[1] }));

                if (!((edge.Vertices[0] == vertices[1] || edge.Vertices[0] == vertices[2]) && (edge.Vertices[1] == vertices[1] || edge.Vertices[1] == vertices[2])))
                    Edges.Add(new Edge(mesh, new[] { vertices[1], vertices[2] }));

                if (!((edge.Vertices[0] == vertices[2] || edge.Vertices[0] == vertices[0]) && (edge.Vertices[1] == vertices[2] || edge.Vertices[1] == vertices[0])))
                    Edges.Add(new Edge(mesh, new[] { vertices[2], vertices[0] }));
            }

            if (Mesh.Edges.Count == 0 || Edges.Count == 0)
            {
                Edges.Add(new Edge(mesh, new[] { vertices[0], vertices[1] }));
                Edges.Add(new Edge(mesh, new[] { vertices[1], vertices[2] }));
                Edges.Add(new Edge(mesh, new[] { vertices[2], vertices[0] }));
            }

            foreach (var vertex in Vertices)
            {
                vertex.Faces.Add(this);
                InitialVectors.Add(vertex,new Vector3());
            }

            Mesh.Faces.Add(this);

            FaceDummy = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Vertex"))).GetComponent<VertexDummy>();
            FaceDummy.transform.parent = mesh.MainObject.transform;
            FaceDummy.Face = this;
            FaceDummy.GetComponent<MeshRenderer>().material = FaceDummy.FaceMaterial;

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
                InitialVectors[vertex] = Quaternion.Inverse(Mesh.MainObject.transform.rotation) * ( FaceDummy.transform.position - vertex.VertexDummy.transform.position);
            }

            Normal = Vector3.Cross(Vertices[1].VertexDummy.transform.position - Vertices[0].VertexDummy.transform.position, Vertices[2].VertexDummy.transform.position - Vertices[0].VertexDummy.transform.position);
            FaceDummy.transform.rotation *= Quaternion.FromToRotation(FaceDummy.transform.forward, Normal);

            Rotation = FaceDummy.transform.rotation;
        }
    }

    public class Edge
    {
        public Vertex[] Vertices = new Vertex[2];
        public CustomMesh Mesh;
        public VertexDummy EdgeDummy;
        public Vector3 Midpoint;

        public Dictionary<Vertex, Vector3> InitialVectors = new Dictionary<Vertex, Vector3>();
        public Quaternion Rotation = new Quaternion();

        public Edge(CustomMesh mesh, Vertex[] vertices)
        {
            Mesh = mesh;
            Vertices = vertices;
            foreach (var vertex in Vertices)
            {
                vertex.Edges.Add(this);
                InitialVectors.Add(vertex, new Vector3());
            }

           

            Mesh.Edges.Add(this);

            EdgeDummy = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Vertex"))).GetComponent<VertexDummy>();
            EdgeDummy.transform.parent = mesh.MainObject.transform;
            EdgeDummy.Edge = this;

            calculateDummyPosition();

            Mesh.NeedsRedraw = true;
        }

        public void Remove()
        {
            foreach (var vertex in Vertices)
            {
                vertex.Edges.Remove(this);
            }

            Mesh.Edges.Remove(this);

            Mesh.NeedsRedraw = true;
        }

        public void calculateDummyPosition()
        {
            EdgeDummy.transform.localPosition =
                (Vertices[0].Position + Vertices[1].Position) / 2f;

            foreach (var vertex in Vertices)
            {
                InitialVectors[vertex] = Quaternion.Inverse(Mesh.MainObject.transform.rotation) * (EdgeDummy.transform.position - vertex.VertexDummy.transform.position);
            }

            Midpoint = (Vertices[0].VertexDummy.transform.position + Vertices[1].VertexDummy.transform.position) / 2f;
            EdgeDummy.transform.rotation *= Quaternion.FromToRotation(EdgeDummy.transform.forward, Midpoint);
        }

    }


    public MainObject MainObject;
    public List<Vertex> Vertices = new List<Vertex>();
    public List<Edge> Edges = new List<Edge>();
    public List<Face> Faces = new List<Face>();

    public bool NeedsRedraw = false;

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

    List<Vertex> list = new List<Vertex>();
    public bool isValid()
    {

        foreach (var face1 in Faces)
        {
            bool contains = false;
            
            foreach (var face2 in Faces)
            {
                if (face1.Equals(face2))
                    continue;

                list.Clear();
                list.AddRange(face2.Vertices);
                if (list.Contains(face1.Vertices[0]) && list.Contains(face1.Vertices[1]))
                    contains = true;
            }
            if (!contains)
                return false;

            contains = false;
            foreach (var face2 in Faces)
            {
                if (face1.Equals(face2))
                    continue;

                list.Clear();
                list.AddRange(face2.Vertices);
                if (list.Contains(face1.Vertices[1]) && list.Contains(face1.Vertices[2]))
                    contains = true;
            }
            if (!contains)
                return false;

            contains = false;
            foreach (var face2 in Faces)
            {
                if (face1.Equals(face2))
                    continue;

                list.Clear();
                list.AddRange(face2.Vertices);
                if (list.Contains(face1.Vertices[2]) && list.Contains(face1.Vertices[0]))
                    contains = true;
            }
            if (!contains)
                return false;

        }
        return true;
    }

    public void changeFaceDirection()
    {
        var tmpfaces = new List<Face>(Faces);

        foreach(var face in tmpfaces)
        {
            var hands = GameObject.FindObjectsOfType<CustomHand>();

            foreach(var hand in hands)
            {
                if (hand.currentAttachedObject == face.FaceDummy)
                    continue;
            }

            var position = (face.Vertices[0].Position * 1.01f + face.Vertices[1].Position * 0.99f + face.Vertices[2].Position) / 3;

            var normal = Vector3.Cross(face.Vertices[1].Position - face.Vertices[0].Position, face.Vertices[2].Position - face.Vertices[0].Position);
            normal.Normalize();

            position += normal * 0.001f;

            List<Face> colliding = new List<Face>();
            var count = 0;
            foreach (var face2 in Faces)
            {
                if (Intersect(face2.Vertices[0].Position, face2.Vertices[1].Position, face2.Vertices[2].Position, new Ray(position, new Vector3(0, -1, 0))))
                {
                    colliding.Add(face2);
                    count++;
                }
                    
            }

            if (count % 2 != 0)
            {
                var vertices = new List<Vertex>(face.Vertices);
                vertices.Reverse();
                face.Remove();
                new Face(this, vertices.ToArray());
            }
        }
    }

    private bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
    {
        // Vectors from p1 to p2/p3 (edges)
        Vector3 e1, e2;

        Vector3 p, q, t;
        float det, invDet, u, v;


        //Find vectors for two edges sharing vertex/point p1
        e1 = p2 - p1;
        e2 = p3 - p1;

        // calculating determinant 
        p = Vector3.Cross(ray.direction, e2);

        //Calculate determinat
        det = Vector3.Dot(e1, p);

        //if determinant is near zero, ray lies in plane of triangle otherwise not
        if (det > -Mathf.Epsilon && det < Mathf.Epsilon) { return false; }
        invDet = 1.0f / det;

        //calculate distance from p1 to ray origin
        t = ray.origin - p1;

        //Calculate u parameter
        u = Vector3.Dot(t, p) * invDet;

        //Check for ray hit
        if (u < 0 || u > 1) { return false; }

        //Prepare to test v parameter
        q = Vector3.Cross(t, e1);

        //Calculate v parameter
        v = Vector3.Dot(ray.direction, q) * invDet;

        //Check for ray hit
        if (v < 0 || u + v > 1) { return false; }

        if ((Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon)
        {
            //ray does intersect
            return true;
        }

        // No hit at all
        return false;
    }

    public Mesh ToMesh()
    {
        Mesh newMesh = MainObject.GetComponent<MeshFilter>().mesh;

        List<Vector3> vectors = new List<Vector3>();

        changeFaceDirection();

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
        newMesh.RecalculateTangents();

        

        NeedsRedraw = false;
        return newMesh;
    }

}
