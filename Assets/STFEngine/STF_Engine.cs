using STFEngine.Core;
using STFEngine.Module;
using UnityEngine;

namespace STFEngine
{
    [RequireComponent(typeof(STFEngine_CameraMan))]
    public class STF_Engine : STF_SingletonMonoBehaviour<STF_Engine>
    {
        private STFEngine_CameraMan mCameraCtrl;

        // Use this for initialization
        void Start()
        {
            mCameraCtrl = GetComponent<STFEngine_CameraMan>();
        }

        
        public STFEngine_CameraMan getCameraSys()
        {
            return mCameraCtrl;
        }
    }
}

