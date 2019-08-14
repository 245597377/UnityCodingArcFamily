
using System.Runtime.InteropServices;
using UnityEngine;

namespace STFEngine.Render.Example
{
    
    public class STF_RE_ComputerShaderDemo01 : MonoBehaviour
    {
           //自定义结构用于基础数据在cpu和gpu运算间传递
        private struct Particle
        {
            public Vector3 position;
            public Vector3 velocity;
        }
        public Transform mSpawnCenter;
        public Transform mTarget1;
        public Transform mTarget2;
        public float mMinMoveDistance = 1.0f;
        public ComputeShader computeShader;
        public Material material;
        public int size = 1024000;
        public float spawnRange = 1.0f;
        public float moveSpeedScale = 0.2f;
        private ComputeBuffer particles;

        const int WARP_SIZE = 1024;

        //粒子数量
       
        private int stride;
        private int warpCount;
        private int kernelIndex;
        private Particle[] initBuffer;
        
        // Use this for initialization
        private  void Start () 
        {
            //运算次数
            warpCount = Mathf.CeilToInt((float)size / WARP_SIZE);
            
            //计算数据所占内存大小
            stride = Marshal.SizeOf(typeof(Particle));
            //ComputeBuffer 用于向ComputeShader 传递自定义结构
            particles = new ComputeBuffer(size, stride);

            initBuffer = new Particle[size];

            for (var i = 0; i < size; i++)
            {
                initBuffer[i] = new Particle
                {
                    position = mSpawnCenter.position + Random.insideUnitSphere * spawnRange, 
                    velocity = Vector3.zero
                };
            }
            //写入 数据到 computeBuffer
            //之后会用于在 cpu 到 compute shader 到 shader 直接传递参数
            particles.SetData(initBuffer);
            
            //Compute Shader 里可执行函数的 id, 后面update里调用
            //Update 为Compute Shader 里函数的 函数名
            kernelIndex = computeShader.FindKernel("Update");
            
            //为compute shader 的 函数 写入数据buffer
            computeShader.SetBuffer(kernelIndex, "Particles", particles);
            
            //为shader 写入数据buffer
            material.SetBuffer("Particles", particles);
        }
	    
        // Update is called once per frame
        private void Update () 
        {

            if (Input.GetKeyDown(KeyCode.R))
            {
                //重置buffer 往compute shader 里更新自定义数据也是这样写
                particles.SetData(initBuffer);
            }
            var centerPosition1 = GetCenterPosition1();
            var centerPosition2 = GetCenterPosition2();
            var vIsCanMove = Vector3.Distance(mTarget1.position, mTarget2.position) > mMinMoveDistance? 1:0;
            computeShader.SetInt("shouldMove", vIsCanMove);
            computeShader.SetFloats("centerPosition1", centerPosition1);
            computeShader.SetFloats("centerPosition2", centerPosition2);
            computeShader.SetFloat("dt", Time.deltaTime);
            computeShader.SetFloat("SpeedScale", moveSpeedScale);
            //执行一次compute shader 里的函数
            computeShader.Dispatch(kernelIndex, warpCount, 1, 1);
        }

        private float[] GetCenterPosition1()
        {
            var v = mTarget1.position;
            return new float[] { v.x, v.y,v.z };
        }
        
        private float[] GetCenterPosition2()
        {
            var v = mTarget2.position;
            return new float[] { v.x, v.y,v.z };
        }
        
        //渲染效果
        private void OnRenderObject()
        {
            //写入材质球 pass
            material.SetPass(0);
            //渲染粒子
            Graphics.DrawProceduralNow(MeshTopology.Points, 1, size);
        }

        private void OnDestroy()
        {
            particles?.Release();
        }
        
    }
}