using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace STFEngine.Render.Example.ScriptRenderPipeline
{
    public class STF_RE_SRP_E01: UnityEngine.Rendering.RenderPipeline
    {
        
        [NonSerialized]
        private CommandBuffer _cmdBf;
        Material errorMaterial;
        private CullingResults outCull;
        private bool mIsStere = false;
        private FilteringSettings m_FilteringSettings;
        private RenderStateBlock m_RenderStateBlock;
        private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        private StencilState m_DefaultStencilState;
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
                
#if UNITY_EDITOR
                // Emit scene view UI
                if (pCamera.cameraType == CameraType.SceneView)
                    ScriptableRenderContext.EmitWorldGeometryForSceneView(pCamera);
                              
#endif
                pRenderContext.DrawSkybox(pCamera);


                if (!pCamera.TryGetCullingParameters(out var vCullParameters))
                {
                    return;
                }
                vCullParameters.isOrthographic   = false;
                outCull = pRenderContext.Cull(ref vCullParameters);
                int length =  outCull.visibleLights.Length;
//                Debug.Log(length);
                m_ShaderTagIdList = new List<ShaderTagId>();
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, ~0);
                
                StencilStateData stencilData = new StencilStateData();
                m_DefaultStencilState = StencilState.defaultValue;
                m_DefaultStencilState.enabled = stencilData.overrideStencilState;
                m_DefaultStencilState.SetCompareFunction(stencilData.stencilCompareFunction);
                m_DefaultStencilState.SetPassOperation(stencilData.passOperation);
                m_DefaultStencilState.SetFailOperation(stencilData.failOperation);
                m_DefaultStencilState.SetZFailOperation(stencilData.zFailOperation);
                
                m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
                m_RenderStateBlock.stencilReference = stencilData.stencilReference;
                m_RenderStateBlock.mask = RenderStateMask.Stencil;
                m_RenderStateBlock.stencilState = m_DefaultStencilState;
                
                SortingSettings sortingSettings = new SortingSettings(pCamera) { criteria = SortingCriteria.CommonOpaque };
                DrawingSettings settings = new DrawingSettings(m_ShaderTagIdList[0], sortingSettings)
                {
                    perObjectData = PerObjectData.None,
                    enableInstancing = true,
                    mainLightIndex = 0,
                    enableDynamicBatching = false,
                };
            
                for (int i = 1; i < m_ShaderTagIdList.Count; ++i)
                    settings.SetShaderPassName(i, m_ShaderTagIdList[i]);

                pRenderContext.DrawRenderers(outCull,ref settings,ref m_FilteringSettings);
               
              
                pRenderContext.Submit();
            }
        }

    }
}