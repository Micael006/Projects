using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class CombineObjects : EditorWindow
{
    private int count = 2;
    private List<GameObject> lists = new List<GameObject>();
    private string newObjectName = "NewGameObject";
    private bool SaveMesh = true;
    GameObject mainGameObject;
    [MenuItem("Window/Combine Gameobjects")]

    public static void Init()
    {
        CombineObjects coWind = GetWindow<CombineObjects>("Combine Gameobjects");
        DontDestroyOnLoad(coWind);
    }

    private void OnGUI()
    {
        using(var verticalArea = new EditorGUILayout.VerticalScope())
        {
            GUILayout.Label("The amount of objects");
            count = EditorGUILayout.IntField(count);
            GUILayout.Space(10);

            GUILayout.Label("Gameobjects to combine");
            if (lists.Count < count)
            {
                lists.AddRange(new GameObject[count - lists.Count]);
            }
            else if (lists.Count > count)
            {
                lists.RemoveRange(count - 1, lists.Count - count - 1);
            }
            for (int i = 0; i < count; i++)
            {
                lists[i] = (GameObject)EditorGUILayout.ObjectField(lists[i], typeof(GameObject), true);
            }
            GUILayout.Label("Main gameobject");
            mainGameObject = (GameObject)EditorGUILayout.ObjectField(mainGameObject, typeof(GameObject), true);
            GUILayout.Space(10);
            GUILayout.Label("The name of new object");
            newObjectName = EditorGUILayout.TextField(newObjectName);
            GUILayout.Space(10);
            SaveMesh = GUILayout.Toggle(SaveMesh, "Save mesh in file");
            GUILayout.Space(10);
            if (GUILayout.Button("Combine"))
            {
                GameObject gameObject = new GameObject();
                //
                Mesh[] collidersMeshes = new Mesh[count];
                //
                List<Vector3> verts = new List<Vector3>();
                List<int> triags = new List<int>();
                List<Vector2> uvs = new List<Vector2>();
                int triagplus = 0;
                for(int i = 0; i < count; i++)
                {
                    Vector3 local = lists[i].transform.position - mainGameObject.transform.position;
                    Quaternion rot = lists[i].transform.rotation;
                    Vector3 scale = lists[i].transform.localScale;
                    verts.AddRange(AddLocals(lists[i].GetComponent<MeshFilter>().mesh.vertices, local, rot, scale));
                    triags.AddRange(AddNumber(lists[i].GetComponent<MeshFilter>().mesh.triangles, triagplus));
                    uvs.AddRange(lists[i].GetComponent<MeshFilter>().mesh.uv);
                    //
                    collidersMeshes[i] = new Mesh();
                    collidersMeshes[i].vertices = (AddLocals(lists[i].GetComponent<MeshFilter>().mesh.vertices, local, rot, scale)).ToArray();
                    collidersMeshes[i].triangles = (AddNumber(lists[i].GetComponent<MeshFilter>().mesh.triangles, 0)).ToArray();
                    collidersMeshes[i].uv = (lists[i].GetComponent<MeshFilter>().mesh.uv).ToArray();
                    //
                    triagplus = verts.Count;
                }
                Mesh m = new Mesh();
                m.vertices = verts.ToArray();
                m.triangles = triags.ToArray();
                m.uv = uvs.ToArray();
                m.RecalculateNormals();
                if (SaveMesh)
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

                MeshFilter mf = gameObject.AddComponent<MeshFilter>();
                mf.mesh = m;
                if (UnityEditorInternal.ComponentUtility.CopyComponent(lists[0].GetComponent<MeshRenderer>()))
                {
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObject);
                }
                //Отдельно после меша копируем коллайдеры
                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                for (int i = 0; i < count; i++)
                {
                    MeshCollider collider = gameObject.AddComponent<MeshCollider>();
                    //
                    collider.sharedMesh = collidersMeshes[i];
                    collider.convex = true;
                    //
                }
                /*for (int i = 0; i < count; i++)
                {
                    Vector3 local = lists[i].transform.position - mainGameObject.transform.position;
                    Quaternion rot = lists[i].transform.rotation;
                    Vector3 scale = lists[i].transform.localScale;
     

                    if (UnityEditorInternal.ComponentUtility.CopyComponent(lists[i].GetComponent<Collider>()))
                    {
                        GameObject helper = new GameObject();
                        helper.transform.position = lists[i].transform.position;
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(helper);
                        helper.transform.rotation = rot;
                        Collider helperCol = helper.GetComponent<Collider>();
                        UnityEditorInternal.ComponentUtility.CopyComponent(helperCol);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObject);
                        DestroyImmediate(helper);
                    }
                    Collider col = gameObject.GetComponents<Collider>().Last();
                    if (col is BoxCollider)
                    {
                        ((BoxCollider)col).size = scale;
                        ((BoxCollider)col).center += local;
                        //Debug.Log("+box");
                    }
                    if (col is SphereCollider)
                    {
                        ((SphereCollider)col).radius = Mathf.Max(Mathf.Max(scale.x, scale.y), scale.z) / 2;
                        ((SphereCollider)col).center += local;
                        //Debug.Log("+sph");
                    }
                    if (col is CapsuleCollider)
                    {
                        ((CapsuleCollider)col).radius = Mathf.Max(scale.x, scale.z) / 2;
                        ((CapsuleCollider)col).height = 2 * scale.y;
                        ((CapsuleCollider)col).center += local;
                        //Debug.Log("+cap");
                    }
                }*/
            }
        }
    }

    private static Vector3[] AddLocals(Vector3[] arr, Vector3 loc)
    {
        Vector3[] res = new Vector3[arr.Length];
        for (int i = 0; i < res.Length;i++)
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