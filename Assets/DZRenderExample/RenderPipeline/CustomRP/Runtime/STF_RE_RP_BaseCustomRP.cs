using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STFEngine.Render.Example.RenderPipeline
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class STF_RE_RP_BaseCustomRP : MonoBehaviour
    {
        // Start is called before the first frame update
        private Camera _camera;
        private RenderTexture _rt;
        private void Start()
        {
            _rt = new RenderTexture(Screen.width,Screen.height,24);
        }

        private void OnPostRender()
        {
            _camera = Camera.main;
            if (_camera != null)
            {
                Graphics.SetRenderTarget(_rt);
                GL.Clear(true,true,Color.grey);
                Graphics.Blit(_rt,_camera.targetTexture);
            }
        }
    }
}

