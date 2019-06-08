using UnityEngine;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(menuName = "DZAsset/DZRendering/DZ_Pipeline_Simple")]
public class DZCRPipeline_Simple_Asset : RenderPipelineAsset
{
    [SerializeField]
    bool dynamicBatching;

    [SerializeField]
    bool instancing;

    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new DZRenderPipeline_Simple(dynamicBatching, instancing);
    }
}