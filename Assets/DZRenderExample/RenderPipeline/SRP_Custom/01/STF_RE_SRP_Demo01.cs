using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class STF_RE_SimpleSRP : IRenderPipeline
{
    /// <summary>
    ///   <para>When the IRenderPipeline is invalid or destroyed this returns true.</para>
    /// </summary>
    public bool disposed
    {
        get { return true; }
    }

    /// <summary>
    ///   <para>Defines custom rendering for this RenderPipeline.</para>
    /// </summary>
    /// <param name="renderContext">Structure that holds the rendering commands for this loop.</param>
    /// <param name="cameras">Cameras to render.</param>
    public void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        
    }

    public void Dispose()
    {
        
    }
}
