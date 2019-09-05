
using UnityEditor;
using UnityEngine;

namespace STFEngine.Util.Editor
{

    class DZScriptError_Editor : EditorWindow
    {
        [MenuItem("STFEngine/SceneTool/DZScriptError_Remove")]
        static void OpenMoveSeleWin()
        {
            DZScriptError_Editor win = (DZScriptError_Editor)EditorWindow.GetWindow(typeof(DZScriptError_Editor), false, "DZEngine_SceneObjHandle_Win", true);
            win.Show();
        }


        private GameObject currMoveToObj;
        private Vector2 mScrollPos;
        void OnGUI()
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Select will Check's Obj");
            currMoveToObj = (GameObject)EditorGUILayout.ObjectField("MObj", currMoveToObj, typeof(GameObject), true);



            if (currMoveToObj != null)
            {
                int vlenChild = currMoveToObj.transform.childCount;

                if (GUILayout.Button("check and remove error"))
                {
                    for (int i = 0; i < vlenChild; i++)
                    {
                        Transform vs = currMoveToObj.transform.GetChild(i);
                        GameObject vItem = vs.gameObject;
                        SerializedObject so = new SerializedObject(vItem);
                        var soProperties = so.FindProperty("m_Component");
                        var components = vItem.GetComponents<Component>();
                        int propertyIndex = 0;
                        foreach (var c in components)
                        {
                            if (c == null)
                            {
                                soProperties.DeleteArrayElementAtIndex(propertyIndex);
                            }
                            ++propertyIndex;
                        }
                        so.ApplyModifiedProperties();
                    }

                }


            }
            GUILayout.EndVertical();
        }
    }
}