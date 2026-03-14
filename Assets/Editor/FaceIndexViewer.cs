using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;

class FaceIndexViewer : EditorWindow
{
    [MenuItem("Tools/ProBuilder/Face Index Viewer")]
    static void MenuInitEditorCallbackViewer()
    {
        GetWindow<FaceIndexViewer>(false, "Face Index Viewer", true).Show();
    }

    List<string> logs = new List<string>();
    Vector2 scroll = Vector2.zero;
    bool collapse = true;

    static Color logBackgroundColor
    {
        get
        {
            return EditorGUIUtility.isProSkin
                ? new Color(.15f, .15f, .15f, .5f)
                : new Color(.8f, .8f, .8f, 1f);
        }
    }

    static Color disabledColor
    {
        get
        {
            return EditorGUIUtility.isProSkin
                ? new Color(.3f, .3f, .3f, .5f)
                : new Color(.8f, .8f, .8f, 1f);
        }
    }

    void OnEnable()
    {
        ProBuilderEditor.selectionUpdated += OnSelectionUpdate;
    }

    void OnDisable()
    {
        ProBuilderEditor.selectionUpdated -= OnSelectionUpdate;
    }

    private List<int> GetSelectedTriangleIndexes(ProBuilderMesh pb)
    {
        List<int> triIndexes = new List<int>();

        MeshFilter mf = pb.GetComponent<MeshFilter>();
        Mesh mesh = mf != null ? mf.sharedMesh : null;
        if (mesh == null)
            return triIndexes;

        HashSet<int> selectedTriangleVertices = new HashSet<int>();

        foreach (int faceIndex in pb.selectedFaceIndexes)
        {
            if (faceIndex < 0 || faceIndex >= pb.faces.Count)
                continue;

            foreach (int index in pb.faces[faceIndex].indexes)
                selectedTriangleVertices.Add(index);
        }

        int triangleIdx = 0;
        for (int i = 0; i < mesh.triangles.Length; i += 3, triangleIdx++)
        {
            if (selectedTriangleVertices.Contains(mesh.triangles[i + 0]) &&
                selectedTriangleVertices.Contains(mesh.triangles[i + 1]) &&
                selectedTriangleVertices.Contains(mesh.triangles[i + 2]))
            {
                triIndexes.Add(triangleIdx);
            }
        }

        return triIndexes;
    }

    void OnSelectionUpdate(IEnumerable<ProBuilderMesh> selection)
    {
        var selected = selection != null ? selection.ToList() : null;

        if (selected != null &&
            selected.Count == 1 &&
            selected[0] != null &&
            selected[0].selectedFaceCount == 1)
        {
            ProBuilderMesh pb = selected[0];
            MeshFilter mf = pb.GetComponent<MeshFilter>();
            Mesh mesh = mf != null ? mf.sharedMesh : null;
            if (mesh == null)
                return;

            pb.ToMesh();
            pb.Refresh();

            logs.Clear();

            int faceIdx = pb.selectedFaceIndexes[0];
            List<int> selectedTriIdxs = GetSelectedTriangleIndexes(pb);

            string selTriStr = "";
            foreach (int t in selectedTriIdxs)
                selTriStr += t + " ";

            AddLog("Face: " + faceIdx);
            AddLog("[" + selectedTriIdxs.Count + "] Selected triangles: " + selTriStr);
            AddLog("Mesh Triangle Count: " + (mesh.triangles.Length / 3));
        }
        else if (selected != null &&
                 selected.Count == 1 &&
                 selected[0] != null &&
                 selected[0].selectedFaceCount > 1)
        {
            ProBuilderMesh pb = selected[0];
            pb.ToMesh();
            pb.Refresh();

            List<int> selectedTriIdxs = GetSelectedTriangleIndexes(pb);
            List<int> selectedFaces = pb.selectedFaceIndexes.ToList();

            logs.Clear();

            string faceSel = "Selected Faces [" + selectedFaces.Count + "]: ";
            foreach (int faceIdx in selectedFaces)
                faceSel += faceIdx + " ";

            string selStr = "\n";
            foreach (int t in selectedTriIdxs)
                selStr += t + "\n";

            AddLog(faceSel);
            AddLog("[" + selectedTriIdxs.Count + "] Selected triangles: " + selStr);
        }
    }

    void AddLog(string summary)
    {
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
        r.width = position.width;
        r.height = position.height;

        GUILayout.Space(4);

        // EditorGUI.DrawRect(r, logBackgroundColor);

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