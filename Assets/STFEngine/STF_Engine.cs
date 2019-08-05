using STFEngine.Module;
using UnityEngine;

namespace STFEngine
{
    [RequireComponent(typeof(STFEngine_CameraMan))]
    public class STF_Engine : MonoBehaviour
    {
        private STFEngine_CameraMan mCameraCtrl;
        private static STF_Engine ins;
        public static STF_Engine mIns
        {
            get
            {
                if (ins == null)
                {

                }
                return ins;
            }
        }
        // Use this for initialization
        void Start()
        {
            ins = this;
            mCameraCtrl = GetComponent<STFEngine_CameraMan>();
        }

        public STFEngine_CameraMan getCameraSys()
        {
            return mCameraCtrl;
        }
    }
}

