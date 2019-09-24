using UnityEngine;

namespace STFEngine.Module.AR
{
    /// <summary>
    /// AR控制器抽象
    /// </summary>
    public interface ISTF_ARControl
    {
        /// <summary>
        /// 当前锚点管理器
        /// </summary>
        /// <returns></returns>
        ISTF_ARReferencePointManager getReferenPointManager();

        /// <summary>
        /// 获得平面管理器
        /// </summary>
        /// <returns></returns>
        ISTF_ARPanelManager getARPanelManager();

        /// <summary>
        /// 当前AR射线管理器
        /// </summary>
        /// <returns></returns>
        ISTF_ARRaycastManager GetArRaycastManager();

        /// <summary>
        /// 当前虚拟摄像机管理者
        /// </summary>
        /// <returns></returns>
        ISTF_ARCamera getCamesePoseManager();

        /// <summary>
        /// 开启纹理数据
        /// </summary>
        void StartAR();

        /// <summary>
        /// 获取当前相机的纹理数据
        /// </summary>
        /// <param name="pImgDataBuffer"></param>
        /// <param name="pImgDataOutput2"></param>
        /// <returns></returns>
        STF_AR_ApiArStatus ArFrameGetImageData(byte[] pImgDataBuffer, byte[] pImgDataOutput2);

        /// <summary>
        /// 是否完成AR会话创建
        /// </summary>
        /// <returns></returns>
        bool isSessionCreateComplety();

        /// <summary>
        /// 获得当前背景宽高
        /// </summary>
        /// <param name="pwidth"></param>
        /// <param name="pHeight"></param>
        void ArFrameGetImageParams(out float pwidth, out float pHeight);
    }
    
}