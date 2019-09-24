using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFM_SmaArrow : MonoBehaviour
{
    public GameObject Left;

    public GameObject right;
    public GameObject up;
    public GameObject down;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void refresh(Vector3 transformPosition)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(transformPosition);
        Vector3 dir = (transformPosition - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面
        clear();
        if (dot<0)
        {
            down.SetActive(true);
        }
        if(viewPos.x < 0)
        {
            Left.SetActive(true);
        }

        if(viewPos.x >= 1)
        {
            Left.SetActive(true);
        }
        if(viewPos.y < 0)
        {
            down.SetActive(true);
        }
        if(viewPos.y > 1)
        {
            up.SetActive(true);
        }
    }

    private void clear()
    {
        Left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        up.gameObject.SetActive(false);
        down.gameObject.SetActive(false);
    }
}
