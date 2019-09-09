using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace STFEngine.Render.Example.ScriptRenderPipeline
{
    public class STF_RE_SRP_E01: UnityEngine.Rendering.RenderPipeline
    {
        
        Material errorMaterial;
        private CullingResults outCull;
        private bool mIsStere = false;
        private FilteringSettings m_FilteringSettings;
        private DrawingSettings settings;
        private RenderStateBlock m_RenderStateBlock;
        [NonSerialized]
        private List<ShaderTagId> m_ShaderTagIdList;
        private StencilState m_DefaultStencilState;
        private StencilStateData stencilData;
        const string k_RenderCameraTag = "Render Camera";
        protected virtual void Dispose(bool disposing)
        {
           
        }
      
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            //base.Render(context,cameras);
         
            if (m_ShaderTagIdList == null)
            {
                m_ShaderTagIdList = new List<ShaderTagId>();
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            }

            if (stencilData == null)
            {
                stencilData = new StencilStateData();
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
#if UNITY_EDITOR
                string tag = pCamera.name;
#else
                string tag = k_RenderCameraTag;
#endif
                CommandBuffer cmd = CommandBufferPool.Get(tag);
                using (new ProfilingSample(cmd, tag))
                {
                    cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    CameraClearFlags vclearFlag = pCamera.clearFlags;
                    cmd.ClearRenderTarget(
                        (vclearFlag & CameraClearFlags.Depth) != 0,
                        (vclearFlag & CameraClearFlags.Color) != 0,
                        pCamera.backgroundColor);
                    pRenderContext.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                
#if UNITY_EDITOR
                    // Emit scene view UI
                    if (pCamera.cameraType == CameraType.SceneView)
                        ScriptableRenderContext.EmitWorldGeometryForSceneView(pCamera);
                              
#endif
                    if (!pCamera.TryGetCullingParameters(out var vCullParameters))
                    {
                        return;
                    }
                    vCullParameters.isOrthographic   = false;
                    outCull = pRenderContext.Cull(ref vCullParameters);
                    int length =  outCull.visibleLights.Length;

                    DrawOpaque(pRenderContext,pCamera);
                    DrawSkyBox(pRenderContext,pCamera);
                    DrawTransfer(pRenderContext,pCamera);
                }
                pRenderContext.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
                pRenderContext.Submit();
            }
        }

        private void DrawOpaque(ScriptableRenderContext pRenderContext,Camera pCamera)
        {
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, ~0);
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
            settings = new DrawingSettings(m_ShaderTagIdList[0], sortingSettings)
            {
                perObjectData = PerObjectData.None,
                enableInstancing = true,
                mainLightIndex = 0,
                enableDynamicBatching = false,
            };
            
            for (int i = 1; i < m_ShaderTagIdList.Count; ++i)
                settings.SetShaderPassName(i, m_ShaderTagIdList[i]);
            
            pRenderContext.DrawRenderers(outCull,ref settings,ref m_FilteringSettings);
        }

        private void DrawSkyBox(ScriptableRenderContext pRenderContext,Camera pCamera)
        {
            pRenderContext.DrawSkybox(pCamera);
        }

        private void DrawTransfer(ScriptableRenderContext pRenderContext,Camera pCamera)
        {
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.transparent, ~0);
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
            settings = new DrawingSettings(m_ShaderTagIdList[0], sortingSettings)
            {
                perObjectData = PerObjectData.None,
                enableInstancing = true,
                mainLightIndex = 0,
                enableDynamicBatching = false,
            };
            
            for (int i = 1; i < m_ShaderTagIdList.Count; ++i)
                settings.SetShaderPassName(i, m_ShaderTagIdList[i]);
            
            pRenderContext.DrawRenderers(outCull,ref settings,ref m_FilteringSettings);
        }
      
    }
}