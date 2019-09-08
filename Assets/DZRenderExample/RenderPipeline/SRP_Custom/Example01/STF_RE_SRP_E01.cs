using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace STFEngine.Render.Example.ScriptRenderPipeline
{
    public class STF_RE_SRP_E01: UnityEngine.Rendering.RenderPipeline
    {
        
        [NonSerialized]
        private CommandBuffer _cmdBf;
        Material errorMaterial;
        private CullingResults outCull;
        private bool mIsStere = false;
        
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
                _cmdBf = new CommandBuffer()
                {
                    name = "Render Camera"
                };
            }
          
            for (int i = 0,UPPER = cameras.Length; i < UPPER; i++)
            {
                BeginCameraRendering(context, cameras[i]);
                RenderSingleCamera(context,cameras[i]);
                EndCameraRendering(context, cameras[i]);
            }
        }

        private void RenderSingleCamera(ScriptableRenderContext pRenderContext, Camera pCamera)
        {
            if (pCamera.isActiveAndEnabled)
            {
              
                pRenderContext.SetupCameraProperties(pCamera, mIsStere);
                
                _cmdBf.name = "Render Camera";
                _cmdBf.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                CameraClearFlags vclearFlag = pCamera.clearFlags;
                _cmdBf.ClearRenderTarget(
                    (vclearFlag & CameraClearFlags.Depth) != 0,
                    (vclearFlag & CameraClearFlags.Color) != 0,
                    pCamera.backgroundColor);
                pRenderContext.ExecuteCommandBuffer(_cmdBf);
                _cmdBf.Clear();
                
                pRenderContext.DrawSkybox(pCamera);


                if (!pCamera.TryGetCullingParameters(out var vCullParameters))
                {
                    return;
                }
                vCullParameters.isOrthographic   = false;
                outCull = pRenderContext.Cull(ref vCullParameters);
                int length =  outCull.visibleLights.Length;
//                Debug.Log(length);
              
                FilteringSettings vFilterSetting = new FilteringSettings()
                {
                    renderQueueRange = RenderQueueRange.opaque,
                    layerMask = ~0
                };
                SortingSettings sortingSettings = new SortingSettings(pCamera) { criteria = SortingCriteria.RenderQueue };
                DrawingSettings vDrawingSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"),sortingSettings);
                vDrawingSettings.SetShaderPassName(0,new ShaderTagId("SRPDefaultUnlit"));
                vDrawingSettings.overrideMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
                pRenderContext.DrawRenderers(outCull,ref vDrawingSettings,ref vFilterSetting);
               
              
                pRenderContext.Submit();
            }
        }
    }
}