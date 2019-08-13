// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "STF/RE/Fundamentals/FirstShader_1"
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

			float4 FirstShaderVertex(float4 position : POSITION) :SV_POSITION
			{
				//return 0;
				//return position;


				//correctly project our onto the display {mode - view - projection]
				//hlslcc_mtx4x4unity_ObjectToWorld  hlslcc_mtx4x4unity_MatrixVP
				return UnityObjectToClipPos(position);
			}

			float4 FirstShaderFragment(float4 position:SV_POSITION) :SV_TARGET
			{
				//return 0;
				return _BaseColor;
			}
			ENDCG
		}
    }
}
