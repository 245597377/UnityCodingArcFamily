using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CarPath : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public float worldDisScale = 1.0f;
    [HideInInspector]
    public List<GameObject> Nodes = new List<GameObject>();
    //public List<float> Speeds = new List<float>();

    public List<NodeListClass> NodesProperties = new List<NodeListClass>();

    [HideInInspector]
    public float RotationSpeed = 50f;
    [HideInInspector]
    private int NumberOfTurnPoints = 100;
    [HideInInspector]
    public float NodeThreshold = 0.25f;

    public bool Repeat = true;
    public bool ReverseDirections = false;
    public bool SingleSpeedForAllNodes = false;
    [Range(0.01f, 120f)]
    public float SingleSpeed;
    //public bool UpdateAfterEveryLoop = false;

    public GameObject NodeParent;

    List<Vector3> points = new List<Vector3>();

    public IEnumerator ienum;

    Quaternion SampledRotation;
    float OriginalSpeed;

    void OnDrawGizmos()
    {

        if (Repeat)
        {
            if (Nodes.Count >= 2)
            {
                Gizmos.DrawLine(Nodes.Last().transform.position, Nodes.First().transform.position);
            }
        }
        else
        {

            Nodes.First().GetComponent<NodeScript>().BefNode = null;
            Nodes.First().GetComponent<NodeScript>().CurvePoint = false;

            Nodes.Last().GetComponent<NodeScript>().AftNode = null;
            Nodes.Last().GetComponent<NodeScript>().CurvePoint = false;

        }
    }

    public void AddNode()
    {

        if (NodeParent != null)
        {

            GameObject new_node = new GameObject("Node");

            new_node.AddComponent<NodeScript>();

            if (Nodes.Count == 0)
            {
                new_node.transform.position = transform.position;
            }
            else
            {
                new_node.GetComponent<NodeScript>().BefNode = Nodes.Last();
                new_node.transform.position = Nodes.Last().transform.position;
                Nodes.ToArray()[Nodes.Count - 1].GetComponent<NodeScript>().AftNode = new_node;
                Nodes.First().GetComponent<NodeScript>().BefNode = new_node;
            }

            new_node.transform.parent = NodeParent.transform;
            new_node.GetComponent<NodeScript>().Car = gameObject;

#if UNITY_EDITOR

            Selection.activeGameObject = new_node;

#endif

            Nodes.Add(new_node);

        }
        else
        {

            Debug.Log("No Node Parent set, creating one automatically");
            NodeParent = new GameObject("NodeParent");
            NodeParent.transform.position = Vector3.zero;

            GameObject new_node = new GameObject("Node");

            new_node.AddComponent<NodeScript>();

            if (Nodes.Count == 0)
            {
                new_node.transform.position = transform.position;
            }
            else
            {
                new_node.GetComponent<NodeScript>().BefNode = Nodes.Last();
                new_node.transform.position = Nodes.Last().transform.position;
                Nodes.ToArray()[Nodes.Count - 1].GetComponent<NodeScript>().AftNode = new_node;
                Nodes.First().GetComponent<NodeScript>().BefNode = new_node;
            }

            new_node.transform.parent = NodeParent.transform;
            new_node.GetComponent<NodeScript>().Car = gameObject;

#if UNITY_EDITOR

            Selection.activeGameObject = new_node;

#endif

            Nodes.Add(new_node);

        }


    }

    public void RemoveNode()
    {

        if (Nodes.Count != 0)
        {
            DestroyImmediate(Nodes.Last());
            Nodes.Remove(Nodes.Last());

            //Nodes.Last().GetComponent<NodeScript>().AftNode = null;
            //Nodes.First().GetComponent<NodeScript>().AftNode = null;

            if (Nodes.Last().GetComponent<NodeScript>().CurvePoint || Nodes.First().GetComponent<NodeScript>().CurvePoint)
            {
                Nodes.Last().GetComponent<NodeScript>().CurvePoint = false;
                Nodes.First().GetComponent<NodeScript>().CurvePoint = false;
            }

        }
        else
        {

            Debug.Log("No nodes to remove");

        }

    }

    public void RecalculatePath()
    {

        //if(Application.isPlaying) {
        refreshNode();
        NodesProperties.Clear();

        for (int i = 0; i < Nodes.Count; i++)
        {
            //Debug.Log("yup");
            NodesProperties.Add(Nodes.ToArray()[i].GetComponent<NodeScript>().ThisNodeClass);
            //Debug.Log(Nodes.ToArray()[i].GetComponent<NodeScript>().ThisNodeClass.Speed);
            //Debug.Log(NodesTest.ToArray()[i].Speed);
        }

        float sum = 0f;
        foreach (GameObject node in Nodes)
        {
            sum += node.GetComponent<NodeScript>().Speed;
        }
        sum = sum / Nodes.Count;

        //sum_p = sum;

        if (sum < 12f)
        {

            NodeThreshold = 0.35f;
            RotationSpeed = 30f;

        }
        else if (sum >= 20f && sum < 50f)
        {

            NodeThreshold = 0.95f;
            RotationSpeed = 100f;

        }
        else if (sum >= 50f && sum < 100f)
        {

            NodeThreshold = 1.55f;
            RotationSpeed = 120f;

        }
        else if (sum >= 100f && sum < 200f)
        {

            NodeThreshold = 1.95f;
            RotationSpeed = 150f;

        }
        else if (sum >= 200 && sum < 300)
        {

            NodeThreshold = 2.75f;
            RotationSpeed = 250f;

        }
        else if (sum >= 300)
        {

            NodeThreshold = 2.5f;
            RotationSpeed = 350f;

        }

        StopAllCoroutines();

        points.Clear();

        int n = 0;

        while (n < Nodes.ToArray().Length)
        {

            if (Nodes.ToArray()[n].GetComponent<NodeScript>().CurvePoint == true)
            {

                float t = 0f;
                Vector3 position = Vector3.zero;
                for (int i = 0; i < NumberOfTurnPoints; i++)
                {
                    t = i / (NumberOfTurnPoints - 1.0f);
                    position = (1.0f - t) * (1.0f - t) * Nodes.ToArray()[n].GetComponent<NodeScript>().CurveStartPoint + 2.0f * (1.0f - t) * t * Nodes.ToArray()[n].transform.position + t * t * Nodes.ToArray()[n].GetComponent<NodeScript>().CurveEndPoint;

                    points.Add(position);
                }

                n++;

            }
            else
            {

                points.Add(Nodes.ToArray()[n].transform.position);
                n++;

            }

        }

        if (ReverseDirections)
        {
            points.Reverse();
        }

        if (Nodes.Count >= 2)
        {
            
            StartCoroutine(Following());

        }
        else
        {

            Debug.Log("Too few nodes to follow");

        }
    }

    void OnValidate()
    {

        if (SingleSpeedForAllNodes)
        {
            foreach (NodeListClass node_lc in NodesProperties)
            {
                node_lc.Node.GetComponent<NodeScript>().Speed = SingleSpeed;
            }
        }
        else
        {
            foreach (NodeListClass node_lc in NodesProperties)
            {
                node_lc.Node.GetComponent<NodeScript>().Speed = node_lc.Speed;
            }
        }
    }

    protected virtual void EditorUpdate()
    {

        if (ienum == null)
        {
            ienum = Following();
        }
        if (!Application.isPlaying)
        {
            if (ienum.MoveNext())
            {
                for (int i = 0; i < 2500; i++)
                {
                    ienum.MoveNext();
                }
            }
        }

    }

    public void SampleRotation()
    {
        SampledRotation = transform.rotation;
    }

    public void MTSP()
    {

        transform.position = Nodes.First().transform.position;
        transform.rotation = SampledRotation;

    }

