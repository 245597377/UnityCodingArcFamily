 using UnityEngine.Rendering;
 

 
 
 namespace STFEngine.Render.Example.ScriptRenderPipeline
 {
    public class STF_RE_SRP_E01_Asset:RenderPipelineAsset
    {
        protected override UnityEngine.Rendering.RenderPipeline CreatePipeline()
        {
           return new STF_RE_SRP_E01();
        }
        
        
    }
}