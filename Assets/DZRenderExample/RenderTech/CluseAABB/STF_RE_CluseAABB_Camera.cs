using System.Runtime.InteropServices;
using UnityEngine;

namespace STFEngine.Render.Example
{
    [ExecuteInEditMode]
    public class STF_RE_CluseAABB_Camera : MonoBehaviour
    {
        struct AABB
        {
            public Vector4 Min;
            public Vector4 Max;
        };
        /// <summary>
        /// Cluse Data  Will Fill Buffer
        /// </summary>
        public struct CD_DIM
        {
            public float fieldOfViewY;
            public float zNear;
            public float zFar;

            public float sD;
            public float logDimY;
            public float logDepth;

            public int clusterDimX;
            public int clusterDimY;
            public int clusterDimZ;
            public int clusterDimXYZ;
        };

        public int m_ClusterGridBlockSize;
        public CD_DIM m_DimData;
        public Camera _camera;
        private RenderTexture _rt_Color;
        private RenderTexture _rt_Depth;
        private ComputeBuffer cb_ClusterAABBs;
        public Material mtlDebugCluster;
        private ComputeShader cs_ComputeClusterAABB;

        private void Start()
        {
            _rt_Color = new RenderTexture(Screen.width, Screen.height, 24);
            //深度图不需要Gram
            _rt_Depth = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);

            CalculateMDim(_camera);

            int stride = Marshal.SizeOf(typeof(AABB));
            cb_ClusterAABBs = new ComputeBuffer(m_DimData.clusterDimXYZ, stride);
        }


        private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            Graphics.SetRenderTarget(_rt_Color.colorBuffer, _rt_Depth.depthBuffer);
            GL.Clear(true, true, Color.gray);
            //UpdateClusterCBuffer()
            //Pass_ComputeClusterAABB();
            Pass_DebugCluster();
            Graphics.Blit(_rt_Color, destTexture);
        }



        void CalculateMDim(Camera cam)
        {
            // The half-angle of the field of view in the Y-direction.
            //Degree 2 Radiance:  Param.CameraInfo.Property.Perspective.fFovAngleY * 0.5f;
            float fieldOfViewY = cam.fieldOfView * Mathf.Deg2Rad * 0.5f;
            // Param.CameraInfo.Property.Perspective.fMinVisibleDistance;
            float zNear = cam.nearClipPlane;
            // Param.CameraInfo.Property.Perspective.fMaxVisibleDistance;
            float zFar = cam.farClipPlane;

            // Number of clusters in the screen X direction.
            int clusterDimX = Mathf.CeilToInt(Screen.width / (float)m_ClusterGridBlockSize);
            // Number of clusters in the screen Y direction.
            int clusterDimY = Mathf.CeilToInt(Screen.height / (float)m_ClusterGridBlockSize);

            // The depth of the cluster grid during clustered rendering is dependent on the 
            // number of clusters subdivisions in the screen Y direction.
            // Source: Clustered Deferred and Forward Shading (2012) (Ola Olsson, Markus Billeter, Ulf Assarsson).
            float sD = 2.0f * Mathf.Tan(fieldOfViewY) / (float)clusterDimY;
            float logDimY = 1.0f / Mathf.Log(1.0f + sD);

            float logDepth = Mathf.Log(zFar / zNear);
            int clusterDimZ = Mathf.FloorToInt(logDepth * logDimY);

            m_DimData.zNear = zNear;
            m_DimData.zFar = zFar;
            m_DimData.sD = sD;
            m_DimData.fieldOfViewY = fieldOfViewY;
            m_DimData.logDepth = logDepth;
            m_DimData.logDimY = logDimY;
            m_DimData.clusterDimX = clusterDimX;
            m_DimData.clusterDimY = clusterDimY;
            m_DimData.clusterDimZ = clusterDimZ;
            m_DimData.clusterDimXYZ = clusterDimX * clusterDimY * clusterDimZ;
        }


        void UpdateClusterCBuffer(ComputeShader cs)
        {
            int[] gridDims = { m_DimData.clusterDimX, m_DimData.clusterDimY, m_DimData.clusterDimZ };
            int[] sizes = { m_ClusterGridBlockSize, m_ClusterGridBlockSize };
            Vector4 screenDim = new Vector4((float)Screen.width, (float)Screen.height, 1.0f / Screen.width, 1.0f / Screen.height);
            float viewNear = m_DimData.zNear;

            cs.SetInts("ClusterCB_GridDim", gridDims);
            cs.SetFloat("ClusterCB_ViewNear", viewNear);
            cs.SetInts("ClusterCB_Size", sizes);
            cs.SetFloat("ClusterCB_NearK", 1.0f + m_DimData.sD);
            cs.SetFloat("ClusterCB_LogGridDimY", m_DimData.logDimY);
            cs.SetVector("ClusterCB_ScreenDimensions", screenDim);
        }


        void Pass_ComputeClusterAABB()
        {
            var projectionMatrix = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, false);
            var projectionMatrixInvers = projectionMatrix.inverse;
            cs_ComputeClusterAABB.SetMatrix("_InverseProjectionMatrix", projectionMatrixInvers);

            UpdateClusterCBuffer(cs_ComputeClusterAABB);

            int threadGroups = Mathf.CeilToInt(m_DimData.clusterDimXYZ / 1024.0f);

            int kernel = cs_ComputeClusterAABB.FindKernel("CSMain");
            cs_ComputeClusterAABB.SetBuffer(kernel, "RWClusterAABBs", cb_ClusterAABBs);
            cs_ComputeClusterAABB.Dispatch(kernel, threadGroups, 1, 1);
        }

        void Pass_DebugCluster()
        {
            GL.wireframe = true;

            mtlDebugCluster.SetBuffer("ClusterAABBs", cb_ClusterAABBs);

            mtlDebugCluster.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Points, m_DimData.clusterDimXYZ);

            GL.wireframe = false;
        }
    }
}

