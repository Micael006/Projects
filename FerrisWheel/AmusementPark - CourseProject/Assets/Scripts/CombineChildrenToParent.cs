using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CombineChildrenToParent : EditorWindow
{
    private List<GameObject> children = new List<GameObject>();
    private string newObjectName = "New Game Object";
    private bool saveMesh = false;
    GameObject parent;

    [MenuItem("Window/Combine ChildrenToParent")]
    public static void Init()
    {
        CombineChildrenToParent coWind = GetWindow<CombineChildrenToParent>("Combine Parent object with it's children in a single object");
        DontDestroyOnLoad(coWind);
    }

    private void OnGUI()
    {
        using (var verticalArea = new EditorGUILayout.VerticalScope())
        {
            children.Clear();
            GUILayout.Label("Main gameObject");
            parent = (GameObject)EditorGUILayout.ObjectField(parent, typeof(GameObject), true);
            GUILayout.Space(10);
            GUILayout.Label("The name of new object");
            newObjectName = EditorGUILayout.TextField(newObjectName);
            GUILayout.Space(10);
            saveMesh = GUILayout.Toggle(saveMesh, "Save mesh in file");
            GUILayout.Space(10);

            if (GUILayout.Button("Combine"))
            {
                if (parent.transform.childCount == 0)
                {
                    Debug.Log("Given object doesn't have children");
                    GUIUtility.ExitGUI();
                    return;
                }

                int childCount = parent.transform.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    children.Add(parent.transform.GetChild(i).gameObject);
                }

                GameObject gameObject = new GameObject();
                //
                Mesh[] collidersMeshes = new Mesh[childCount];
                //
                List<Vector3> verts = new List<Vector3>();
                List<int> triags = new List<int>();
                List<Vector2> uvs = new List<Vector2>();
                int triagplus = 0;
                for (int i = 0; i < childCount; i++)
                {
                    Vector3 local = children[i].transform.position - parent.transform.position;
                    Quaternion rot = children[i].transform.rotation;
                    Vector3 scale = children[i].transform.localScale;
                    verts.AddRange(AddLocals(children[i].GetComponent<MeshFilter>().sharedMesh.vertices, local, rot, scale));
                    triags.AddRange(AddNumber(children[i].GetComponent<MeshFilter>().sharedMesh.triangles, triagplus));
                    uvs.AddRange(children[i].GetComponent<MeshFilter>().sharedMesh.uv);
                    //
                    collidersMeshes[i] = new Mesh();
                    collidersMeshes[i].vertices = (AddLocals(children[i].GetComponent<MeshFilter>().sharedMesh.vertices, local, rot, scale)).ToArray();
                    collidersMeshes[i].triangles = (AddNumber(children[i].GetComponent<MeshFilter>().sharedMesh.triangles, 0)).ToArray();
                    collidersMeshes[i].uv = (children[i].GetComponent<MeshFilter>().sharedMesh.uv).ToArray();
                    //
                    triagplus = verts.Count;
                }
                Mesh m = new Mesh();
                m.vertices = verts.ToArray();
                m.triangles = triags.ToArray();
                m.uv = uvs.ToArray();
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

                MeshFilter mf = gameObject.AddComponent<MeshFilter>();
                mf.mesh = m;
                if (UnityEditorInternal.ComponentUtility.CopyComponent(children[0].GetComponent<MeshRenderer>()))
                {
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObject);
                }
                //Отдельно после меша копируем коллайдеры
                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                for (int i = 0; i < childCount; i++)
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
