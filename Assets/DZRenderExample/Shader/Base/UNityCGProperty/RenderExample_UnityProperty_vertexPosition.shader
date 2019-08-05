// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DZRenderExample/Unity_CGProperty/vertexPosition"
{

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		Pass
		{
			CGPROGRAM
				#pragma	vertex		vertMain
				#pragma	fragment	fragmentMain

				float4 _centerpos;
				#include "UnityCG.cginc"
				struct vertMainOut
				{
					float4 pos:	SV_POSITION;
					fixed4 color : COLOR0;
				};

				vertMainOut vertMain(appdata_full ver_in)
				{
					vertMainOut  ver_out;
					ver_out.pos = UnityObjectToClipPos(ver_in.vertex);
					//float4 _worldPos = mul(unity_ObjectToWorld, ver_in.vertex);
					ver_out.color = fixed4(ver_in.vertex.xyz * 0.5f + fixed3(0.5f, 0.5f, 0.5f), 1.0f);
					return ver_out;
				}

				fixed4 fragmentMain(vertMainOut frag_in) : SV_Target
				{
					return frag_in.color;
				}
			ENDCG
		}
       
    }
    FallBack "Diffuse"
}
