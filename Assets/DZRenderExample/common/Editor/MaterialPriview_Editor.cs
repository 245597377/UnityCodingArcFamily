using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialPreview))]
public class MaterialPriview_Editor : Editor
{
    // Start is called before the first frame update
    private MaterialPreview tar;

    private void OnEnable()
    {
        tar = target as MaterialPreview;
        tar.RefreshMesh();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Select Show Material");
        if (tar.logText != null)
        {
            tar.logText.text = EditorGUILayout.TextField("view info", tar.logText.text);
        }

        if (tar.previewMesh != null)
        {
            GUILayout.Space(5.0f);
            GUILayout.BeginVertical("Box");

            GUILayout.Space(2.0f);
            GUILayout.Label("Mat Property:");
            tar.StandMat = (Material) EditorGUILayout.ObjectField("StandMat", tar.StandMat, typeof(Material), false);
            tar.LWRPMat = (Material) EditorGUILayout.ObjectField("LWRPMat", tar.LWRPMat, typeof(Material), false);
            tar.HDRPMat = (Material) EditorGUILayout.ObjectField("HDRPMat", tar.HDRPMat, typeof(Material), false);
            tar.CustomMat = (Material) EditorGUILayout.ObjectField("CustomMat", tar.CustomMat, typeof(Material), false);
            GUILayout.EndVertical();
            GUILayout.Space(5.0f);
        }
        else
        {
            GUILayout.Label("didnt found Mesh");
        }

        MaterialPreview.ERenderPipeline vSRP =
            (MaterialPreview.ERenderPipeline) EditorGUILayout.EnumFlagsField(tar.mCurrShowPipeline);
        if (vSRP != tar.mCurrShowPipeline)
        {
            tar.ChangeRenderPipeline(vSRP);
            EditorUtility.SetDirty(tar.gameObject);
        }

        if (GUILayout.Button("Force Refresh"))
        {
            tar.RefreshByCurrPipeLine();
            EditorUtility.SetDirty(tar.gameObject);
        }

        if (GUILayout.Button("Force Refresh All MatPreview"))
        {
            MaterialPreview[] vItemArr = GameObject.FindObjectsOfType<MaterialPreview>();
            for (int i = 0, len = vItemArr.Length; i < len; i++)
            {
                vItemArr[i].RefreshByCurrPipeLine();
                EditorUtility.SetDirty(vItemArr[i].gameObject);
            }
        }

        GUILayout.EndVertical();
    }
}