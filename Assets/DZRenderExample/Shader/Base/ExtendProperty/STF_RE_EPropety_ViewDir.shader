// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "STF/RE/EProperty/ViewDir"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _RimColor ("RimColor", Color) = (1,1,1,1)
       	_RimPower ("Power", Range(0.6,9.0)) = 1.0
    }
    SubShader
    {
        pass
        {
            Tags { "RenderType"="Opaque" }
            LOD 200

            CGPROGRAM
                #include "UnityCG.cginc"
                    // Physically based Standard lighting model, and enable shadows on all light types
                #pragma vertex vert
                #pragma fragment frag

                float4 _Color;
                float4 _RimColor;
                float _RimPower;
               
                struct vertexInput
                {
                    float4 position:            POSITION;
                    float3 normal :             NORMAL;
                    float2 UVCoordsChannel0:    TEXCOORD0;
                };

                struct VertexToFragment
                {
                    float4 view_pos :           SV_POSITION;  //像素位置
                    float3 world_normal :       NORMAL;   //法线向量坐标
                    float2 UVCoordsChannel0:    TEXCOORD0;
                    float4 world_pos :          TEXCOORD1;   //在世界空间中的坐标位置
                };

                VertexToFragment vert(vertexInput input)
                {
                    VertexToFragment output;
                    output.view_pos = UnityObjectToClipPos(input.position);
                    output.world_pos = mul(unity_ObjectToWorld, input.position);
                    output.UVCoordsChannel0 = input.UVCoordsChannel0;
                    output.world_normal = mul(float4(input.normal, 0.0), unity_WorldToObject).xyz;
                    return output;
                }

                fixed4 frag(VertexToFragment input): COLOR
                {
                    float3 viewDir = _WorldSpaceCameraPos.xyz - input.world_pos;
                    float  rimfren = 1.0 - saturate(dot(normalize(viewDir), input.world_normal));
                    return float4(_Color.rgb + _RimColor.rgb * pow(rimfren, _RimPower), _Color.a);
                }    
             ENDCG        
        }
        
    }
    FallBack "Diffuse"
}
