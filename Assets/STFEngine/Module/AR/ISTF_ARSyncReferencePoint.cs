using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace STFEngine.Module.AR
{
    public interface ISTF_ARSyncReferencePoint
    {
        
        void hideDebugMesh();
        Transform transform { get; set; }
        Object gameObject { get; set; }

        void RegeistServerKey();
        bool isCanCreatFromClient();
        void ReBindCloudCreateSuccessEvent(Action<string> registerRoomCloudOver);
        void ReBindCloudCreateFailureEvent(Action<string> registerRoomCloudOver);
        void CreatFromClient(ISTF_ARReferencePoint pAnchor);
        bool IsSetupCloudPoint();
        void ReBindSyncFromCloudSuccessEvent(Action<string> loadRoomCloudAncOver);
        void ReBindSyncFromCloudFailureEvent(Action<string> loadRoomCloudAncOver);
        void ReaderFormCloud(string pmsgMCloudAncId);
        void clear();
    }
}