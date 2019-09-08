using UnityEngine;
using UnityEditor;

namespace STFEngine.Util.Editor
{
    public class CameraEditor_Tool
    {
        [MenuItem("STFEngine/DZCameraTool/Align/SceneView")]
        static void AlignToSceneView()
        {
            if (Selection.gameObjects.Length > 0)
            {
                Camera viewCame = Selection.gameObjects[0].GetComponent<Camera>();
                if (viewCame != null)
                {
                    Camera cas = UnityEditor.SceneView.lastActiveSceneView.camera;
                    if (cas == null || cas == viewCame)
                        return;
                    Transform camT = cas.transform;
                    viewCame.gameObject.transform.position = camT.position;
                    viewCame.gameObject.transform.LookAt(camT.position + camT.forward * 10);
                }
            }
        }

        [MenuItem("STFEngine/DZCameraTool/RunGameFromCurrView")]
        static void RunInCurrView()
        {
            if (Selection.gameObjects.Length > 0)
            {
                Camera viewCame = Selection.gameObjects[0].GetComponent<Camera>();
                if (viewCame != null)
                {
                    Camera cas = UnityEditor.SceneView.lastActiveSceneView.camera;
                    if (cas == null || cas == viewCame)
                        return;
                    Transform camT = cas.transform;

                    UnityEditor.EditorApplication.isPlaying = true;
                    Camera mMainCamera = Camera.main;
                    if(mMainCamera != null)
                    {
                        mMainCamera.transform.position = camT.position;
                        mMainCamera.transform.localRotation = camT.rotation;
                    }
                }
            }
        }



        [InitializeOnLoadMethod]
        static void StartInitializeOnLoadMethod()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
                && Event.current.button == 1 && Event.current.type <= EventType.MouseUp)
            {
                GameObject selectedGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                //这里可以判断selectedGameObject的条件
                if (selectedGameObject && selectedGameObject.GetComponent<Camera>() != null)
                {
                    Vector2 mousePosition = Event.current.mousePosition;

                    EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "STFEngine/", null);
                    Event.current.Use();
                }
            }
        }
    }
}

