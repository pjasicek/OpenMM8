using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ProBuilder.Interface;	// pb_GUI_Utility
using ProBuilder.Core;
using ProBuilder.EditorCore;
using ProBuilder.MeshOperations;
using System.Linq;				// Sum()

class FaceIndexViewer : EditorWindow
{
    [MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Face Index Viewer")]
    static void MenuInitEditorCallbackViewer()
    {
        EditorWindow.GetWindow<FaceIndexViewer>(false, "Face Index Viewer", true).Show();
    }

    List<string> logs = new List<string>();
    Vector2 scroll = Vector2.zero;
    bool collapse = true;

    static Color logBackgroundColor
    {
        get
        {
            return EditorGUIUtility.isProSkin ? new Color(.15f, .15f, .15f, .5f) : new Color(.8f, .8f, .8f, 1f);
        }
    }

    static Color disabledColor
    {
        get
        {
            return EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, .5f) : new Color(.8f, .8f, .8f, 1f);
        }
    }

    void OnEnable()
    {
        pb_EditorApi.AddOnSelectionUpdateListener(OnSelectionUpdate);
    }

    void OnDisable()
    {
        pb_EditorApi.RemoveOnSelectionUpdateListener(OnSelectionUpdate);
    }

    private List<int> GetSelectedTriangleIndexes(pb_Object pb)
    {
        List<int> triIndexes = new List<int>();

        Mesh mesh = pb.gameObject.GetComponent<MeshFilter>().sharedMesh;

        int[] selectedFaceTris = pb.SelectedTriangles;

        int triangleIdx = 0;
        for (int i = 0; i < mesh.triangles.Length; i += 3, triangleIdx++)
        {
            if (selectedFaceTris.Contains(mesh.triangles[i + 0]) &&
                selectedFaceTris.Contains(mesh.triangles[i + 1]) &&
                selectedFaceTris.Contains(mesh.triangles[i + 2]))
            {
                triIndexes.Add(triangleIdx);
            }
        }

        return triIndexes;
    }

    void OnSelectionUpdate(pb_Object[] selection)
    {
        // Selected 1 face on 1 model
        if (selection != null && selection.Length == 1 && selection[0].SelectedFaceCount == 1)
        {
            pb_Object pb = selection[0];
            Mesh mesh = pb.gameObject.GetComponent<MeshFilter>().sharedMesh;

            pb.ToMesh();
            pb.Refresh();
            int[] selectedTriangles = pb.SelectedTriangles;

            int faceIdx;
            if (pb.FaceWithTriangle(selectedTriangles, out faceIdx))
            {
                logs.Clear();

                List<int> selectedTriIdxs = GetSelectedTriangleIndexes(selection[0]);
                string selTriStr = "";
                foreach (int t in selectedTriIdxs)
                {
                    selTriStr += t + " ";
                }

                AddLog("Face: " + faceIdx);
                AddLog("[" + selectedTriIdxs.Count + "] Selected triangles: " + selTriStr);
                AddLog("Mesh Triangle Count: " + mesh.triangles.Length);
            }
            else
            {
                Debug.LogError("Selected one face but cannot get its index ?!");
            }
        }
        else if (selection != null &&
            selection.Length == 1 &&
            selection[0] != null &&
            selection[0].SelectedFaceCount > 1)
        {
            // Selected multiple faces on 1 model

            pb_Object pb = selection[0];
            pb.ToMesh();
            pb.Refresh();

            List<int> selectedTriIdxs = GetSelectedTriangleIndexes(selection[0]);
            List<int> selectedFaces = new List<int>();

            for (int faceIdx = 0; faceIdx < selection[0].faces.Length; faceIdx++)
            {
                if (selection[0].SelectedFaces.Contains(selection[0].faces[faceIdx]))
                {
                    selectedFaces.Add(faceIdx);
                }
            }

            logs.Clear();

            string faceSel = "Selected Faces [" + selectedFaces.Count + "]: ";
            foreach (int faceIdx in selectedFaces)
            {
                faceSel += faceIdx + " ";
            }

            string selStr = "\n";
            foreach (int t in selectedTriIdxs)
            {
                selStr += t + "\n";
            }

            AddLog(faceSel);
            AddLog("[" + selectedTriIdxs.Count + "]      Selected triangles: " + selStr);
        }
    }

    void OnVertexMovementBegin(pb_Object[] selection)
    {
        AddLog("Began Moving Vertices");
    }

    void OnVertexMovementFinish(pb_Object[] selection)
    {
        AddLog("Finished Moving Vertices");
    }

    void OnMeshCompiled(pb_Object pb, Mesh mesh)
    {
        AddLog(string.Format("Mesh {0} rebuilt", pb.name));
    }

    void AddLog(string summary)
    {
        //logs.Clear();
        logs.Add(summary);
        Repaint();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.FlexibleSpace();

        GUI.backgroundColor = collapse ? disabledColor : Color.white;
        if (GUILayout.Button("Collapse", EditorStyles.toolbarButton))
            collapse = !collapse;
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            logs.Clear();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Callback Log", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        Rect r = GUILayoutUtility.GetLastRect();
        r.x = 0;
        r.y = r.y + r.height + 6;
        r.width = this.position.width;
        r.height = this.position.height;

        GUILayout.Space(4);

        //pb_EditorGUIUtility.DrawSolidColor(r, logBackgroundColor);

        scroll = GUILayout.BeginScrollView(scroll);

        int len = logs.Count;
        int min = System.Math.Max(0, len - 1024);

        for (int i = len - 1; i >= min; i--)
        {
            if (collapse &&
                i > 0 &&
                i < len - 1 &&
                logs[i].Equals(logs[i - 1]) &&
                logs[i].Equals(logs[i + 1]))
                continue;

            GUILayout.Label(string.Format("{0,3}: {1}", i, logs[i]));
        }

        GUILayout.EndScrollView();
    }
}