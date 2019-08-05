using UnityEngine;

namespace STFEngine.Module
{
    public class STFEngine_CameraMan : MonoBehaviour
    {
        public Camera mMainCamera;
        /// <summary>
        /// 用于展示Mesh的Camera;
        /// @todo:  自定义管线后Feature可以去去掉
        /// </summary>
        public Camera mUIMeshCamera;
        // Use this for initialization
        void RefreshCamera(Camera pCamera)
        {
            mMainCamera = pCamera;
        }

        public Transform GetCameraTatget()
        {
            if (mMainCamera == null)
            {
                return null;
            }
            return mMainCamera.transform;
        }

        public void Preview3DObjOnUI(GameObject pObj)
        {
            pObj.transform.position = mUIMeshCamera.transform.TransformPoint(Vector3.forward * 10f);
        }
    }
}
