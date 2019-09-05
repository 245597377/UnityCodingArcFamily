using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace STFEngine.Render.Example.ScriptRenderPipeline
{
    public class STF_Re_SRP_Editor
    {
        [MenuItem("Assets/STFEngine/Rendering/RenderPipeline/Create_SRP_RE_Srp01")]
        public static void create_STF_RenderExample_SRP_01()
        {
            
            SRP_01_CreateAction vAssetCreateActionID = ScriptableObject.CreateInstance<SRP_01_CreateAction>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,vAssetCreateActionID,"STF_Re_SRP.asset",null,null);
        }
        
        /// <summary>
        /// Unity calls this function when the user accepts an edited name, either by pressing the Enter key or by losing the keyboard input focus.
        /// @author: xdz
        /// </summary>
        public class SRP_01_CreateAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                STF_RE_SRP_E01_Asset vasset = CreateInstance<STF_RE_SRP_E01_Asset>();
                AssetDatabase.CreateAsset(vasset,pathName);
            }
        }
    }
}