#if UNITY_EDITOR

    public void Play()
    {
        //MonoBehaviour.res
        if (!Application.isPlaying)
        {
            //StopAllCoroutines();
            EditorApplication.update -= EditorUpdate;
            //RecalculatePath();
            EditorApplication.update += EditorUpdate;
            //goto restart;
        }
        else
        {
            Debug.Log("Play is only used in edit mode");
        }
    }

    public void Stop()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.update -= EditorUpdate;
            //transform.position = Nodes.First().transform.position;
            //StopCoroutine(ienum);
            StopAllCoroutines();
        }
        else
        {
            Debug.Log("Stop is only used in edit mode");
        }
    }

#endif


    public void refreshNode()
    {
        GameObject[] vNodeArr = Nodes.ToArray();
        for (int i = 0; i < Nodes.Count; i++)
        {
            NodeScript vTempItem = vNodeArr[i].GetComponent<NodeScript>();
            if (vTempItem != null)
            {
                vTempItem.Refresh();
            }
        }
    }

    void Start()
    {

        if (!Application.isPlaying)
        {
            ienum = Following();
        }
        else
        {
            refreshNode();
        }
        //NodesTest = new List<NodeListClass>(new NodeListClass[Nodes.Count]);

        if (NodesProperties.Count == 0)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                //Debug.Log("yup");
                NodesProperties.Add(Nodes.ToArray()[i].GetComponent<NodeScript>().ThisNodeClass);
                //Debug.Log(Nodes.ToArray()[i].GetComponent<NodeScript>().ThisNodeClass.Speed);
                //Debug.Log(NodesTest.ToArray()[i].Speed);
            }
        }

        //EditorApplication.update -= TestUpdate;

        //Play();
        float sum = 0f;
        foreach (GameObject node in Nodes)
        {
            sum += node.GetComponent<NodeScript>().Speed;
        }
        sum = sum / Nodes.Count;

        //sum_p = sum;

        if (sum < 12f)
        {

            NodeThreshold = 0.35f;
            RotationSpeed = 30f;

        }
        else if (sum >= 20f && sum < 50f)
        {

            NodeThreshold = 0.95f;
            RotationSpeed = 100f;

        }
        else if (sum >= 50f && sum < 100f)
        {

            NodeThreshold = 1.35f;
            RotationSpeed = 120f;

        }
        else if (sum >= 100f && sum < 200f)
        {

            NodeThreshold = 1.55f;
            RotationSpeed = 150f;

        }
        else if (sum >= 200 && sum < 300)
        {

            NodeThreshold = 2.75f;
            RotationSpeed = 250f;

        }
        else if (sum >= 300)
        {

            NodeThreshold = 2.5f;
            RotationSpeed = 350f;

        }


        //NodeThreshold = Mathf.Clamp(sum/20f,0.2f, 1.25f);
        //RotationSpeed = Mathf.Clamp(sum/1.5f, 20f, 200f);

        int n = 0;
        points.Clear();
        while (n < Nodes.ToArray().Length)
        {

            if (Nodes.ToArray()[n].GetComponent<NodeScript>().CurvePoint == true)
            {

                float t = 0f;
                Vector3 position = Vector3.zero;
                for (int i = 0; i < NumberOfTurnPoints; i++)
                {
                    t = i / (NumberOfTurnPoints - 1.0f);
                    position = (1.0f - t) * (1.0f - t) * Nodes.ToArray()[n].GetComponent<NodeScript>().CurveStartPoint + 2.0f * (1.0f - t) * t * Nodes.ToArray()[n].transform.position + t * t * Nodes.ToArray()[n].GetComponent<NodeScript>().CurveEndPoint;

                    points.Add(position);
                }

                n++;

            }
            else
            {

                points.Add(Nodes.ToArray()[n].transform.position);
                n++;

            }

        }

        if (ReverseDirections)
        {
            points.Reverse();
        }
        if (Nodes.Count >= 2)
        {
            StartCoroutine(Following());
        }
    }

    public void ReevaluatePath()
    {
        Nodes.Clear();// = Car.GetComponent<CarPath>().NodeParent.GetComponentsInChildren<GameObject>().ToList();

        foreach (Transform child in NodeParent.transform)
        {
            Nodes.Add(child.gameObject);
        }

        RecalculatePath();
    }



    public IEnumerator Following()
    {
        int node_number = 0;
        int point_number = 0;
        int point_count = points.Count;

        GameObject[] vNodeGameObject = Nodes.ToArray();
        List<NodeScript> vNodeScriptList = new List<NodeScript>();
        for(int i=0; i< vNodeGameObject.Length; i++)
        {
            if(vNodeGameObject[i] != null)
            {
                NodeScript vNodeScriptTemp = vNodeGameObject[i].GetComponent<NodeScript>();
                if(vNodeScriptTemp != null)
                {
                    vNodeScriptList.Add(vNodeScriptTemp);
                }
            }
        }
        NodeScript[] vNodeScriptArr = vNodeScriptList.ToArray();

        while (point_number < point_count)
        {
            Vector3 point = points[point_number];
            bool stopping = false;
            bool accelerating = false;

           
            if (vNodeScriptArr[node_number].StopPoint && vNodeScriptArr[node_number].SmoothStopping)
            {
                stopping = true;
                OriginalSpeed = vNodeScriptArr[node_number].Speed;

            }
            else if (node_number != 0)
            {

                if (vNodeScriptArr[node_number - 1].StopPoint && vNodeScriptArr[node_number - 1].SmoothStopping)
                {

                    accelerating = true;

                    OriginalSpeed = vNodeScriptArr[node_number].Speed;

                    vNodeScriptArr[node_number].Speed = 2f;

                }

            }



            while (Vector3.Distance(transform.position, point) > NodeThreshold)
            {

                if (node_number < Nodes.Count)
                {
                    transform.position = Vector3.MoveTowards(transform.position, point, Time.deltaTime * vNodeScriptArr[node_number].Speed * worldDisScale);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, point, Time.deltaTime * SingleSpeed * worldDisScale);
                }


                if (points[point_number] - transform.position != Vector3.zero)
                {
                    Vector3 vtarDir = points[point_number] - transform.position;
                    if (vtarDir.sqrMagnitude > 0)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(points[point_number] - transform.position, Vector3.up), Time.deltaTime * RotationSpeed * 1.0f);
                    }
                }
                else if (point_number + 1 < points.Count)
                {
                    Vector3 vtarDir = points[point_number + 1] - transform.position;

                    if (vtarDir.sqrMagnitude > 0)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(points[point_number + 1] - transform.position, Vector3.up), Time.deltaTime * RotationSpeed * 1.0f);
                    }
                }


                if (stopping)
                {


                    if (vNodeScriptArr[node_number].Speed <= 0.05f)
                    {
                        if (SingleSpeedForAllNodes)
                        {
                            vNodeScriptArr[node_number].Speed = SingleSpeed;
                        }
                        else
                        {
                            vNodeScriptArr[node_number].Speed = OriginalSpeed;
                        }
                        break;
                    }
                    else
                    {

                        float param = Mathf.InverseLerp(0f, 
                            Vector3.Distance(vNodeScriptArr[node_number].transform.position, vNodeScriptArr[node_number - 1].transform.position), 
                            Vector3.Distance(transform.position, vNodeScriptArr[node_number - 1].transform.position)
                            );

                        vNodeScriptArr[node_number].Speed = Mathf.Lerp(OriginalSpeed, 0f, param);

                    }

                }
                else if (accelerating)
                {

                    if (vNodeScriptArr[node_number].Speed >= OriginalSpeed)
                    {
                        if (SingleSpeedForAllNodes)
                        {
                            vNodeScriptArr[node_number].Speed = SingleSpeed;
                        }
                        else
                        {
                            vNodeScriptArr[node_number].Speed = OriginalSpeed;
                        }
                        break;
                    }
                    else
                    {
                        float param = Mathf.InverseLerp(
                            0f, 
                            Vector3.Distance(vNodeScriptArr[node_number].transform.position, vNodeScriptArr[node_number - 1].transform.position), 
                            Vector3.Distance(transform.position, vNodeScriptArr[node_number - 1].transform.position));

                        vNodeScriptArr[node_number].Speed = Mathf.Lerp(2f, OriginalSpeed, param);

                    }

                }

                yield return new WaitForEndOfFrame();

            }

            if (vNodeScriptArr[node_number].Speed != OriginalSpeed && stopping)
            {
                vNodeScriptArr[node_number].Speed = OriginalSpeed;
            }


            point_number++;

            for(int i=0; i< Nodes.Count; i++)
            {
                GameObject node = Nodes[i];
                if (point == node.transform.position || point == node.GetComponent<NodeScript>().CurveEndPoint)
                {

                    if (node_number + 1 < Nodes.Count)
                    {
                        node_number++;
                    }

                    if (node.GetComponent<NodeScript>().StopPoint)
                    {
                        yield return new WaitForSeconds(node.GetComponent<NodeScript>().StopTime);
                    }

                    break;

                }

            }

            if(point_number >= point_count && Repeat)
            {
                point_number = 0;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
