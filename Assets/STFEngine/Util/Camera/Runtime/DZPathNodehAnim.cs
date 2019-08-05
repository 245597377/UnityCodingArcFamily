using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PathNode
{
    public Transform nodeTarget;
    public float NodeEnterTime;
    public float NodeEnterDistance;
    public float NodeExitTime;
    public float NodeExitDistance;
}

public class DZPathNodehAnim : MonoBehaviour
{
    [SerializeField]
    public PathNode[] mNode;
    public bool isAutoPlay = true;

    private void Awake() 
    {
        
    }

    private void Start() 
    {
        if(isAutoPlay)
        {
            playMovePathFollowing();
        }
    }

    public void Play()
    {
        playMovePathFollowing();
    }
   
    private void playMovePathFollowing()
    {

    }
}
