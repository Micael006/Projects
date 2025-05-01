using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RingMaster : ScriptableWizard
{
    public float wR = 10f; // WheelRadius
    public float wW = 1f; // WheelWidth
    public float wD = 1f; // WheelDepth
    public int approximation = 64; //ApproximationPower
    public Material material;
    public bool saveMesh = false;

    [MenuItem("GameObject/3D Object/Ring")]

    static void ShowConeWizard()
    {
        ScriptableWizard.DisplayWizard<WheelMaster>("Creating a ring", "Create");
    }

    void OnWizardCreate()
    {
        //List<Vector3> verticies = new List<Vector3>();
        //List<int> triangles = new List<int>();
        //List<Vector2> uvs = new List<Vector2>();

        // Тест соединений
        float angleStep = 2 * Mathf.PI / approximation;
        List<GameObject> parts = new List<GameObject>();
        for (int i = 0; i < approximation; i++)
        {
            List<Vector3> verticies = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            Vector3 A1 = new Vector3((wR + wW) * Mathf.Cos(i * angleStep), (wR + wW) * Mathf.Sin(i * angleStep), -wD);
            Vector3 B1 = new Vector3(wR * Mathf.Cos(i * angleStep), wR * Mathf.Sin(i * angleStep), -wD);
            Vector3 C1 = new Vector3(wR * Mathf.Cos(i * angleStep), wR * Mathf.Sin(i * angleStep), 0);
            Vector3 D1 = new Vector3((wR + wW) * Mathf.Cos(i * angleStep), (wR + wW) * Mathf.Sin(i * angleStep), 0);

            Vector3 A2 = new Vector3((wR + wW) * Mathf.Cos((i + 1) * angleStep), (wR + wW) * Mathf.Sin((i + 1) * angleStep), -wD);
            Vector3 B2 = new Vector3(wR * Mathf.Cos((i + 1) * angleStep), wR * Mathf.Sin((i + 1) * angleStep), -wD);
            Vector3 C2 = new Vector3(wR * Mathf.Cos((i + 1) * angleStep), wR * Mathf.Sin((i + 1) * angleStep), 0);
            Vector3 D2 = new Vector3((wR + wW) * Mathf.Cos((i + 1) * angleStep), (wR + wW) * Mathf.Sin((i + 1) * angleStep), 0);
            // Грань A1-B1-C1-D1
            //verticies.AddRange(new Vector3[] { A1, B1, C1, D1 });
            //uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            //triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });

            // Грань A1-B1-B2-A2
            verticies.AddRange(new Vector3[] { A1, A2, B2, B1 });
            uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });

            // Грань B1-C1-C2-B2
            verticies.AddRange(new Vector3[] { B1, B2, C2, C1 });
            uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });

            // Грань C1-D1-D2-C2
            verticies.AddRange(new Vector3[] { C1, C2, D2, D1 });
            uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });

            // Грань D1-A1-A2-D2
            verticies.AddRange(new Vector3[] { D1, D2, A2, A1 });
            uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });

            // Грань A2-D2-C2-B2
            //verticies.AddRange(new Vector3[] { A2, D2, C2, B2 });
            //uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            //triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });

            GameObject part = new GameObject();
            part.name = "part" + i.ToString();
            MeshFilter meshf = part.AddComponent<MeshFilter>();
            MeshRenderer mr = part.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.uv = uvs.ToArray();

            meshf.mesh = mesh;
            mr.material = material;

            part.AddComponent<Rigidbody>();
            part.GetComponent<Rigidbody>().useGravity = false;

            MeshCollider collider = part.AddComponent<MeshCollider>();
            //
            collider.sharedMesh = mesh;
            collider.convex = true;

            parts.Add(part);
        }

        GameObject ring = new GameObject();
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].transform.SetParent(ring.transform, false);
        }
    }
}
