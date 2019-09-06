using UnityEngine;

namespace STFEngine.Module.AR
{
    public interface ISTF_ARRaycastManager
    {
        ISTF_ARReferencePoint GetRayPanelReferencePoint(Vector3 pPos,Quaternion pRot);
    }
}