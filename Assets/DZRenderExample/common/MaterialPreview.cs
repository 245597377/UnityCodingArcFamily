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

    public MeshRenderer[] previewMesh;

    public TextMesh logText;

    public ERenderPipeline mCurrShowPipeline;
    public void RefreshMesh()
    {
        logText = GetComponentInChildren<TextMesh>();
    }

    public void RefreshByCurrPipeLine()
    {
        ChangeRenderPipeline(mCurrShowPipeline);
    }

    public void ChangeRenderPipeline(ERenderPipeline pipeLine)
    {
        mCurrShowPipeline = pipeLine;
        if(previewMesh == null || previewMesh.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < previewMesh.Length; i++)
        {
            switch(mCurrShowPipeline)
            {
                case  ERenderPipeline.Stand:
                    previewMesh[i].sharedMaterial = StandMat;
                    break;

                case  ERenderPipeline.LWRP:
                    previewMesh[i].sharedMaterial = LWRPMat;
                    break;

                case  ERenderPipeline.HDRP:
                    previewMesh[i].sharedMaterial = HDRPMat;
                    break;

                case  ERenderPipeline.Custom:
                    previewMesh[i].sharedMaterial = CustomMat;
                    break;
            }
        }
      
    }
   
}
