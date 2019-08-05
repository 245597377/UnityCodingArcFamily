// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "RenderExample/Feature/NPR/OutLine"
{
	Properties
    {
        _outlineColor("Outline Color", Color) = (0,0,0,1)
		_outlineThickness ("Outline Thickness", Range(0.0, 0.025)) = 0.01
		_outlineThickness(" ", Float) = 0.01
		_outlineShift ("Outline Light Shift", Range(0.0, 0.025)) = 0.01
		_outlineShift(" ", Float) = 0.01
    }

    SubShader
    {
        pass
        {
            ZWrite ON
            Tags{"Queue" = "Transparent" "RenderType"="Transparent"}
            Cull Front

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vertexMain
            #pragma fragment fragMain

            uniform fixed4 _outlineColor;
			uniform fixed _outlineThickness;
			uniform fixed _outlineShift;

            struct app2vert 
            {
				float4 vertex 	: 	POSITION;
				fixed4 normal 	:	NORMAL;	
			};
			
			struct vert2Pixel
			{
				float4 pos 		: 	SV_POSITION;
			};

            vert2Pixel vertexMain(app2vert In)
            {
                vert2Pixel OUT;
                float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
                float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;

                float4 deformedPosition = mul(World, In.vertex);
                fixed3 norm = normalize(mul(  In.normal.xyz , WorldInverseTranspose ).xyz);	

                half3 pixelToLightPosition =  _WorldSpaceLightPos0.xyz - (deformedPosition * _WorldSpaceLightPos0.w);
                fixed3 lightDirection = normalize(-pixelToLightPosition);
				
				deformedPosition.xyz += ( norm * _outlineThickness) + (lightDirection * _outlineShift);
				deformedPosition.xyz = mul(WorldInverseTranspose, float4 (deformedPosition.xyz, 1)).xyz * 1.0;

				OUT.pos = mul(WorldViewProjection, deformedPosition);
				
				return OUT;
            }

            fixed4 fragMain(vert2Pixel In):COLOR
            {
                fixed4 outColor;							
				outColor =  _outlineColor;
				return outColor;
            }

            ENDCG
        }
        
    }
}
