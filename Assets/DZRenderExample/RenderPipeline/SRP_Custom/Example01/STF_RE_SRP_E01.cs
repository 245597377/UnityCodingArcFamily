using UnityEngine;
using UnityEngine.Rendering;

namespace STFEngine.Render.Example.ScriptRenderPipeline
{
    public class STF_RE_SRP_E01: UnityEngine.Rendering.RenderPipeline
    {
        private CommandBuffer _cmdBf;
        protected virtual void Dispose(bool disposing)
        {
            if (_cmdBf != null)
            {
                _cmdBf.Dispose();
            }
        }
      
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            //base.Render(context,cameras);
            if (_cmdBf == null)
            {
                _cmdBf = new CommandBuffer();
            }

            for (int i = 0,UPPER = cameras.Length; i < UPPER; i++)
            {
                Camera vCamera = cameras[i];
                if (vCamera.isActiveAndEnabled)
                {
                    //TODO :  支持鱼眼
                    context.SetupCameraProperties(vCamera, false);
                    _cmdBf.ClearRenderTarget(true,true,vCamera.backgroundColor);
                    context.ExecuteCommandBuffer(_cmdBf);
                    _cmdBf.Clear();
                    context.Submit();
                }
                
                
            }
            
        }
    }
}