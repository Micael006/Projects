using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class FerrisWheelMaster : ScriptableWizard
{
    public static float cabinHeight = 3f; // Height of Ferris wheel's single cabin (affects Radius of rings)
    public float wX = 0.1f; // Thickness of Outer Ring of Ferris Wheel (Axe X)
    public float wZ = 0.1f; // Thickness of Outer Ring of Ferris Wheel (Axe Z)
    public float wD = 2f; // Distance between two rings (Axe Z)

    int approximation = 12; //ApproximationPower
    int cylinderApproximation = 32;

    public Material cylinderMaterial;
    public Material supportRingMaterial;
    public Material supportBeamMaterial;
    public Material connectionXMaterial;
    public Material connectionZMaterial;
    public Material generalRingMaterial;

    public bool saveMesh = false;

    [MenuItem("GameObject/3D Object/FerrisWheel")]
    static void ShowConeWizard()
    {
        ScriptableWizard.DisplayWizard<FerrisWheelMaster>("Creating a ferris wheel", "Create");
    }

    void OnWizardCreate()
    {
        float angleStep = 2 * Mathf.PI / approximation;
        float cylinderAngleStep = 2 * Mathf.PI / cylinderApproximation;
        List<GameObject> movingParts = new List<GameObject>();
        List<GameObject> staticParts = new List<GameObject>();
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        void AddPart(string objName, Material material, bool isMoving)
        {
            GameObject part = new GameObject();
            part.name = objName;
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
            if (isMoving)
            {
                movingParts.Add(part);
            }
            else
            {
                staticParts.Add(part);
            }
        }
        
        void AddRectPoly(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
        {
            verticies.AddRange(new Vector3[] { point1, point2, point3, point4 });
            uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            triangles.AddRange(new int[] { verticies.Count - 4, verticies.Count - 1, verticies.Count - 2, verticies.Count - 2, verticies.Count - 3, verticies.Count - 4 });
        }

        void AddTriagPoly(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            verticies.AddRange(new Vector3[] { point1, point2, point3 });
            uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1) });
            triangles.AddRange(new int[] { verticies.Count - 3, verticies.Count - 2, verticies.Count - 1 });
        }

        void ClearMesh()
        {
            verticies = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
        }

        // Creating Ferris wheel's center cylinder
        float cR = cabinHeight * 0.1f;
        float sRO = cR + 0f; // supportRingOffset = CylinderRadius + offset for motion of innerPart
        float wSX = 3 * wX;
        float cH = (wD + 4 * wZ + 2 * wSX);
        float centerZ = -(wD + 2 * wZ) / 2f;
        Vector3 posCenter = new Vector3(0, 0, centerZ + cH / 2f);
        Vector3 negCenter = new Vector3(0, 0, centerZ - cH / 2f);
        ClearMesh();

        for (int i = 0; i < cylinderApproximation; i++)
        {
            Vector3 A = new Vector3(cR * Mathf.Cos(i * cylinderAngleStep), cR * Mathf.Sin(i * cylinderAngleStep), posCenter.z);
            Vector3 B = new Vector3(A.x, A.y, negCenter.z);
            Vector3 C = new Vector3(cR * Mathf.Cos((i + 1) * cylinderAngleStep), cR * Mathf.Sin((i + 1) * cylinderAngleStep), negCenter.z);
            Vector3 D = new Vector3(C.x, C.y, posCenter.z);

            // Грань B-A-D-C
            AddRectPoly(B, A, D, C);
            // Лицевая грань
            AddTriagPoly(A, D, posCenter);
            // Задняя грань
            AddTriagPoly(C, B, negCenter);
        }

        AddPart("partCylinder", cylinderMaterial, true);

        // Creating supportingRings

        for (int i = 0; i < cylinderApproximation; i++)
        {
            ClearMesh();

            Vector3 A1 = new Vector3((sRO + wSX) * Mathf.Cos(i * cylinderAngleStep), (sRO + wSX) * Mathf.Sin(i * cylinderAngleStep), posCenter.z - wSX);
            Vector3 B1 = new Vector3(sRO * Mathf.Cos(i * cylinderAngleStep), sRO * Mathf.Sin(i * cylinderAngleStep), posCenter.z - wSX);
            Vector3 C1 = new Vector3(B1.x, B1.y, posCenter.z);
            Vector3 D1 = new Vector3(A1.x, A1.y, posCenter.z);

            Vector3 A2 = new Vector3((sRO + wSX) * Mathf.Cos((i + 1) * cylinderAngleStep), (sRO + wSX) * Mathf.Sin((i + 1) * cylinderAngleStep), posCenter.z - wSX);
            Vector3 B2 = new Vector3(sRO * Mathf.Cos((i + 1) * cylinderAngleStep), sRO * Mathf.Sin((i + 1) * cylinderAngleStep), posCenter.z - wSX);
            Vector3 C2 = new Vector3(B2.x, B2.y, posCenter.z);
            Vector3 D2 = new Vector3(A2.x, A2.y, posCenter.z);
            // Грань A1-B1-C1-D1
            //AddRectPoly(A1, B1, C1, D1);
            // Грань A1-A2-B2-B1
            AddRectPoly(A1, A2, B2, B1);
            // Грань B1-B2-C2-C1
            AddRectPoly(B1, B2, C2, C1);
            // Грань C1-C2-D2-D1
            AddRectPoly(C1, C2, D2, D1);
            // Грань D1-D2-A2-A1
            AddRectPoly(D1, D2, A2, A1);
            // Грань A2-D2-C2-B2
            //AddRectPoly(A2, D2, C2, B2);
            
            AddPart("SupportRing1_" + i.ToString(), supportRingMaterial, false);

            ClearMesh();

            A1.z = negCenter.z;
            B1.z = A1.z;
            A2.z = A1.z;
            B2.z = A1.z;

            C1.z = negCenter.z + wSX;
            D1.z = C1.z;
            C2.z = C1.z;
            D2.z = C1.z;

            // Грань A1-B1-C1-D1
            //AddRectPoly(A1, B1, C1, D1);
            // Грань A1-A2-B2-B1
            AddRectPoly(A1, A2, B2, B1);
            // Грань B1-B2-C2-C1
            AddRectPoly(B1, B2, C2, C1);
            // Грань C1-C2-D2-D1
            AddRectPoly(C1, C2, D2, D1);
            // Грань D1-D2-A2-A1
            AddRectPoly(D1, D2, A2, A1);
            // Грань A2-D2-C2-B2
            //AddRectPoly(A2, D2, C2, B2);
            
            AddPart("SupportRing2_" + i.ToString(), supportRingMaterial, false);
        }


        // Creating supportingBeams
        float alpha = Mathf.PI / 6;
        float floorDist = cabinHeight * 6f;
        int numOfRings = 2;
        float ringSize = ((floorDist - cabinHeight) / numOfRings);
        Vector2 direction = new Vector2(Mathf.Cos(-Mathf.PI / 2f - alpha), Mathf.Sin(-Mathf.PI / 2f - alpha));
        for(int i = cylinderApproximation / 2 + 1; i < 3 * cylinderApproximation / 4; i++)
        {
            ClearMesh();

            Vector3 B1 = new Vector3((sRO + wSX) * Mathf.Cos(i * cylinderAngleStep), (sRO + wSX) * Mathf.Sin(i * cylinderAngleStep), posCenter.z - wSX);
            Vector3 C1 = new Vector3((sRO + wSX) * Mathf.Cos(i * cylinderAngleStep), (sRO + wSX) * Mathf.Sin(i * cylinderAngleStep), posCenter.z);

            Vector2 newXY1 = new Vector2(B1.x, B1.y) + ((floorDist + B1.y) / Mathf.Cos(alpha)) * direction;

            Vector3 A1 = new Vector3(newXY1.x, newXY1.y, B1.z);
            Vector3 D1 = new Vector3(newXY1.x, newXY1.y, C1.z);

            Vector3 B2 = new Vector3((sRO + wSX) * Mathf.Cos((i + 1) * cylinderAngleStep), (sRO + wSX) * Mathf.Sin((i + 1) * cylinderAngleStep), posCenter.z - wSX);
            Vector3 C2 = new Vector3((sRO + wSX) * Mathf.Cos((i + 1) * cylinderAngleStep), (sRO + wSX) * Mathf.Sin((i + 1) * cylinderAngleStep), posCenter.z);

            Vector2 newXY2 = new Vector2(B2.x, B2.y) + ((floorDist + B2.y) / Mathf.Cos(alpha)) * direction;

            Vector3 A2 = new Vector3(newXY2.x, newXY2.y, B2.z);
            Vector3 D2 = new Vector3(newXY2.x, newXY2.y, C2.z);

            // Грань A1-B1-C1-D1
            if(i == cylinderApproximation / 2 + 1)
            {
                AddRectPoly(A1, B1, C1, D1);
            }
            // Грань A1-A2-B2-B1
            AddRectPoly(A1, A2, B2, B1);
            // Грань B1-B2-C2-C1
            //AddRectPoly(B1, B2, C2, C1);
            // Грань C1-C2-D2-D1
            AddRectPoly(C1, C2, D2, D1);
            // Грань D1-A1-A2-D2
            AddRectPoly(D1, D2, A2, A1);
            // Грань A2-D2-C2-B2
            if(i == 3 * cylinderApproximation / 4 - 1)
            {
                AddRectPoly(A2, D2, C2, B2);
            }

            AddPart("partBeam1_" + (i - cylinderApproximation / 2).ToString(), supportBeamMaterial, false);

            ClearMesh();

            A1.z = negCenter.z;
            B1.z = A1.z;
            A2.z = A1.z;
            B2.z = A1.z;

            C1.z = negCenter.z + wSX;
            D1.z = C1.z;
            C2.z = C1.z;
            D2.z = C1.z;

            // Грань A1-B1-C1-D1
            if (i == cylinderApproximation / 2 + 1)
            {
                AddRectPoly(A1, B1, C1, D1);
            }
            // Грань A1-A2-B2-B1
            AddRectPoly(A1, A2, B2, B1);
            // Грань B1-B2-C2-C1
            //AddRectPoly(B1, B2, C2, C1);
            // Грань C1-C2-D2-D1
            AddRectPoly(C1, C2, D2, D1);
            // Грань D1-A1-A2-D2
            AddRectPoly(D1, D2, A2, A1);
            // Грань A2-D2-C2-B2
            if (i == 3 * cylinderApproximation / 4 - 1)
            {
                AddRectPoly(A2, D2, C2, B2);
            }

            AddPart("partBeam3_" + (i - cylinderApproximation / 2).ToString(), supportBeamMaterial, false);

            ClearMesh();

            A1 = new Vector3(-newXY2.x, newXY2.y, negCenter.z + wSX);
            A2 = new Vector3(-newXY1.x, newXY1.y, A1.z);
            D1 = new Vector3(A1.x, A1.y, negCenter.z);
            D2 = new Vector3(A2.x, A2.y, D1.z);

            B1 = new Vector3(-(sRO + wSX) * Mathf.Cos((i + 1) * cylinderAngleStep), (sRO + wSX) * Mathf.Sin((i + 1) * cylinderAngleStep), A1.z);
            B2 = new Vector3(-(sRO + wSX) * Mathf.Cos(i * cylinderAngleStep), (sRO + wSX) * Mathf.Sin(i * cylinderAngleStep), A1.z);
            C1 = new Vector3(B1.x, B1.y, D1.z);
            C2 = new Vector3(B2.x, B2.y, D1.z);

            // Грань A1-B1-C1-D1
            if (i == 3 * cylinderApproximation / 4 - 1)
            {
                AddRectPoly(A1, D1, C1, B1);
            }
            // Грань A1-A2-B2-B1
            AddRectPoly(A1, B1, B2, A2);
            // Грань B1-B2-C2-C1
            //AddRectPoly(B1, C1, C2, B2);
            // Грань C1-C2-D2-D1
            AddRectPoly(C1, D1, D2, C2);
            // Грань D1-A1-A2-D2
            AddRectPoly(D1, A1, A2, D2);
            // Грань A2-D2-C2-B2
            if (i == cylinderApproximation / 2 + 1)
            {
                AddRectPoly(A2, B2, C2, D2);
            }   

            AddPart("partBeam4_" + (i - cylinderApproximation / 2).ToString(), supportBeamMaterial, false);

            ClearMesh();

            A1.z = posCenter.z;
            A2.z = A1.z;
            B1.z = A1.z;
            B2.z = A1.z;

            C1.z = posCenter.z - wSX;
            C2.z = C1.z;
            D1.z = C1.z;
            D2.z = C1.z;

            // Грань A1-B1-C1-D1
            if (i == 3 * cylinderApproximation / 4 - 1)
            {
                AddRectPoly(A1, D1, C1, B1);
            }
            // Грань A1-A2-B2-B1
            AddRectPoly(A1, B1, B2, A2);
            // Грань B1-B2-C2-C1
            //AddRectPoly(B1, C1, C2, B2);
            // Грань C1-C2-D2-D1
            AddRectPoly(C1, D1, D2, C2);
            // Грань D1-A1-A2-D2
            AddRectPoly(D1, A1, A2, D2);
            // Грань A2-D2-C2-B2
            if (i == cylinderApproximation / 2 + 1)
            {
                AddRectPoly(A2, B2, C2, D2);
            }

            AddPart("partBeam2_" + (i - cylinderApproximation / 2).ToString(), supportBeamMaterial, false);
        }

        // Creating generalRings and connectionsZ
        for (int curR = 0; curR <= numOfRings; curR++)
        {
            //Creating generalRings
            float wR = Mathf.Max(curR * ringSize, cR);
            for (int i = 0; i < approximation; i++)
            {
                // Creating first ring
                ClearMesh();

                Vector3 A1 = new Vector3((wR + wX) * Mathf.Cos(i * angleStep), (wR + wX) * Mathf.Sin(i * angleStep), -wZ);
                Vector3 B1 = new Vector3(wR * Mathf.Cos(i * angleStep), wR * Mathf.Sin(i * angleStep), -wZ);
                Vector3 C1 = new Vector3(B1.x, B1.y, 0);
                Vector3 D1 = new Vector3(A1.x, A1.y, 0);

                Vector3 A2 = new Vector3((wR + wX) * Mathf.Cos((i + 1) * angleStep), (wR + wX) * Mathf.Sin((i + 1) * angleStep), -wZ);
                Vector3 B2 = new Vector3(wR * Mathf.Cos((i + 1) * angleStep), wR * Mathf.Sin((i + 1) * angleStep), -wZ);
                Vector3 C2 = new Vector3(B2.x, B2.y, 0);
                Vector3 D2 = new Vector3(A2.x, A2.y, 0);

                // Грань A1-B1-C1-D1
                //AddRectPoly(A1, B1, C1, D1);
                // Грань A1-A2-B2-B1
                AddRectPoly(A1, A2, B2, B1);
                // Грань B1-B2-C2-C1
                AddRectPoly(B1, B2, C2, C1);
                // Грань C1-C2-D2-D1
                AddRectPoly(C1, C2, D2, D1);
                // Грань D1-D2-A2-A1
                AddRectPoly(D1, D2, A2, A1);
                // Грань A2-D2-C2-B2
                //AddRectPoly(A2, D2, C2, B2);

                AddPart("generalRing1_" + curR.ToString() + "_" + i.ToString(), generalRingMaterial, true);

                // Creating second ring
                ClearMesh();

                A1.z = -(wD + 2 * wZ);
                B1.z = A1.z;
                A2.z = A1.z;
                B2.z = A1.z;

                C1.z = -(wD + wZ);
                D1.z = C1.z;
                C2.z = C1.z;
                D2.z = C1.z;

                // Грань A1-B1-C1-D1
                //AddRectPoly(A1, B1, C1, D1);
                // Грань A1-A2-B2-B1
                AddRectPoly(A1, A2, B2, B1);
                // Грань B1-B2-C2-C1
                AddRectPoly(B1, B2, C2, C1);
                // Грань C1-C2-D2-D1
                AddRectPoly(C1, C2, D2, D1);
                // Грань D1-D2-A2-A1
                AddRectPoly(D1, D2, A2, A1);
                // Грань A2-D2-C2-B2
                //AddRectPoly(A2, D2, C2, B2);

                AddPart("generalRing2_" + curR.ToString() + "_" + i.ToString(), generalRingMaterial, true);
            }

            // Creating rings' connections (between rings of the same radius, different Z)
            for (int i = 0; i < approximation; i++)
            {
                ClearMesh();

                Vector3 center = new Vector3((wR + (wX / 2f)) * Mathf.Cos(i * angleStep), (wR + (wX / 2f)) * Mathf.Sin(i * angleStep), -(wD + 2 * wZ) / 2f);

                Vector3 A1 = new Vector3(center.x + wX / 4f, center.y + wX / 4f, center.z + (wD / 2f));
                Vector3 C1 = new Vector3(center.x - wX / 4f, center.y - wX / 4f, A1.z);
                Vector3 B1 = new Vector3(A1.x, C1.y, A1.z);
                Vector3 D1 = new Vector3(C1.x, A1.y, A1.z);

                Vector3 A2 = new Vector3(A1.x, A1.y, center.z - (wD / 2f));
                Vector3 B2 = new Vector3(A1.x, C1.y, A2.z);
                Vector3 C2 = new Vector3(C1.x, C1.y, A2.z);
                Vector3 D2 = new Vector3(C1.x, A1.y, A2.z);

                // Грань A1-B1-C1-D1
                //AddRectPoly(A1, B1, C1, D1);
                // Грань A1-A2-B2-B1
                AddRectPoly(A1, A2, B2, B1);
                // Грань B1-B2-C2-C1
                AddRectPoly(B1, B2, C2, C1);
                // Грань C1-C2-D2-D1
                AddRectPoly(C1, C2, D2, D1);
                // Грань D1-A1-A2-D2
                AddRectPoly(D1, D2, A2, A1);
                // Грань A2-D2-C2-B2
                //AddRectPoly(A2, D2, C2, B2);
                
                AddPart("connectionZ_" + curR.ToString() + "_" + i.ToString(), connectionZMaterial, true);
            }
        }

        // Creating rings' connections (different radius, same Z)
        for (int curR = 0; curR < numOfRings; curR++)
        {
            float wR1 = Mathf.Max(curR * ringSize, cR);
            float wR2 = (curR + 1) * ringSize;
            for(int i = 0; i < approximation; i++)
            {
                ClearMesh();

                Vector3 center1 = new Vector3((wR1 + wX) * Mathf.Cos(i * angleStep), (wR1 + wX) * Mathf.Sin(i * angleStep), -wZ / 2f);
                Vector3 center2 = new Vector3(wR2 * Mathf.Cos(i * angleStep), wR2 * Mathf.Sin(i * angleStep), center1.z);

                Vector2 p1 = new Vector2(cabinHeight * Mathf.Cos((i - 1) * angleStep), cabinHeight * Mathf.Sin((i - 1) * angleStep));
                Vector2 p2 = new Vector2(cabinHeight * Mathf.Cos(i * angleStep), cabinHeight * Mathf.Sin(i * angleStep));
                Vector2 p3 = new Vector2(cabinHeight * Mathf.Cos((i + 1) * angleStep), cabinHeight * Mathf.Sin((i + 1) * angleStep));

                Vector3 A11 = new Vector3(center1.x + (p1 - p2).x * wX / 2f, center1.y + (p1 - p2).y * wX / 2f, center1.z + wZ / 2f);
                Vector3 B11 = new Vector3(center1.x, center1.y, A11.z);
                Vector3 C11 = new Vector3(center1.x + (p3 - p2).x * wX / 2f, center1.y + (p3 - p2).y * wX / 2f, A11.z);
                Vector3 D11 = new Vector3(C11.x, C11.y, center1.z - wZ / 2f);
                Vector3 E11 = new Vector3(center1.x, center1.y, D11.z);
                Vector3 F11 = new Vector3(A11.x, A11.y, D11.z);

                Vector3 A21 = new Vector3(center2.x + (p1 - p2).x * wX / 2f, center2.y + (p1 - p2).y * wX / 2f, center2.z + wZ / 2f);
                Vector3 B21 = new Vector3(center2.x, center2.y, A21.z);
                Vector3 C21 = new Vector3(center2.x + (p3 - p2).x * wX / 2f, center2.y + (p3 - p2).y * wX / 2f, A21.z);
                Vector3 D21 = new Vector3(C21.x, C21.y, center2.z - wZ / 2f);
                Vector3 E21 = new Vector3(center2.x, center2.y, D21.z);
                Vector3 F21 = new Vector3(A21.x, A21.y, D21.z);

                Vector3 A12 = new Vector3(A11.x, A11.y, center1.z - (wD + wZ) + wZ / 2f);
                Vector3 B12 = new Vector3(center1.x, center1.y, A12.z);
                Vector3 C12 = new Vector3(C11.x, C11.y, A12.z);
                Vector3 D12 = new Vector3(C11.x, C11.y, center1.z - (wD + wZ) - wZ / 2f);
                Vector3 E12 = new Vector3(center1.x, center1.y, D12.z);
                Vector3 F12 = new Vector3(A11.x, A11.y, D12.z);

                Vector3 A22 = new Vector3(A21.x, A21.y, center2.z - (wD + wZ) + wZ / 2f);
                Vector3 B22 = new Vector3(center2.x, center2.y, A22.z);
                Vector3 C22 = new Vector3(C21.x, C21.y, A22.z);
                Vector3 D22 = new Vector3(C21.x, C21.y, center2.z - (wD + wZ) - wZ / 2f);
                Vector3 E22 = new Vector3(center2.x, center2.y, D22.z);
                Vector3 F22 = new Vector3(A21.x, A21.y, D22.z);

                // Грань A11-F11-E11-B11
                //AddRectPoly(A11, F11, E11, B11);
                // Грань A11-B11-B21-A21
                AddRectPoly(A11, B11, B21, A21);
                // Грань B11-E11-E21-B21
                //AddRectPoly(B11, E11, E21, B21);
                // Грань E11-F11-F21-E21
                AddRectPoly(E11, F11, F21, E21);
                // Грань F11-A11-A21-F21
                AddRectPoly(F11, A11, A21, F21);
                // Грань A21-B21-E21-F21
                //AddRectPoly(A21, B21, E21, F21);
                
                AddPart("partLowerConnectionX1_" + curR.ToString() + "-" + (curR + 1).ToString() + "_" + i.ToString(), connectionXMaterial, true);

                ClearMesh();

                // Грань B11-C11-D11-E11
                //AddRectPoly(B11, C11, D11, E11);
                // Грань B11-C11-C21-B21
                AddRectPoly(B11, C11, C21, B21);
                // Грань C11-D11-D21-C21
                AddRectPoly(C11, D11, D21, C21);
                // Грань D11-E11-E21-D21
                AddRectPoly(D11, E11, E21, D21);
                // Грань E11-B11-B21-E21
                //AddRectPoly(E11, B11, B21, E21);
                // Грань B21-C21-D21-E21
                //AddRectPoly(B21, C21, D21, E21);
                
                AddPart("partUpperConnectionX1_" + curR.ToString() + "-" + (curR + 1).ToString() + "_" + i.ToString(), connectionXMaterial, true);

                ClearMesh();

                // Грань A12-F12-E12-B12
                //AddRectPoly(A12, F12, E12, B12);
                // Грань A12-B12-B22-A22
                AddRectPoly(A12, B12, B22, A22);
                // Грань B12-E12-E22-B22
                //AddRectPoly(B12, E12, E22, B22);
                // Грань E12-F12-F22-E22
                AddRectPoly(E12, F12, F22, E22);
                // Грань F12-A12-A22-F22
                AddRectPoly(F12, A12, A22, F22);
                // Грань A22-B22-E22-F22
                //AddRectPoly(A22, B22, E22, F22);
                
                AddPart("partLowerConnectionX2_" + curR.ToString() + "-" + (curR + 1).ToString() + "_" + i.ToString(), connectionXMaterial, true);

                ClearMesh();

                // Грань B12-C12-D12-E12
                //AddRectPoly(B12, C12, D12, E12);
                // Грань B12-C12-C22-B22
                AddRectPoly(B12, C12, C22, B22);
                // Грань C12-D12-D22-C22
                AddRectPoly(C12, D12, D22, C22);
                // Грань D12-E12-E22-D22
                AddRectPoly(D12, E12, E22, D22);
                // Грань E12-B12-B22-E22
                //AddRectPoly(E12, B12, B22, E22);
                // Грань B22-C22-D22-E22
                //AddRectPoly(B22, C22, D22, E22);

                AddPart("partUpperConnectionX2_" + curR.ToString() + "-" + (curR + 1).ToString() + "_" + i.ToString(), connectionXMaterial, true);
            }
        }

        // Connecting everything
        GameObject ferrisWheel = new GameObject();
        ferrisWheel.name = "FerrisWheel";
        for (int i = 0; i < movingParts.Count; i++)
        {
            movingParts[i].transform.SetParent(ferrisWheel.transform, false);
        }

        GameObject supportingBeams = new GameObject();
        supportingBeams.name = "SupportingBeams";
        for (int i = 0; i < staticParts.Count; i++)
        {
            staticParts[i].transform.SetParent(supportingBeams.transform, false);
        }

        /*GameObject test = new GameObject();
        test.name = "Test";
        MeshFilter testMf = test.AddComponent<MeshFilter>();
        testMf.mesh = CombineMeshes(movingParts);*/
    }
}
