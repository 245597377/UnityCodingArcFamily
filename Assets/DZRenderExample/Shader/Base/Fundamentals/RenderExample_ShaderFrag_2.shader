// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DZRenderExample/Fundamentals/FirstShader_1"
{
	Properties
	{
		_BaseColor("baseColor",Color) = (1, 1, 1, 1)
	}

		SubShader
	{
	   Pass
		{
			CGPROGRAM
			#pragma vertex FirstShaderVertex
			#pragma fragment FirstShaderFragment

			#include "UnityCG.cginc"

			float4 _BaseColor;

			float4 FirstShaderVertex(
				float4 position : POSITION,
				out float3 localPosition : TEXCOORD0
			) :SV_POSITION
			{
				localPosition = position.xyz;
				return UnityObjectToClipPos(position);
			}

			float4 FirstShaderFragment(
				float4 position:SV_POSITION,
				float3 localPosition : TEXCOORD0
			) :SV_TARGET
			{
				//return 0;
				return _BaseColor + float4(localPosition,0.0f);
			}
			ENDCG
		}
    }
}
