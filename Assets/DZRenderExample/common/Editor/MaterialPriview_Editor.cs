using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialPreview))]
public class MaterialPriview_Editor : Editor
{
    // Start is called before the first frame update
    private MaterialPreview tar;
    private MeshRenderer    previewMesh;
    private TextMesh        logText;
    private void OnEnable()
    {
        tar = target as MaterialPreview;
        Transform c1 = tar.transform.GetChild(0);
        previewMesh = c1!=null? c1.GetComponentInChildren<MeshRenderer>() : null;
        logText = tar.GetComponentInChildren<TextMesh>();
    }

    public override void OnInspectorGUI()
    {

        //base.OnInspectorGUI();

        GUILayout.BeginVertical("Box");
        GUILayout.Label("Select Show Material");
       
        if (previewMesh != null)
        {
            previewMesh.sharedMaterial = (Material)EditorGUILayout.ObjectField("PreViewMaterial", previewMesh.sharedMaterial, typeof(Material), false);
            if (logText != null)
            {
                logText.text = EditorGUILayout.TextField("view info", logText.text);
            }
            if (GUILayout.Button("save"))
            {
               
            }
        }
        else
        {
            GUILayout.Label("didnt found Mesh");
        }
      
        GUILayout.EndVertical();
    }
}
