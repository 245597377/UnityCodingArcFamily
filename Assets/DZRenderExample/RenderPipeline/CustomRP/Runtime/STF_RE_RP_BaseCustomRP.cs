using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace STFEngine.Render.Example.RenderPipeline
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class STF_RE_RP_BaseCustomRP : MonoBehaviour
    {
        // Start is called before the first frame update
        private Camera _camera;
        private RenderTexture _rt;
        public Material mSkyBoxMaterial;
        public string mCornelShaderKeyName;
        private Mesh mSkyBoxMesh;
        private readonly Vector4[]  _mCorNel = new Vector4[4];
       

        private void Start()
        {
            _rt = new RenderTexture(Screen.width,Screen.height,24);
        }

        private void OnPreRender()
        {
            GL.Clear(true,true,Color.grey);
        }

        private void OnPostRender()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }
            Graphics.SetRenderTarget(_rt);
            GL.Clear(true,true,Color.grey);
            DrawMesh();
            DrawSkyBox();
            Graphics.Blit(_rt,_camera.targetTexture);
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

        private void DrawSkyBox()
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
            Graphics.DrawMeshNow(SkyBoxMesh, Matrix4x4.identity);
        }
    }
}

