using UnityEngine;
using UnityEditor;

namespace STFEngine.Util.Editor
{

    public class SceneObjectHandle_Editor : EditorWindow
    {
        [MenuItem("DZEngineTool/SceneTool/MoveSelectEd_Obj")]
        static void OpenMoveSeleWin()
        {
            SceneObjectHandle_Editor win = (SceneObjectHandle_Editor)EditorWindow.GetWindow(typeof(SceneObjectHandle_Editor), false, "DZEngine_SceneObjHandle_Win", true);
            win.Show();
        }


        private GameObject currMoveToObj;
        private Vector2 mScrollPos;
        void OnGUI()
        {
            GUILayout.BeginVertical("Box");

            currMoveToObj = (GameObject)EditorGUILayout.ObjectField("MObj", currMoveToObj, typeof(GameObject), true);
            GUILayout.BeginVertical("Box");

            GUILayout.Label("Select will move's Obj");
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, GUILayout.Width(200.0f), GUILayout.Height(300.0f));

            UnityEngine.Object[] vSeleObjs = Selection.objects;
            for (int i = 0; i < vSeleObjs.Length; i++)
            {
                EditorGUILayout.ObjectField(vSeleObjs[i].name, vSeleObjs[i], typeof(GameObject), true);
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Move To MObj"))
            {
                if (currMoveToObj != null)
                {
                    for (int i = 0; i < vSeleObjs.Length; i++)
                    {
                        GameObject vtr = (vSeleObjs[i] as GameObject);
                        vtr.transform.parent = currMoveToObj.transform;
                    }
                }

            }
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

    }
}