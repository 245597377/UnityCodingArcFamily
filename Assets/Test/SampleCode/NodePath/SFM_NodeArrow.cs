using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct NodeSeal
{
    public Transform mNodeTarget;
    public string mshowName;
}

public class SFM_NodeArrow : MonoBehaviour
{
    public SFM_SmaArrow smaUIArrow;
    public GameObject BigArrow;
    public Text mShowText;
    public NodeSeal[] vNodeList;
    public Vector3 posLocPlayer;
    public Transform PlayerCtrl;
    public float MinSizeNode;
    public int currNodeIndex;
    public GameObject overEffect;
    private bool isOver;
    // Start is called before the first frame update
    void Start()
    {
        isOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOver)
        {
            return;
        }
        if (PlayerCtrl == null)
        {
            return;
        }

        if (vNodeList == null || vNodeList.Length == 0 )
        {
            return;
        }

        if (currNodeIndex == vNodeList.Length -1)
        {
            GameObject.Instantiate(overEffect,PlayerCtrl.position,PlayerCtrl.rotation);
            isOver = true;
        }

        if(vNodeList[currNodeIndex].mNodeTarget == null )
                return;
        SyncLoc();
        Vector2 v1 = new Vector2(vNodeList[currNodeIndex].mNodeTarget.position.x,
            vNodeList[currNodeIndex].mNodeTarget.position.z);
        Vector2 v2 = new Vector2(PlayerCtrl.position.x,
            PlayerCtrl.position.z);
        if (Vector2.Distance(v1,v2) < MinSizeNode)
        {
            currNodeIndex++;
        }  
        
    }
    
    

    private void SyncLoc()
    {
        Vector3 pPos = PlayerCtrl.TransformPoint(posLocPlayer);
        BigArrow.transform.position = new Vector3(pPos.x, vNodeList[currNodeIndex].mNodeTarget.position.y, pPos.z);
        BigArrow.transform.LookAt(vNodeList[currNodeIndex].mNodeTarget);
        Debug.DrawLine(BigArrow.transform.position,vNodeList[currNodeIndex].mNodeTarget.position);

        if (!IsInView(BigArrow.transform.position))
        {
            smaUIArrow.refresh(BigArrow.transform.position);
            smaUIArrow.gameObject.SetActive(true);
        }
        else
        {
            smaUIArrow.gameObject.SetActive(false);
        }
    }
    
    public bool IsInView(Vector3 worldPos)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面
 
 
        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }
    
}
