using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class WheelMaster : ScriptableWizard
{
    public float wR = 10f; // WheelRadius
    public float wW = 1f; // WheelWidth
    public float wD = 1f; // WheelDepth
    public int approximation = 64; //ApproximationPower
    public Material material;
    public bool saveMesh = false;

    [MenuItem("GameObject/3D Object/Wheel")]

    static void ShowConeWizard()
    {
        ScriptableWizard.DisplayWizard<WheelMaster>("Creating a wheel", "Create");
    }

    void OnWizardCreate()
    {
        //List<Vector3> verticies = new List<Vector3>();
        //List<int> triangles = new List<int>();
        //List<Vector2> uvs = new List<Vector2>();

        // Тест соединений
        float angleStep = 2 * Mathf.PI / approximation;
        List<GameObject> parts = new List<GameObject>();
        for(int i = 0; i < approximation; i++)
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

        GameObject wheel = new GameObject();
        for(int i = 0; i < parts.Count; i++)
        {
            parts[i].transform.SetParent(wheel.transform, false);
        }

        /*GameObject wheel = new GameObject();
        Mesh[] collidersMeshes = new Mesh[parts.Count];

        List<Vector3> verts = new List<Vector3>();
        List<int> triags = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        int triagplus = 0;

        for(int i = 0; i < parts.Count; i++)
        {
            Vector3 local = parts[i].transform.position;
            Quaternion rot = parts[i].transform.rotation;
            Vector3 scale = parts[i].transform.localScale;
            verts.AddRange(AddLocals(parts[i].GetComponent<MeshFilter>().mesh.vertices, local, rot, scale));
            triags.AddRange(AddNumber(parts[i].GetComponent<MeshFilter>().mesh.triangles, triagplus));
            uv.AddRange(parts[i].GetComponent<MeshFilter>().mesh.uv);
            //
            collidersMeshes[i] = new Mesh();
            collidersMeshes[i].vertices = AddLocals(parts[i].GetComponent<MeshFilter>().mesh.vertices, local, rot, scale).ToArray();
            collidersMeshes[i].triangles = (AddNumber(parts[i].GetComponent<MeshFilter>().mesh.triangles, 0)).ToArray();
            collidersMeshes[i].uv = (parts[i].GetComponent<MeshFilter>().mesh.uv).ToArray();
            //
            triagplus = verts.Count;
        }

        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.triangles = triags.ToArray();
        m.uv = uv.ToArray();
        m.RecalculateNormals();
        if (saveMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", "", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                Debug.Log(path);
                Mesh targetMesh = m;
                AssetDatabase.CreateAsset(targetMesh, path);
                AssetDatabase.SaveAssets();
            }
        }

        MeshFilter mf = wheel.AddComponent<MeshFilter>();
        mf.mesh = m;
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(wheel);

        //Отдельно после меша копируем коллайдеры
        wheel.AddComponent<Rigidbody>();
        wheel.GetComponent<Rigidbody>().useGravity = false;
        for (int i = 0; i < parts.Count; i++)
        {
            MeshCollider collider = wheel.AddComponent<MeshCollider>();
            //
            collider.sharedMesh = collidersMeshes[i];
            collider.convex = true;
            //
        }*/



        /*GameObject wheel = new GameObject();
        MeshFilter mf = wheel.AddComponent<MeshFilter>();
        MeshRenderer mr = wheel.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs.ToArray();

        mf.mesh = mesh;
        mr.material = material;

        wheel.AddComponent<Rigidbody>();
        wheel.GetComponent<Rigidbody>().useGravity = false;

        MeshCollider collider = wheel.AddComponent<MeshCollider>();
        //
        collider.sharedMesh = mesh;
        collider.convex = true;

        if (saveMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", "", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                Mesh targetMesh = mesh;
                AssetDatabase.CreateAsset(targetMesh, path);
                AssetDatabase.SaveAssets();
            }
        }*/
    }

    private static Vector3[] AddLocals(Vector3[] arr, Vector3 loc)
    {
        Vector3[] res = new Vector3[arr.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = arr[i] + loc;
        }
        return res;
    }

    private static Vector3[] AddLocals(Vector3[] arr, Vector3 loc, Quaternion rot, Vector3 scl)
    {
        Vector3[] res = new Vector3[arr.Length];
        for (int i = 0; i < res.Length; i++)
        {
            Vector3 cur_vertice = arr[i];
            res[i] = rot * new Vector3(cur_vertice[0] * scl[0], cur_vertice[1] * scl[1], cur_vertice[2] * scl[2]) + loc;

        }
        return res;
    }

    private static int[] AddNumber(int[] arr, int loc)
    {
        int[] res = new int[arr.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = arr[i] + loc;
        }
        return res;
    }
}
