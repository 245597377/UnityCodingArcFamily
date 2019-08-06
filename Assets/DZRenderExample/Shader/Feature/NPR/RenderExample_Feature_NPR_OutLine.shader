// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
Shader "RenderExample/Feature/NPR/OutLine"
{
	Properties
    {
		_lightingRamp("Lighting", 2D) = "white" {}
        _outlineColor("Outline Color", Color) = (0,0,0,1)
		_outlineThickness ("Outline Thickness", Range(0.0, 0.025)) = 0.01
		_outlineThickness(" ", Float) = 0.01
		_outlineShift ("Outline Light Shift", Range(0.0, 0.025)) = 0.01
		_outlineShift(" ", Float) = 0.01
		_diffuseColor("Diffuse Color", Color) = (1,1,1,1)
		_diffuseMap("Diffuse", 2D) = "white" {}
		_specularIntensity ("Specular Intensity", Range(0.0, 1.0)) = 1.0
		_specularIntensity (" ", Float) = 1.0
		_specMapConstant ("Additive Specularity", Range(0.0, 0.5)) = .25
		_specMapConstant (" ", Float) = .25
		_specularColor("Specular Color", Color) = (1,1,1,1)
		_normalMap("Normal / Specular (A)", 2D) = "bump" {}
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

        pass
        {
            Tags{"LightMode"="ForwardBase"}
            Cull Back
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
			#pragma target 3.0

            #pragma vertex vertexMain
            #pragma fragment fragMain

			uniform fixed3 _diffuseColor;
			uniform sampler2D _lightingRamp;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;
			uniform fixed4 _LightColor0; 
			uniform fixed _specMapConstant;
			uniform half _specularIntensity;
			uniform fixed4 _specularColor;
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;

            struct app2vert
            {
                fixed4 vertex       :POSITION;
                fixed4 normal       :NORMAL;
                fixed4 tangent      :TANGENT;
                fixed2 texcoord0    :TEXCOORD0;
            };

            struct vert2Pixel
            {
                float4 Pos          :SV_POSITION;
                fixed2 uvs			:TEXCOORD0;
				fixed3 normalDir	:TEXCOORD1;	
				fixed3 binormalDir	:TEXCOORD2;	
				fixed3 tangentDir	:TEXCOORD3;	
				half3 posWorld		:TEXCOORD4;	
				fixed3 viewDir		:TEXCOORD5;
				fixed3 Lighting		:TEXCOORD6;
            };

			//兰伯特光照模式
			fixed lambert(fixed3 norDir, fixed3 lightDir)
			{
				return saturate( (dot(norDir,lightDir) + 1) /2 );
			}



            vert2Pixel vertexMain(app2vert In)
            {
                vert2Pixel Out;
                float4x4 worldviewProjectMat    = UNITY_MATRIX_MVP;
                float4x4 WorldInverseTranspose  = unity_WorldToObject;
                float4x4 World                  = unity_ObjectToWorld;

                Out.Pos = normalize(mul(worldviewProjectMat, In.vertex));
                Out.normalDir = normalize(mul(In.normal, WorldInverseTranspose));
                Out.tangentDir = normalize(mul(In.tangent, WorldInverseTranspose));
                Out.binormalDir = normalize(cross(Out.normalDir,Out.tangentDir));
				Out.posWorld = mul(In.vertex, World);
				Out.viewDir  = normalize(Out.posWorld - _WorldSpaceCameraPos);
				Out.Lighting = fixed3(0.0,0.0,0.0);
                return Out;
            }

			//DaoZhang_XDZ
			//Toon Light Frag with jianbian LightRAMP and NormalMap
            float4 fragMain(vert2Pixel IN):COLOR
            {
				half2 normalUVs = TRANSFORM_TEX(IN.uvs, _normalMap);
				fixed4 normal1D = tex2D(_normalMap, normalUVs);
				//unpack Normal
				normal1D.xyz = normal1D.xyz * 2 - 1;
				

				fixed3	normalDir	= normal1D.xyz;
				fixed	specMap		= normal1D.w;
				fixed3 ambientL = UNITY_LIGHTMODEL_AMBIENT.xyz;
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));


				//Main Dirction Light
				fixed3	pixelToLightSource	= _WorldSpaceLightPos0.xyz - (IN.posWorld - _WorldSpaceLightPos0.w);
				fixed	attenuation			= lerp(1.0, 1.0/length(pixelToLightSource), _WorldSpaceLightPos0.w);
				fixed3  lightDirection		= normalize(pixelToLightSource);
				float3	diffuseL			= lambert(normalDir, lightDirection);

				//Diffuse Lighting
				// Base On LightRamp -- uv's u with length
				fixed	Lighting			= saturate(saturate(IN.Lighting - diffuseL) + diffuseL + ambientL);
				fixed   LightUV				= clamp(Lighting, 0.01, 0.99);
				fixed3	diffuse				= tex2D(_lightingRamp, fixed2(LightUV, 0.0));

				//Spec Light

                return float4(1.0,0.25,0.25,1.0);
            }

            ENDCG
        }
        
    }
}
