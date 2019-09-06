using System;
using UnityEngine;

namespace STFEngine.Module.AR
{
    public interface ISTF_ARReferencePoint
    {
        Transform GetU3DTransform { get;  }
        IntPtr m_AnchorNativeHandle { get;  }
    }
}