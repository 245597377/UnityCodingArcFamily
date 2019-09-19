using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STFEngine.Module.AR
{
    /// <summary>
    /// AR相机代理
    /// </summary>
    public interface ISTF_ARCamera
    {
        void SyncCamearPose(Vector3 Position, Vector3 Rot);
        Ray ScreenPointToRay(Vector3 mousePosition);
        Transform GetUnityTransform { get; set; }
        
     
    }
}

