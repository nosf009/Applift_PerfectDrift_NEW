using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using System.Linq;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class ProceduralMeshMaker : MonoBehaviour
{
    //PUBLIC
    private int meshSize = 1;
    private float width = 1f;
    private float height = 1f;
    //PRIVATE
    private MeshFilter mF;
    private Mesh mesh;
    private Vector3[] verts;
    //private Vector2[] vertsUV;
    private int[] triAs;

    public GameObject spherePart;

    // Use this for initialization
    void Start()
    {
        mF = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mF.mesh = mesh;
        mesh.name = "CustomPlane";

        //Assign default material
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Diffuse"));

        //Create Mesh Resources
        //CreateVerts();
       // CreateTrias();
    }

    protected virtual void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    protected virtual void OnDisable()
    {
        // Unhook the events
        LeanTouch.OnFingerUp -= OnFingerUp;
    }

    void OnFingerUp(LeanFinger finger)
    {
        var old = FindObjectsOfType<MeshPart>();
        foreach (var o in old)
        {
            Destroy(o.gameObject);
        }
        DrawMesh();
    }

    public void DrawMesh()
    {
        LineRenderer lr = FindObjectOfType<LineRenderer>();
        //verts = new Vector3[lr.positionCount];
        //lr.GetPositions(verts);
        //foreach (Vector3 v in verts)
        //{
        //    Debug.Log(v);
        //}
        //CreateVerts();
        //CreateTrias();
        lr.BakeMesh(mesh, true);
        AdjustMesh();
    }

    void AdjustMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        mesh.GetVertices(verts);
        for (int i = 0; i < verts.Count; ++i)
        {
            verts[i] = new Vector3(verts[i].x, verts[i].y, verts[i].z);
            if (i % 2 != 0)
            {
                GameObject sph = Instantiate(spherePart, this.transform);
                sph.transform.position = verts[i];
            }
        }
        //mesh.SetVertices(verts);
        GetComponent<MeshRenderer>().enabled = false;
    }

    void CreateVerts()
    {
        //Set verts[] size = equal to the needed number of verts (2 times the x and y of how ever many squares there will be <meshSize>)
        verts = new Vector3[meshSize * 2 + meshSize * 2];

        //Keep track of current vert
        int curVert = 0;

        //Create Verts based on the specifed meshSize
        for (int x = 0; x < meshSize * 2; x++)
        {
            for (int z = 0; z < meshSize * 2; z++)
            {
                float xf = width;
                float zf = height;
                verts[curVert] = new Vector3(xf * x, 0f, zf * z);
                curVert++;
            }
        }
        mesh.vertices = verts;
        for (int v = 0; v < verts.Length; v++)
        {
            Debug.Log("Vert " + v + ": " + verts[v].x + ", " + verts[v].z);
        }
    }

    void CreateTrias()
    {
        //Set triAs[] to the total number of triangle points needed (6 times how ever many squares there will be <meshSize>)
        triAs = new int[meshSize * 6];

        //Keep track of seet points
        int triPoints = 0;
        int triFocus = 0;

        //Set every triangle point
        for (int t = 0; t < triAs.Length; t++)
        {
            switch (triPoints)
            {
                case 0:
                    triAs[t] = triFocus + meshSize * 2;
                    triPoints++;
                    Debug.Log("case 0: " + triAs[t]);
                    break;
                case 1:
                    triAs[t] = triFocus + 1;
                    triPoints++;
                    Debug.Log("case 1: " + triAs[t]);
                    break;
                case 2:
                    triAs[t] = triFocus;
                    triPoints++;
                    Debug.Log("case 2: " + triAs[t]);
                    break;
                case 3:
                    triAs[t] = triFocus + 1;
                    triPoints++;
                    Debug.Log("case 3: " + triAs[t]);
                    break;
                case 4:
                    triAs[t] = triFocus + meshSize * 2;
                    triPoints++;
                    Debug.Log("case 4: " + triAs[t]);
                    break;
                case 5:
                    triAs[t] = triFocus + meshSize * 2 + 1;
                    triPoints = 0;
                    triFocus += 2;
                    Debug.Log("case 5: " + triAs[t]);
                    break;
                default:
                    triPoints = 0;
                    triFocus += 2;
                    break;
            }
        }
        mesh.triangles = triAs;
    }
}