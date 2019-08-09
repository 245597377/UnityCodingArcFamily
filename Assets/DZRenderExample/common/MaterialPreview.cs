using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPreview : MonoBehaviour
{
    public enum ERenderPipeline{
        Stand = 0,
        LWRP  = 1,
        HDRP  = 2,
        Custom = 4
    };

    public Material StandMat;
    public Material LWRPMat;

    public Material HDRPMat;

    public Material CustomMat;

    public MeshRenderer previewMesh;

    public TextMesh logText;

    public ERenderPipeline mCurrShowPipeline;
    public void RefreshMesh()
    {
        Transform c1 = transform.GetChild(0);
        previewMesh = c1!=null? c1.GetComponentInChildren<MeshRenderer>() : null;
        logText = GetComponentInChildren<TextMesh>();
    }

    public void RefreshByCurrPipeLine()
    {
        ChangeRenderPipeline(mCurrShowPipeline);
    }

    public void ChangeRenderPipeline(ERenderPipeline pipeLine)
    {
        mCurrShowPipeline = pipeLine;
        if(previewMesh == null)
        {
            return;
        }
        switch(mCurrShowPipeline)
        {
            case  ERenderPipeline.Stand:
                   previewMesh.sharedMaterial = StandMat;
                   break;

            case  ERenderPipeline.LWRP:
                   previewMesh.sharedMaterial = LWRPMat;
                   break;

            case  ERenderPipeline.HDRP:
                   previewMesh.sharedMaterial = HDRPMat;
                   break;

            case  ERenderPipeline.Custom:
                   previewMesh.sharedMaterial = CustomMat;
                   break;
        }
    }
   
}
