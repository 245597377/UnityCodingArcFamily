using UnityEngine;

namespace STFEngine.Module.AR
{
    public interface ISTF_ARReferencePointManager
    {
        ISTF_ARReferencePoint GetReferenPoint(Vector3 pPos,Quaternion pRot);
    }
}