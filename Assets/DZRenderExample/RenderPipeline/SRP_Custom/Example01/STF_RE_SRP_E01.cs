using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace STFEngine.Render.Example.ScriptRenderPipeline
{
    public class STF_RE_SRP_E01: UnityEngine.Rendering.RenderPipeline
    {
        [NonSerialized]
        private CommandBuffer _cmdBf;
        protected virtual void Dispose(bool disposing)
        {
            if (_cmdBf != null)
            {
                _cmdBf.Dispose();
                _cmdBf.Release();
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
                RenderSingleCamera(context,cameras[i]);
            }
        }

        private void RenderSingleCamera(ScriptableRenderContext pRenderContext, Camera pCamera)
        {
            if (pCamera.isActiveAndEnabled)
            {
                //TODO :  支持鱼眼
                pRenderContext.SetupCameraProperties(pCamera, false);
                    
                CameraClearFlags vclearFlag = pCamera.clearFlags;
                _cmdBf.ClearRenderTarget(
                    (vclearFlag & CameraClearFlags.Depth) != 0,
                    (vclearFlag & CameraClearFlags.Color) != 0,
                    pCamera.backgroundColor);
                
                pRenderContext.DrawSkybox(pCamera);

                pRenderContext.ExecuteCommandBuffer(_cmdBf);
                _cmdBf.Clear();
                pRenderContext.Submit();
            }
        }
        
        
        
    }
}