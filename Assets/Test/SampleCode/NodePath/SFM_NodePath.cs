using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SFM_NodePath : MonoBehaviour
{
    public NodeSeal[] mNodeList;
    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
        if (mNodeList == null)
        {
            return;
        }
        for (int i = 0; i < mNodeList.Length -1 ; i++)
        {
            Gizmos.color = Color.blue;
            ;
            if(mNodeList[i].mNodeTarget == null || mNodeList[i+1].mNodeTarget==null)
                return;
            ;
            Gizmos.DrawLine(mNodeList[i].mNodeTarget.position,mNodeList[i+1].mNodeTarget.position);
        }
       
    }
}
