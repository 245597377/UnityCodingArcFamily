using UnityEngine;


namespace STFEngine.Util
{
    /// <summary>
    /// 合并网格
    /// </summary>
    public class ChinarMergeMesh : MonoBehaviour
    {
        public Material combineMaterial;
        void Start()
        {
            MergeMesh();
        }


        /// <summary>
        /// 合并网格
        /// </summary>
        private void MergeMesh()
        {
            return;
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();   //获取 所有子物体的网格
            CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length]; //新建一个合并组，长度与 meshfilters一致
            for (int i = 0; i < meshFilters.Length; i++)                                  //遍历
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;                   //将共享mesh，赋值
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix; //本地坐标转矩阵，赋值
            }
            Mesh newMesh = new Mesh();                                  //声明一个新网格对象
            newMesh.CombineMeshes(combineInstances);                    //将combineInstances数组传入函数
            gameObject.AddComponent<MeshFilter>().sharedMesh = newMesh; //给当前空物体，添加网格组件；将合并后的网格，给到自身网格
                                                                        //到这里，新模型的网格就已经生成了。运行模式下，可以点击物体的 MeshFilter 进行查看网格

            #region 0x11

            gameObject.AddComponent<MeshRenderer>().material = Material.Instantiate(combineMaterial);
            foreach (Transform t in transform)                                                              //禁用掉所有子物体
            {
                t.gameObject.SetActive(false);
            }
            #endregion
        }
    }
}