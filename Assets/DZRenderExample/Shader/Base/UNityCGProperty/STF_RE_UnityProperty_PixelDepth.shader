// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "STF/RE/Unity_CGProperty/PixelDepth"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		Pass
		{
				ZWrite On
	/*			ColorMask 0*/
				CGPROGRAM
				#pragma	vertex		vertMain
				#pragma	fragment	fragmentMain

				float4 _centerpos;
				#include "UnityCG.cginc"
				struct vertMainOut
				{
					float4 pos:	SV_POSITION;
					float  depth : DEPTH;
				};

				vertMainOut vertMain(appdata_full ver_in)
				{
					vertMainOut  ver_out;
					ver_out.pos = UnityObjectToClipPos(ver_in.vertex);
					ver_out.depth = -mul(UNITY_MATRIX_MV, ver_in.vertex).z * _ProjectionParams.w;
					return ver_out;
				}

				fixed4 fragmentMain(vertMainOut frag_in) : SV_Target
				{
					float depthInvert =  frag_in.depth;
					//depthInvert = depthInvert * (_ProjectionParams.z - _ProjectionParams.y) / 2 + (_ProjectionParams.z + _ProjectionParams.y) / 2;// incorrect result
					return fixed4(depthInvert, depthInvert, depthInvert, 1.0);
				}
			ENDCG
		}
       
    }
    FallBack "Diffuse"
}
