using UnityEngine;

namespace STFEngine.Module.AR
{
    /// <summary>
    /// AR控制器抽象
    /// </summary>
    public interface ISTF_ARControl
    {
        ISTF_ARReferencePointManager getReferenPointManager();
        ISTF_ARPanelManager getARPanelManager();
        ISTF_ARRaycastManager GetArRaycastManager();
        ISTF_ARCamera getCamesePoseManager();
        void StartAR();
    }
    
}