#if UNITY_EDITOR
#pragma warning disable CS0618

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MotionPath : MonoBehaviour
{
    [HideInInspector] public Animator Animator;

    [HideInInspector] public float StartFrame = 0;
    [HideInInspector] public float EndFrame = 60;
    [HideInInspector] public float AnimationSlider;


    [HideInInspector] public AnimationClip[] AnimationClips;
    [HideInInspector] public string[] AniClipsName;
    [HideInInspector] public int SelectAniClip;
    [HideInInspector] public string PlayStateName; //State name to play


    /////////////////////////////////////////
    ////////        Path Info        ////////
    /////////////////////////////////////////
    [HideInInspector] public List<PathInfoData> PathInfo = new List<PathInfoData>(2);
    [System.Serializable]
    public class PathInfoData
    {
        [HideInInspector] public GameObject TargetObject;
        //Path Info 1
        [HideInInspector] public Vector3[] PathPos = new Vector3[0]; //Final position data actually written

        [HideInInspector] public int PathFrame = 120;

        [HideInInspector] public bool PathViewerSetting = false;
        [HideInInspector] public bool AutoUpdate = true;
        [HideInInspector] public Color PathColor = GetRandomColor_HighSaturation();
        [HideInInspector] public float PathWidth = 2;

        //Calculation variables to calculate the vertex average
        [HideInInspector] public bool Vertex_AutoUpdate = true; //Auto update
        public Vector3[] VertexPos = new Vector3[0]; //Vertex average position
        [HideInInspector] public int Vertex_Detail = 10; //Detail value
        [HideInInspector] public float Vertex_Distance = 0.25f; //Vertex distance value

        [HideInInspector] public int EditVertIdx = int.MaxValue; //Ability to adjust vertex position
        [HideInInspector] public bool EditMode = false; //Vertex correction mode
    }

    ////////////////////////////////////////////
    ////////       Generate mesh        ////////
    ////////////////////////////////////////////
    [HideInInspector] public MeshFilter MeshFilter;
    [HideInInspector] public MeshRenderer MeshRenderer;
    [HideInInspector] public CreateMeshInfoData CreateMeshInfo;
    [System.Serializable]
    public class CreateMeshInfoData
    {
        [HideInInspector] public MeshFilter MeshFilter;
        [HideInInspector] public bool InvertUV_X = false;
        [HideInInspector] public bool InvertUV_Y = false;
        [HideInInspector] public bool FlipFace = false;
        [HideInInspector] public int Count_Y = 1;
    }

    //Random color (high saturation)
    static Color GetRandomColor_HighSaturation()
    {
        Color OutputColor = new Color();

        int HightColor = Random.Range(0, 3);
        if (HightColor == 0)
        {
            OutputColor.r = 1;
            OutputColor.g = Random.Range((float)0, (float)1);
            OutputColor.b = Random.Range((float)0, (float)1);
            OutputColor.a = 1;
        }
        else if (HightColor == 1)
        {
            OutputColor.r = Random.Range((float)0, (float)1);
            OutputColor.g = 1;
            OutputColor.b = Random.Range((float)0, (float)1);
            OutputColor.a = 1;
        }
        else
        {
            OutputColor.r = Random.Range((float)0, (float)1);
            OutputColor.g = Random.Range((float)0, (float)1);
            OutputColor.b = 1;
            OutputColor.a = 1;
        }

        return OutputColor;
    }

    //Toolbar
    [HideInInspector] public int SelectToolbar = 0;
    [HideInInspector] public int SelectPath = 0; //Pass number selected in the inspector
    [HideInInspector] public string[] ToolbarName = { "Animation", "Path 1", "Path 2", "Path 3", "Mesh" };


    private void Awake()
    {
        Debug.Log("First created (2 passes)");
        if (PathInfo.Count == 0)
        {
            PathInfo.Add(new PathInfoData());
            PathInfo.Add(new PathInfoData());
        }

        transform.position = new Vector3(0, 0, 0);

        //Organize the mesh data to be created
        SetMesh();
    }

    //Added delegate for drawing scene GUI
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += DrawSceneGUI;
        Undo.undoRedoPerformed += MyUndoCallback;
    }
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= DrawSceneGUI;
        Undo.undoRedoPerformed -= MyUndoCallback;
    }

    void MyUndoCallback()
    {
        //Debug.Log("Undone");
        // code for the action to take on Undo
        GenerateMesh(PathInfo.Count, CreateMeshInfo.Count_Y, PathInfo[0].VertexPos.Length);
    }

    ///////////////////////////////////////
    /////////     Thin GUI      ///////////
    ///////////////////////////////////////
    void DrawSceneGUI(SceneView sceneView)
    {
        MotionPath Ge = this;

        DrawPathInfo(Ge);

        Viwer_MeshInfo();
    }

    //Draw path information
    void DrawPathInfo(MotionPath Ge)
    {
        //Vertex position icon
        GUIStyle Style_Vert = new GUIStyle();
        Style_Vert.contentOffset = new Vector2(-7, -9); //Icon position adjustment

        //Vertex position icon
        GUIStyle Style_Target = new GUIStyle();
        Style_Target.contentOffset = new Vector2(-9.5f, -9.5f); //Icon position adjustment

        //Draw path information
        for (int i = 0; i < PathInfo.Count; i++)
        {
            //Line drawing
            Handles.color = (SelectPath == i && Ge.SelectToolbar == 1 ? new Color(5, 5, 5, 1) : PathInfo[i].PathColor);

            Handles.DrawAAPolyLine(PathInfo[i].PathWidth * (SelectPath == i && Ge.SelectToolbar == 1 ? 3 : 1), Ge.PathInfo[i].PathPos.Length, Ge.PathInfo[i].PathPos);
            Handles.color = GUI.color;

            //////////////////////////////////////////////////
            /////////       Draw vertex position      ////////
            //////////////////////////////////////////////////
            if (PathInfo[i].EditMode)
            {
                VertEditMode(Ge, PathInfo[i]);
            }
            else
            {
                for (int j = 0; j < PathInfo[i].VertexPos.Length; j++)
                {
                    Handles.Label(PathInfo[i].VertexPos[j], EditorGUIUtility.IconContent("winbtn_mac_close"), Style_Vert); //Vertex positions
                }
            }

            //Path target icon
            if (PathInfo[i].TargetObject != null)
            {
                Handles.Label(PathInfo[i].TargetObject.transform.position, EditorGUIUtility.IconContent("DotFrame"), Style_Target); //Vertex positions
            }
        }
    }


    ////////////////////////////////////////////////
    //////////     Vertex Edit Mode     ////////////
    ////////////////////////////////////////////////
    void VertEditMode(MotionPath Ge, PathInfoData GetPathInfo)
    {
        //Style for the selected vertex
        GUIStyle Style = new GUIStyle();
        Style.contentOffset = new Vector2(-7, -9); //Icon position adjustment

        //Button size when in edit mode
        float ButtonSize = 15;
        float ButtonPosAdd = -ButtonSize / 2;
        //Draw a path
        for (int j = 0; j < GetPathInfo.VertexPos.Length; j++)
        {
            if (j != GetPathInfo.EditVertIdx)
            {
                Handles.BeginGUI();
                GUI.backgroundColor = new Color(2, 2, 2);
                if (GUI.Button(new Rect(HandleUtility.WorldToGUIPoint(GetPathInfo.VertexPos[j]).x + ButtonPosAdd, HandleUtility.WorldToGUIPoint(GetPathInfo.VertexPos[j]).y + ButtonPosAdd, ButtonSize, ButtonSize), ""))
                {
                    GetPathInfo.EditVertIdx = j;
                    //Genarator.MovePosition.Insert(i, new Vector3(Genarator.MovePosition[i].x, Genarator.MovePosition[i].y, Genarator.MovePosition[i].z));
                }
                GUI.backgroundColor = GUI.color;
                Handles.EndGUI();
            }
            //Currently selected vertex
            else
            {
                Handles.Label(GetPathInfo.VertexPos[j], EditorGUIUtility.IconContent("d_winbtn_mac_min"), Style);
            }
        }

        //Location gizmo
        if (GetPathInfo.VertexPos.Length > GetPathInfo.EditVertIdx)
        {
            Undo.RecordObject(Ge, "SaveHandlePos");
            EditorGUI.BeginChangeCheck();
            GetPathInfo.VertexPos[GetPathInfo.EditVertIdx] = Handles.PositionHandle(GetPathInfo.VertexPos[GetPathInfo.EditVertIdx], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Ge.GenerateMesh(Ge.PathInfo.Count, Ge.CreateMeshInfo.Count_Y, Ge.PathInfo[0].VertexPos.Length);
            }
        }
    }




    ////////////////////////////////////////////////////////////
    ////////////////   Mesh creation tool     //////////////////
    ////////////////////////////////////////////////////////////
    #region 
    //GUI for debugging when creating a mesh
    [HideInInspector] public bool Debug_VertInfo = false;
    [HideInInspector] public bool Debug_VertPos = false;
    [HideInInspector] public bool Debug_DrawTriLine = false;
    void Viwer_MeshInfo()
    {
        if (Debug_VertInfo)
        {
            for (int i = 0; i < MeshFilter.mesh.vertices.Length; i++)
            {
                Handles.Label(MeshFilter.mesh.vertices[i], i.ToString() + (Debug_VertPos ? " " + MeshFilter.mesh.vertices[i] : "")); //버텍스 위치들
            }
        }
        if (Debug_DrawTriLine)
        {
            Debug_DrawTriLineData();
        }


    }

    //Debug
    Vector3[] ConvertData = new Vector3[4];
    void Debug_DrawTriLineData()
    {
        for (int i = 0; i < Tri.Count; i++)
        {
            Vector3[] Test = ConvertIndexToPos(Tri[i], MeshFilter.mesh.vertices);
            Handles.Label((Test[0] + Test[1] + Test[2]) / 3, i.ToString());
            Handles.DrawAAPolyLine(Test);
        }

        //Convert index into position.
        Vector3[] ConvertIndexToPos(Vector3 GetVertIndex, Vector3[] VertPos)
        {
            ConvertData[0] = VertPos[(int)GetVertIndex.x];
            ConvertData[1] = VertPos[(int)GetVertIndex.y];
            ConvertData[2] = VertPos[(int)GetVertIndex.z];
            ConvertData[3] = VertPos[(int)GetVertIndex.x];
            return ConvertData;
        }
    }

    //Elements to be created first when components are pasted
    void SetMesh()
    {
        if(MeshFilter == null || MeshRenderer == null)
        {
            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();

            MeshFilter.mesh = new Mesh();
            MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            MeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            MeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            MeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    //////////////////////////////////////////////////////////
    ////////////////     Mesh creation    ////////////////////
    //////////////////////////////////////////////////////////
    bool BeformFlip = false;
    public void GenerateMesh(int PathCount, int Ydetail, int XCount)
    {
        if (IsSamePathVert()) //Check if the number of vertices is the same
        {
            if (BeformFlip != CreateMeshInfo.FlipFace)
            {
                for (int i = 0; i < PathInfo.Count; i++)
                {
                    System.Array.Reverse(PathInfo[i].VertexPos);
                }
                BeformFlip = CreateMeshInfo.FlipFace;
            }

            //#endregion
            MeshFilter.mesh.Clear(); //Mesh initialization

            //Vertices
            MeshFilter.mesh.vertices = SetVertPos(Ydetail).ToArray();

            //Triangles
            int YCount = (Ydetail > 1 ? PathCount * Ydetail - Ydetail + 1 : PathCount * Ydetail);
            List<Vector3> Tris = SetTriList(XCount, YCount); //Triangular plane assignment (X vertex, Y pass count)
            MeshFilter.mesh.triangles = Tri_ConvertIntArray(Tris); //Convert Vector3 to Int Array

            //UV
            MeshFilter.mesh.uv = SetUV(XCount, YCount);
        }
    }

    //Assign vertex position
    //[Header("For debug testing (must be removed later)")]
    //public List<Vector3> VertPosList;
    List<Vector3> SetVertPos(int YCount)
    {
        List<Vector3> VertPos = new List<Vector3>();
            for (int X = 0; X < PathInfo[0].VertexPos.Length; X++) //rank
        {
            for (int Y = 0; Y < PathInfo.Count; Y++) //stripe
            {
                //The first index 0 is added randomly
                if (Y == 0)
                {
                    VertPos.Add(PathInfo[0].VertexPos[X]);
                }
                //Add remaining vertices
                if (Y + 1 < PathInfo.Count)
                {
                    for (int i = 1; i <= CreateMeshInfo.Count_Y; i++)
                    {
                        float Value = i / (float)CreateMeshInfo.Count_Y;
                        Vector3 InputPos = Vector3.Lerp(PathInfo[Y].VertexPos[X], PathInfo[Y + 1].VertexPos[X], Value);
                        VertPos.Add(InputPos);
                    }
                }
            }
        }
        return VertPos;
    }

    //Tri Calculation
    [HideInInspector] public List<Vector3> Tri = new List<Vector3>(0);
    List<Vector3> SetTriList(int XCount, int YCount)
    {
        //Debug.Log("Total number " + "X : " + XCount.ToString() + "  Y : " + YCount.ToString());
        //List<Vector3> NewList = new List<Vector3>();
        Tri.Clear();

        //Number
        int idx = 0;
        for (int x = 0; x < XCount - 1; x++)
        {
            //Number of sides
            for (int y = 0; y < YCount - 1; y++)
            {
                //Debug.Log("X : " + x.ToString() + "Y : " + y.ToString() + "  Idx : " + (idx).ToString());
                //Debug.Log(y + " , " + x); //When multiplied by 2 and then multiplied by 3, index data comes
                Tri.Add(GetTri(idx, YCount, true)); //Bottom side
                Tri.Add(GetTri(idx, YCount, false)); //Top
                idx++;
            }
            idx++;
        }
        //Tri = NewList;
        return Tri;
    }

    //Top and bottom configuration
    Vector3 GetTri(int Index, int YCount, bool Under) //Under is the lower triangular surface
    {
        //int TargetIdx = X * (YCount + Y); //Target index
        Vector3 Output = new Vector3();

        if (Under) //Calculate bottom side coordinates
        {
            Output.x = Index;
            Output.y = Index + 1;
            Output.z = Index + YCount;
        }
        else //Top coordinate calculation
        {
            Output.x = Index + YCount;
            Output.y = Index + 1;
            Output.z = Index + YCount + 1;
        }
        //Debug.Log(Output);
        return Output;
    }

    //Change Vector3 list of Tri to Int array
    int[] Tri_ConvertIntArray(List<Vector3> GetTris)
    {
        List<int> Array = new List<int>();
        for (int i = 0; i < GetTris.Count; i++)
        {
            Array.Add((int)GetTris[i].x);
            Array.Add((int)GetTris[i].y);
            Array.Add((int)GetTris[i].z);
        }
        //Debug.Log("Number of triangular surface index : " + Array.Count);
        return Array.ToArray();
    }

    //UV process
    Vector2[] SetUV(int XCount, int YCount)
    {
        List<Vector2> UvList = new List<Vector2>();
        for (int X = 0; X < XCount; X++)
        {
            float ValueX = (float)X / (XCount - 1);
            for (int Y = 0; Y < YCount; Y++)
            {
                float ValueY = (float)Y / (YCount - 1);
                UvList.Add(new Vector2(CreateMeshInfo.InvertUV_X ? 1 - ValueX : ValueX, CreateMeshInfo.InvertUV_Y ? 1 - ValueY : ValueY));
            }
        }
        return UvList.ToArray();
    }
    #endregion


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////    Deactivate the Mesh button if you don't check if the number of vertices of the passes match    ////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public bool IsSamePathVert()
    {
        if (PathInfo.Count > 0) //If there is at least one
        {
            bool Output = true;
            int Count = PathInfo[0].VertexPos.Length; //Based on the number of vertices
            for (int i = 0; i < PathInfo.Count; i++)
            {
                if (PathInfo[i].TargetObject == null) //Deactivate if there is no target object
                {
                    Output = false;
                    break;
                }
                if (PathInfo[i].VertexPos.Length != Count) //Disable if the number is not the same
                {
                    Output = false;
                    break;
                }
            }
            return Output;
        }

        return true;
    }

    ////////////////////////////////////////////
    //////////      Save Mesh      /////////////
    ////////////////////////////////////////////
    //string BeforePath = Path
    [HideInInspector] string BeforePath = "";
    public void SaveMesh(Mesh mesh)
    {
        //Create a new mesh
        Mesh NewMesh = (Mesh)UnityEngine.Object.Instantiate(mesh); 

        //If there is no previous path, it will be taken as the default project path
        string NewBeforePath = (BeforePath.Length != 0) ? BeforePath : Application.dataPath;

        //Path Open the panel to get the save path
        string filePath = 
        EditorUtility.SaveFilePanelInProject("Save Mesh", "FX_Mesh_" + mesh.name, "asset", "", NewBeforePath);
        if (filePath == "") return;

        //Update previous pass
        //(Trying to fetch the old path when you keep saving.)
        BeforePath = filePath;

        //Asset creation
        AssetDatabase.CreateAsset(NewMesh, filePath);  
        
        //Save Asset
        AssetDatabase.SaveAssets();

        //Initialize folder head
        AssetDatabase.Refresh();
    }
}

[CustomEditor(typeof(MotionPath))]
public class MotionPath_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //GUILayout.Space(100);

        MotionPath Ge = (MotionPath)target;

        //All Undo
        Undo.RecordObject(Ge, "All State");

        //Animator 
        EditorGUI.BeginChangeCheck();
        Ge.Animator = (Animator)EditorGUILayout.ObjectField("Animator", Ge.Animator, typeof(Animator));
        bool ChangeAniamtor = EditorGUI.EndChangeCheck(); //Animator change

        // //Object
        // for (int i = 0; i < Ge.PathInfo.Count; i++)
        // {
        //     EditorGUI.BeginChangeCheck();
        //     Ge.PathInfo[i].TargetObject = (GameObject)EditorGUILayout.ObjectField("Target " + "(" + "Path " + (i + 1).ToString() + ")", Ge.PathInfo[i].TargetObject, typeof(GameObject));
        //     if (EditorGUI.EndChangeCheck())
        //     {
        //         if (Ge.PathInfo[i].TargetObject != null)
        //         {
        //             //Regenerate path when tracked object changes

        //             if (Ge.PathInfo[i].AutoUpdate)
        //             {
        //                 CreatePath(Ge.PathInfo[i]);
        //             }
        //         }
        //     }
        // }

        ///////////////////////////////////////////////////////
        //////////////////      GUI     ///////////////////////
        ///////////////////////////////////////////////////////
        GUILayout.Space(10);


        //When you have an animator
        if (Ge.Animator != null)
        {
            //Ge.SelectToolbar = GUILayout.Toolbar(Ge.SelectToolbar, Ge.ToolbarName, GUILayout.MinHeight(35));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Path"))
            {
                Ge.PathInfo.Add(new MotionPath.PathInfoData());
            }

            //In case of more than 2 passes
            if (Ge.PathInfo.Count > 2)
            {
                GUI.backgroundColor = Color.red * 1.5f;
                if (GUILayout.Button("Remove Path" + " (" + (Ge.SelectPath + 1).ToString() + ")", GUILayout.MaxWidth(130)))
                {
                    Ge.PathInfo.RemoveAt(Ge.SelectPath);
                    Ge.SelectPath = Mathf.Min(Ge.PathInfo.Count - 1, Ge.SelectPath); //지운게 마지막꺼면
                    SceneView.RepaintAll();
                    Ge.GenerateMesh(Ge.PathInfo.Count, Ge.CreateMeshInfo.Count_Y, Ge.PathInfo[0].VertexPos.Length);
                }
                GUI.backgroundColor = GUI.color;
            }
            GUILayout.EndHorizontal();

            //////////////////////////////////////////
            //////////     Button list     ///////////
            //////////////////////////////////////////

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            //Animation button
            GUI.backgroundColor = Ge.SelectToolbar == 0 ? new Color(2, 1, 0) : GUI.color;
            if (GUILayout.Button("Animation", GUILayout.MinHeight(35)))
            {
                Ge.SelectToolbar = 0;
            }
            GUI.backgroundColor = GUI.color;

            //Path button
            for (int i = 0; i < Ge.PathInfo.Count; i++)
            {
                //Path Color when the button is selected
                GUI.backgroundColor = Ge.SelectToolbar == 1 && Ge.SelectPath == i ? new Color(2, 1, 0) : GUI.color;
                if (GUILayout.Button("Path " + (i + 1).ToString() + " (" + Ge.PathInfo[i].VertexPos.Length.ToString() + ")", GUILayout.MinHeight(35)))
                {
                    Ge.SelectToolbar = 1;
                    Ge.SelectPath = i;
                }
                GUI.backgroundColor = GUI.color;
            }

            //Mesh button

            GUI.backgroundColor = Ge.SelectToolbar == 2 ? new Color(2, 1, 0) : GUI.color;
            if (GUILayout.Button("Mesh", GUILayout.MinHeight(35)))
            {
                Ge.SelectToolbar = 2;
            }
            GUI.backgroundColor = GUI.color;



            //Horizontal listing finish
            GUILayout.EndHorizontal();

            //Pressing the buttons reloads the scene to update the path display information
            bool ClickToolbar = EditorGUI.EndChangeCheck();
            if (ClickToolbar)
            {
                SceneView.RepaintAll(); //Pressing the pass button redraws the scene information
            }


            //Inspector draw based on button selection
            if (Ge.SelectToolbar == 0)
            {
                //Animation information GUI
                GUILayout.BeginVertical("GroupBox");
                DrawAniInfo();
                GUILayout.EndVertical();
            }
            //pass
            if (Ge.SelectToolbar == 1)
            {
                //Object the path will be drawn on
                EditorGUI.BeginChangeCheck();
                GUILayout.Space(10);
                Ge.PathInfo[Ge.SelectPath].TargetObject = (GameObject)EditorGUILayout.ObjectField("Path Target", Ge.PathInfo[Ge.SelectPath].TargetObject, typeof(GameObject));
                if (EditorGUI.EndChangeCheck())
                {
                    if (Ge.PathInfo[Ge.SelectPath].TargetObject != null)
                    {
                        //Regenerate path when tracked object changes

                        if (Ge.PathInfo[Ge.SelectPath].AutoUpdate)
                        {
                            CreatePath(Ge.PathInfo[Ge.SelectPath]);
                        }
                    }
                }

                //In case of more than 2 passes
                // if (Ge.PathInfo.Count > 2)
                // {
                //     GUI.backgroundColor = Color.red * 1.5f;
                //     if (GUILayout.Button("Remove Path"))
                //     {
                //         Ge.PathInfo.RemoveAt(Ge.SelectPath);
                //         Ge.SelectPath = Mathf.Min(Ge.PathInfo.Count - 1, Ge.SelectPath); //If the last thing you erase
                //         SceneView.RepaintAll();
                //         Ge.GenerateMesh(Ge.PathInfo.Count, Ge.CreateMeshInfo.Count_Y, Ge.PathInfo[0].VertexPos.Length);
                //     }
                //     GUI.backgroundColor = GUI.color;
                // }

                //When there is an object
                if (Ge.PathInfo[Ge.SelectPath].TargetObject != null)
                {
                    //Pass GUI
                    GUILayout.BeginVertical("GroupBox");
                    DrawPathInfo(Ge.PathInfo[Ge.SelectPath]);
                    GUILayout.EndVertical();

                    //Pass To Vertex Switch GUI
                    GUILayout.BeginVertical("GroupBox");
                    Viewer_VertexPos(Ge.PathInfo[Ge.SelectPath]);
                    GUILayout.EndVertical();
                }
            }
            //Mesh creation
            else if (Ge.SelectToolbar == 2)
            {
                GUILayout.BeginVertical("GroupBox");
                GenerateMeshViewer(Ge);
                GUILayout.EndVertical();
            }

        }



        // //Draw path lists
        // void DrawSelectPath_Inspector()
        // {


        //     //Pass GUI
        //     GUILayout.BeginVertical("GroupBox");
        //     DrawPathInfo(Ge.PathInfo[Ge.SelectPath]);
        //     GUILayout.EndVertical();

        //     //Pass To Vertex Switch GUI
        //     GUILayout.BeginVertical("GroupBox");
        //     Viewer_VertexPos(Ge.PathInfo[Ge.SelectPath]);
        //     GUILayout.EndVertical();
        // }

        //Pass information
        void DrawPathInfo(MotionPath.PathInfoData GetPathInfo)
        {
            PathViewerSetting(GetPathInfo);
            GetPathInfo.AutoUpdate = EditorGUILayout.Toggle("Auto Update", GetPathInfo.AutoUpdate);

            EditorGUI.BeginChangeCheck();
            int PathDetail = EditorGUILayout.IntSlider("PathDetail (Frame)", GetPathInfo.PathFrame, 1, 500);
            PathDetail = Mathf.Max(PathDetail, 1); //Minimum value
            GetPathInfo.PathFrame = PathDetail;
            if (EditorGUI.EndChangeCheck()) //Update when path detail is modified
            {
                if (GetPathInfo.AutoUpdate)
                {
                    CreatePath(GetPathInfo);
                }
            }

            if (!GetPathInfo.AutoUpdate)
            {
                if (GUILayout.Button("Create Path"))
                {
                    CreatePath(GetPathInfo);
                }
            }
        }

        //CreatePath (Create Pass)
        void CreatePath(MotionPath.PathInfoData GetPathInfo)
        {
            if (GetPathInfo.TargetObject != null)
            {
                float FirstFrame = Ge.AnimationSlider; //Backup current pause time

                List<Vector3> NewPathPosition = new List<Vector3>(2);
                for (float i = Ge.StartFrame; i < Ge.EndFrame; i += (1f / (float)GetPathInfo.PathFrame))
                {
                    Ge.AnimationSlider = i; //Pose time update
                    UpDatePos(); //Pose update
                    NewPathPosition.Add(GetPathInfo.TargetObject.transform.position);
                }
                GetPathInfo.PathPos = NewPathPosition.ToArray();

                Ge.AnimationSlider = FirstFrame; //Back up to the initially set pause time
                UpDatePos(); //Pose update

                //Automatic update of vertex average position value
                if (GetPathInfo.Vertex_AutoUpdate)
                {
                    CountVertexPos(GetPathInfo); //Update vertex average position value
                }
            }
        }

        //Path view settings
        void PathViewerSetting(MotionPath.PathInfoData GetPathInfo)
        {
            GetPathInfo.PathViewerSetting = EditorGUILayout.Toggle("Path View Setting", GetPathInfo.PathViewerSetting);
            if (GetPathInfo.PathViewerSetting)
            {
                GUILayout.BeginVertical("GroupBox");
                EditorGUI.BeginChangeCheck();
                GetPathInfo.PathColor = EditorGUILayout.ColorField("Path Color", GetPathInfo.PathColor);
                GetPathInfo.PathWidth = EditorGUILayout.FloatField("Path Width", GetPathInfo.PathWidth);
                if (EditorGUI.EndChangeCheck())
                {
                    if (GetPathInfo.AutoUpdate)
                    {
                        CreatePath(GetPathInfo);
                    }
                }
                GUILayout.EndVertical();
            }
        }

        //Animation information
        void DrawAniInfo()
        {
            //Animator change
            if (ChangeAniamtor)
            {
                Ge.AnimationClips = Ge.Animator.runtimeAnimatorController.animationClips;
                Ge.AniClipsName = new string[Ge.AnimationClips.Length];
                for (int i = 0; i < Ge.AniClipsName.Length; i++)
                {
                    Ge.AniClipsName[i] = Ge.AnimationClips[i].name;
                }
            }

            float SelectClipLength = Ge.AnimationClips[Ge.SelectAniClip].length; //Maximum length of selected clip


            //Select animation to play
            EditorGUI.BeginChangeCheck();
            Ge.SelectAniClip = EditorGUILayout.Popup("재생할 애니메이션", Ge.SelectAniClip, Ge.AniClipsName);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("애니 변경");
                Ge.PlayStateName = GetStringFromAniClip(Ge.Animator, Ge.AnimationClips[Ge.SelectAniClip]); //Get the name of the animation state to play

                //Start, end frame preference
                //Ge.StartFrame = 0;
                //Ge.EndFrame = Ge.AnimationClips[Ge.SelectAniClip].length;
                UpDatePos();

                for (int i = 0; i < Ge.PathInfo.Count; i++)
                {
                    if (Ge.PathInfo[i].AutoUpdate)
                    {
                        CreatePath(Ge.PathInfo[i]);
                    }
                }
            }

            EditorGUI.BeginChangeCheck(); //Check if the pose related variable is updated
            //GUILayout.BeginHorizontal();
            //Minimum value
            float SetStartFrame = EditorGUILayout.FloatField("Start Frame", Ge.StartFrame);
            SetStartFrame = Mathf.Clamp(SetStartFrame, 0, Ge.EndFrame);
            Ge.StartFrame = SetStartFrame;

            //Maximum value
            float SetEndFrame = EditorGUILayout.FloatField("End Frame", Ge.EndFrame);
            SetEndFrame = Mathf.Clamp(SetEndFrame, Ge.StartFrame, SelectClipLength);
            Ge.EndFrame = SetEndFrame;

            //GUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider("Set Frame", ref Ge.StartFrame, ref Ge.EndFrame, 0, SelectClipLength);
            bool ChangeMinMax = EditorGUI.EndChangeCheck();
            if (ChangeMinMax) //Create a new path when the values related to the path change
            {
                for (int i = 0; i < Ge.PathInfo.Count; i++)
                {
                    if (Ge.PathInfo[i].AutoUpdate)
                    {
                        CreatePath(Ge.PathInfo[i]);
                    }
                }
            }

            //Annie playing
            EditorGUI.BeginChangeCheck(); //Check if the pose related variable is updated
            Ge.AnimationSlider = EditorGUILayout.Slider("Ani Play", Ge.AnimationSlider, Ge.StartFrame, Ge.EndFrame);
            //When the parameters related to the pose change
            if (EditorGUI.EndChangeCheck() || ChangeMinMax)
            {
                UpDatePos(); //Animation pose update
            }
        }

        //Character animation update
        void UpDatePos()
        {
            Ge.Animator.Play(Ge.PlayStateName, -1, Ge.AnimationSlider);
            Ge.Animator.Update(Ge.AnimationSlider - 1);
        }
    }


    #region Animation related functions
    AnimatorState[] AllState;
    //Get animation state name as clip (Used when getting String for AnimatorPlay)
    string GetStringFromAniClip(Animator GetAnimator, AnimationClip Clip)
    {
        string OutString = "";
        AllState = GetAnimatorStates(GetAnimator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController); //Get all states 
        OutString = GetStateFromClip(AllState, Clip).name; //Import State from animation clip and assign name
        return OutString;
    }

    //Get all animator states
    AnimatorState[] GetAnimatorStates(UnityEditor.Animations.AnimatorController anicon)
    {
        List<AnimatorState> ret = new List<AnimatorState>();
        foreach (var layer in anicon.layers)
        {
            foreach (var subsm in layer.stateMachine.stateMachines)
            {
                foreach (var state in subsm.stateMachine.states)
                {
                    ret.Add(state.state);
                }
            }
            foreach (var s in layer.stateMachine.states)
            {
                ret.Add(s.state);
            }
        }
        return ret.ToArray();
    }

    //Get the state corresponding to the animation clip among all animator states
    AnimatorState GetStateFromClip(AnimatorState[] StateList, AnimationClip GetClip)
    {
        AnimatorState OutState = null;
        for (int i = 0; i < StateList.Length; i++)
        {
            if (StateList[i].motion == GetClip)
            {
                OutState = StateList[i];
                break; //stop
            }
        }
        return OutState;
    }
    #endregion



    //////////////////////////////////////////////////////////////////////
    /////////////////////       VertexVertex      ///////////////////////
    //////////////////////////////////////////////////////////////////////
    //Vertex Allocation Viewer by Average of Passes
    #region Function to calculate the average of the pass and allocate vertices
    void Viewer_VertexPos(MotionPath.PathInfoData GetPathInfo)
    {
        
        GUIStyle VertCountFont = new GUIStyle();
        VertCountFont.normal.textColor = Color.green;
        VertCountFont.fontStyle = FontStyle.Bold;
        VertCountFont.alignment = TextAnchor.UpperCenter;
        VertCountFont.fontSize = 18;
        GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f, 1);
        GUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Count : " + GetPathInfo.VertexPos.Length.ToString(), VertCountFont, GUILayout.MinHeight(18));
        GUILayout.EndVertical();
        GUI.backgroundColor = GUI.color;

        //Update on value change
        EditorGUI.BeginChangeCheck();
        GetPathInfo.Vertex_AutoUpdate = EditorGUILayout.Toggle("Vertex_AutoUpdate", GetPathInfo.Vertex_AutoUpdate);
        GetPathInfo.Vertex_Detail = EditorGUILayout.IntSlider("Vertex_Detail(Average)", GetPathInfo.Vertex_Detail, 1, 100);
        GetPathInfo.Vertex_Distance = EditorGUILayout.Slider("Vertex_Distance", GetPathInfo.Vertex_Distance, 0.01f, 1);
        if (EditorGUI.EndChangeCheck())
        {
            if (GetPathInfo.Vertex_AutoUpdate)
            {
                CountVertexPos(GetPathInfo);
            }
        }

        //Vertex correction mode
        #region 
        EditorGUI.BeginChangeCheck();
        GetPathInfo.EditMode = EditorGUILayout.Toggle("Vertex EditMode", GetPathInfo.EditMode);
        if (GetPathInfo.EditMode)
        {
            GUILayout.BeginVertical("GroupBox");

            //Font style
            GUIStyle InputFrontStyle = new GUIStyle();
            InputFrontStyle.fontStyle = FontStyle.Bold;
            InputFrontStyle.normal.textColor = Color.green;
            InputFrontStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField("In vertex modification mode, vertices are initialized when the values above are modified.", InputFrontStyle);
            GUILayout.EndHorizontal();
        }
        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll(); //When editing edit mode
        }
        #endregion


        if (!GetPathInfo.Vertex_AutoUpdate)
        {
            if (GUILayout.Button("Calculate the average vertex distance of a line"))
            {
                CountVertexPos(GetPathInfo);
            }
        }
    }

    //Calculate the average distance value
    void CountVertexPos(MotionPath.PathInfoData GetPathInfo)
    {
        GetPathInfo.VertexPos = GetVertexPos(GetPathInfo.PathPos, GetPathInfo.Vertex_Detail, GetPathInfo.Vertex_Distance);
        SceneView.RepaintAll();
    }

    //Finding the average position (Detail is the detail between two points, if there are 10, Lerp is calculated 10 times)
    //Detail = Details between two points, if there are 10, calculate Lerp 10 times (about 10 is appropriate)
    //VertDistance = Distance between vertices
    Vector3[] GetVertexPos(Vector3[] GetPos, int Detail, float VertDistance)
    {
        //Create initial value to compare distances
        Vector3 BeforePoint = new Vector3(0, 0, 0);

        BeforePoint.x = GetPos[0].x;
        BeforePoint.y = GetPos[0].y;
        BeforePoint.z = GetPos[0].z;

        //Debug.Log(BeforePoint);

        List<Vector3> OutPut = new List<Vector3>();
        OutPut.Add(BeforePoint); //Add initial value

        for (int i = 0; i < GetPos.Length - 1; i++) //Vertex positions
        {
            for (int j = 0; j <= Detail; j++) //Lerp detail in vertex
            {
                float Value = ((float)j / (float)Detail); // 0~1
                Vector3 NextPos = Vector3.Lerp(GetPos[i], GetPos[i + 1], Value); //Scrape detail values and throw them for distance calculation
                if (GetDistance(BeforePoint, NextPos, VertDistance)) //Compare distance and return if greater than the distance
                {
                    OutPut.Add(NextPos); //Add corresponding position value
                    BeforePoint = NextPos; //Update previous value to next value
                }
            }
        }

        return OutPut.ToArray();
    }

    //True if the previous value and the next value are compared and are farther away by the corresponding distance.
    bool GetDistance(Vector3 BeforePos, Vector3 NextPos, float Distance)
    {
        bool Output = Vector3.Distance(BeforePos, NextPos) >= Distance ? true : false;
        return Output;
    }
    #endregion




    ///////////////////////////////////////////////////////////////////
    /////////////////////       MeshViewer      ///////////////////////
    ///////////////////////////////////////////////////////////////////


    void GenerateMeshViewer(MotionPath Ge)
    {
        bool SameVert = Ge.IsSamePathVert();

        GUIStyle Font = new GUIStyle();
        Font.normal.textColor = Color.green;
        Font.alignment = TextAnchor.MiddleCenter;
        Font.fontStyle = FontStyle.Bold;
        Font.fontSize = 18;
        
        if (!SameVert)
        {

            GUILayout.Label("All vertices in the path must be the same", Font);
        }
        else
        {
            GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f);
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label(Ge.MeshFilter.mesh.vertices.Length.ToString() + " Verts, " + (Ge.MeshFilter.mesh.triangles.Length / 3).ToString() + " tris", Font);
            GUILayout.EndVertical();
            GUI.backgroundColor = GUI.color;

            EditorGUI.BeginChangeCheck();
            Ge.Debug_VertInfo = EditorGUILayout.Toggle("View VertIdx", Ge.Debug_VertInfo);
            if (Ge.Debug_VertInfo)
            {
                Ge.Debug_VertPos = EditorGUILayout.Toggle("View VertPos", Ge.Debug_VertPos);
            }
            Ge.Debug_DrawTriLine = EditorGUILayout.Toggle("View Tris", Ge.Debug_DrawTriLine);
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            

            EditorGUI.BeginChangeCheck();
            Ge.CreateMeshInfo.Count_Y = EditorGUILayout.IntSlider("Count_Y", Ge.CreateMeshInfo.Count_Y, 1, 10);
            if (GUILayout.Button("InvertUV_X"))
            {
                Ge.CreateMeshInfo.InvertUV_X = !Ge.CreateMeshInfo.InvertUV_X;
            }
            if (GUILayout.Button("InvertUV_Y"))
            {
                Ge.CreateMeshInfo.InvertUV_Y = !Ge.CreateMeshInfo.InvertUV_Y;
            }
            if (GUILayout.Button("FlipFace"))
            {
                Ge.CreateMeshInfo.FlipFace = !Ge.CreateMeshInfo.FlipFace;
            }
            bool ChangeMeshData = EditorGUI.EndChangeCheck();
            if (ChangeMeshData) //Rebuild mesh when changing mesh-related data
            {
                Ge.GenerateMesh(Ge.PathInfo.Count, Ge.CreateMeshInfo.Count_Y, Ge.PathInfo[0].VertexPos.Length);
            }

            if (GUILayout.Button("GenerateMesh (Mesh creation)"))
            {
                Ge.GenerateMesh(Ge.PathInfo.Count, Ge.CreateMeshInfo.Count_Y, Ge.PathInfo[0].VertexPos.Length);
            }

            EditorGUI.BeginDisabledGroup(Ge.MeshFilter.mesh.vertices.Length == 0); //Button deactivation condition
            {
                if(GUILayout.Button("Save Mesh"))
                {
                    Ge.SaveMesh(Ge.MeshFilter.mesh);
                }
            }
            EditorGUI.EndDisabledGroup();

        }
    }
}

#pragma warning restore CS0618
#endif
