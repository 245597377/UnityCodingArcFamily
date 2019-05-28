using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Support multiple function methods.
 *Use a delegate and enumeration.
 *Define surfaces in 3D space.
 */
public class DZ_MatheMaticalSurfaces : MonoBehaviour
{
    // Start is called before the first frame update
    const float pi = Mathf.PI;
    public Transform mPointPrefab;
    public MoveTypeEnum mMatheMaticalType;
    public Vector2 stepOffset;
    public float   scaleMove = 1.0f;

    public enum MoveTypeEnum
    {
        sin = 0,
      
        Sin_2D = 1,

        Sin_2D_XX = 2,

        muil_sin = 3,

        multi_sine2D = 4,

        Ripple = 5,
    };
  
    #region PrivateProperty Private Method
    private delegate Vector3 MoveTypeFun (Vector3 currpos,float dt,float pscale);

    private Transform[]   points;

    private MoveTypeFun[] deleFuns = 
    {
        SineMove_Fun,Sine2DMove_Fun
        ,Sine2DMove_2_Fun
        ,Multi_SineMove_Fun
        ,Multi_Sine2DMove_Fun
        ,Ripple_Move_Fun
    };
  
    #endregion

    [Range(1,400)]
    public int resolution = 1;

    void Awake()
    {
        Initailize();
    }

    private void Initailize()
    {
        points = new Transform[resolution * resolution];
       for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) 
       {
			if (x == resolution) {
				x = 0;
				z += 1;
			}
            Transform vInstanceSeal = GameObject.Instantiate(mPointPrefab);
            points[i] = vInstanceSeal;
            points[i].parent = this.gameObject.transform;
            points[i].localPosition = new Vector3((x + 0.5f) * stepOffset.x - 1.0f,0,(z + 0.5f) * stepOffset.y - 1.0f);
        }
    }

    private void UnInit()
    {
        for(int i=0; i< points.Length; i++)
        {
            Transform vInstanceSeal = points[i];
            GameObject.Destroy(vInstanceSeal);
            points[i] = null;
        }
    }

    // Update is called once per frame
    private static Vector3 SineMove_Fun(Vector3 currpos,float dt,float pscale)
    {

       currpos.y = Mathf.Sin(pi * (currpos.x + dt)) * pscale;
       return currpos;
    }

    private static Vector3 Sine2DMove_Fun(Vector3 currpos,float dt,float pscale) 
    {
		currpos.y = Mathf.Sin(pi * (currpos.x + currpos.z + dt));
        return currpos;
	}
    
    static Vector3 Sine2DMove_2_Fun(Vector3 currpos,float dt,float pscale) 
    {
//		return Mathf.Sin(pi * (x + z + t));
		currpos.y = Mathf.Sin(pi * (currpos.x + dt));
		currpos.y += Mathf.Sin(pi * (currpos.z + dt));
		currpos.y *= 0.5f;
		return currpos;
	}

	private static Vector3 Multi_SineMove_Fun (Vector3 currpos,float dt,float pscale) 
    {
		currpos.y = Mathf.Sin(pi * (currpos.x + dt));
		currpos.y += Mathf.Sin(2f * pi * (currpos.x + 2f * dt)) / 2f;
		currpos.y *= 2f / 3f;
		return currpos;
	}

    private static Vector3 Multi_Sine2DMove_Fun (Vector3 currpos,float dt,float pscale)
     {
		currpos.y = 4f * Mathf.Sin(pi * (currpos.x + currpos.z + dt * 0.5f));
		currpos.y += Mathf.Sin(pi * (currpos.x + dt));
		currpos.y += Mathf.Sin(2f * pi * (currpos.z + 2f * dt)) * 0.5f;
		currpos.y *= 1f / 5.5f;
		return currpos;
	}

    private static Vector3  Ripple_Move_Fun (Vector3 currpos,float dt,float pscale)
     {
        float d = Mathf.Sqrt(currpos.x * currpos.x + currpos.z * currpos.z);
		currpos.y = Mathf.Sin(pi * (4f * d - dt));
		return currpos;
	}


    void Start()
    {
        
    }
    void Update()
    {
        float t = Time.time;
		MoveTypeFun f = deleFuns[(int)mMatheMaticalType];
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position = f(position, t,scaleMove);
			point.localPosition = position;
		}
    }


    void OnDestroy()
    {
        UnInit();
    }

    
}
