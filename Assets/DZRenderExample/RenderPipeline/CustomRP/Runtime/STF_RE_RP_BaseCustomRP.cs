using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace STFEngine.Render.Example.RenderPipeline
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class STF_RE_RP_BaseCustomRP : MonoBehaviour
    {
        private static int _DepthTextureID = Shader.PropertyToID("_DepthTexture");
       
        private Camera _camera;
        private RenderTexture _cameraTarget;
        public Material mSkyBoxMaterial;
        public string mCornelShaderKeyName;
        private Mesh mSkyBoxMesh;
        private readonly Vector4[]  _mCorNel = new Vector4[4];
        //----------------Deffered Light ----------------
        private RenderTexture _depthTexture;
        
        /// <summary>
        /// <para>RT0, ARGB32 format: Diffuse color (RGB), occlusion (A).</para>
        /// <para>RT1, ARGB32 format: Specular color (RGB), roughness (A).</para>
        /// <para>RT2, ARGB2101010 format: World space normal (RGB), unused (A).</para>
        /// <para>RT3, ARGB2101010 (non-HDR) or ARGBHalf (HDR) format: Emission + lighting + lightmaps</para>
        /// <para>+ reflection probes</para>
        /// <para>Depth+Stencil buffer</para>
        /// </summary>
        private RenderTexture[] _gbufferTexture = new RenderTexture[4];
        private RenderBuffer[] _gbuffers;
        private int[] _gbufferIDS;
        private bool isInit = false;
        private void Start()
        {
            isInit = false;
            DoInit();
        }

        private void DoInit()
        {
            _cameraTarget = new RenderTexture(Screen.width,Screen.height,0);
            _gbufferTexture = new RenderTexture[]
            {
                new RenderTexture(Screen.width,Screen.height,0,RenderTextureFormat.ARGBHalf,RenderTextureReadWrite.Linear), 
                new RenderTexture(Screen.width,Screen.height,0,RenderTextureFormat.ARGBHalf,RenderTextureReadWrite.Linear), 
                new RenderTexture(Screen.width,Screen.height,0,RenderTextureFormat.ARGBHalf,RenderTextureReadWrite.Linear), 
                new RenderTexture(Screen.width,Screen.height,0,RenderTextureFormat.ARGBHalf,RenderTextureReadWrite.Linear), 
            };
            _depthTexture = new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.Depth,RenderTextureReadWrite.Linear);
            _gbuffers = new RenderBuffer[4];
            for (int i = 0; i < _gbufferTexture.Length; i++)
            {
                _gbuffers[i] = _gbufferTexture[i].colorBuffer;
            }

            _gbufferIDS = new int[]
            {
                Shader.PropertyToID("_GBuffer0"),
                Shader.PropertyToID("_GBuffer1"),
                Shader.PropertyToID("_GBuffer2"),
                Shader.PropertyToID("_GBuffer3")
            };
      
        }
        

     
        private void OnPostRender()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }

         
            Shader.SetGlobalTexture(_DepthTextureID, _depthTexture);
            Graphics.SetRenderTarget(_gbuffers, _depthTexture.depthBuffer);
            GL.Clear(true,true,Color.grey);
            DrawMesh();
            DrawLight(_gbufferTexture,_gbufferIDS,_cameraTarget,_camera);
            DrawSkyBox(_cameraTarget.colorBuffer,_depthTexture.depthBuffer);
            Graphics.Blit(_cameraTarget,_camera.targetTexture);
        }

        private void DrawMesh()
        {
            MeshRenderer[] vMeshList = GameObject.FindObjectsOfType<MeshRenderer>();
            Material ProMat = null;
            for (int i = 0,UPPER = vMeshList.Length; i < UPPER; i++)
            {
                MeshFilter vMesh = vMeshList[i].gameObject.GetComponent<MeshFilter>();
                if (vMesh == null)
                {
                    continue;
                }

                Mesh vMeshItem = null;
                Material vMat = null;
                #if UNITY_EDITOR    
                    vMeshItem = vMesh.sharedMesh;
                    vMat = vMeshList[i].sharedMaterial;
                #else
                    vMeshItem = vMesh.mesh;
                    vMat = vMeshList[i].material;
                #endif
                
                if (vMeshItem == null) continue;
                if (vMat != null)
                {
                    if (vMat != ProMat)
                    {
                        vMat.SetPass(0);
                        ProMat = vMat;
                    }
                }
                
                Graphics.DrawMeshNow(vMeshItem, vMesh.transform.localToWorldMatrix);
            }
        }

        private Mesh SkyBoxMesh
        {
            get
            {
                if (mSkyBoxMesh != null)
                {
                    return mSkyBoxMesh;
                }
                mSkyBoxMesh = new Mesh();
                mSkyBoxMesh.vertices = new Vector3[]
                {
                    new Vector3(-1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(1, -1, 0)
                };
                //DX
                mSkyBoxMesh.uv = new Vector2[]
                {
                    new Vector2(0,1), 
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                }; 
                mSkyBoxMesh.SetIndices(new int[]{0,1,2,3},MeshTopology.Quads,0);
                return mSkyBoxMesh;
            }
        }

        private void DrawSkyBox(RenderBuffer pColorTarget,RenderBuffer pDepthTarget)
        {
            if (mSkyBoxMaterial == null || mCornelShaderKeyName.Equals(""))
            {
                return;
            }
            _mCorNel[0] = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.farClipPlane));
            _mCorNel[1] = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.farClipPlane));
            _mCorNel[2] = _camera.ViewportToWorldPoint(new Vector3(0, 1, _camera.farClipPlane));
            _mCorNel[3] = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.farClipPlane));
            mSkyBoxMaterial.SetVectorArray(mCornelShaderKeyName,_mCorNel);
            mSkyBoxMaterial.SetPass(0);
            Graphics.SetRenderTarget(pColorTarget, pDepthTarget);
            Graphics.DrawMeshNow(SkyBoxMesh, Matrix4x4.identity);
        }


        private static int _CurrentLightDir = Shader.PropertyToID("_CurrentLightDir");
        private static int _LightFinalColor = Shader.PropertyToID("_LightFinalColor");
        private static int _InvVPID = Shader.PropertyToID("_InvVP");
        public Material _deferrenedMat;
        public Light directionalLight;

        private void DrawLight(RenderTexture[] pGbuffersTextures, int[] pGbufferIDs, RenderTexture pTarget, Camera pCamera)
        {
            Matrix4x4 proj = GL.GetGPUProjectionMatrix(pCamera.projectionMatrix, false);
            Matrix4x4 vp = proj * pCamera.worldToCameraMatrix;
            Matrix4x4 invvp = vp.inverse;
            Shader.SetGlobalMatrix(_InvVPID, invvp);
            
            _deferrenedMat.SetVector(_CurrentLightDir, -directionalLight.transform.forward);
            _deferrenedMat.SetVector(_LightFinalColor, directionalLight.color * directionalLight.intensity);
            for (int i = 0; i < _gbufferIDS.Length; i++)
            {
                _deferrenedMat.SetTexture(_gbufferIDS[i],_gbufferTexture[i]);
            }
            Graphics.Blit(null, pTarget, _deferrenedMat, 0);
        }
        
    }
}

