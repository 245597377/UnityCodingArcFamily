﻿#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
Shader "STF/RE/Feature/NPR/OutLineNL"
{
	Properties
    {
		_MainTex ("Main Tex", 2D)  = "white" {}

		_outlineColor("Outline Color", Color) = (0,0,0,1)
		_outlineThickness ("Outline Thickness", Range(0.0, 0.025)) = 0.01
		_outlineThickness(" ", Float) = 0.01
		_outlineShift ("Outline Light Shift", Range(0.0, 0.025)) = 0.01
		_outlineShift(" ", Float) = 0.01

		_DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
		_Shininess ("Shininess", Range(1, 500)) = 40
		_DiffuseSegment ("Diffuse Segment", Vector) = (0.1, 0.3, 0.6, 1.0)
		_SpecularSegment ("Specular Segment", Range(0, 1)) = 0.5
    }

    SubShader
    {
		Tags { "RenderType"="Opaque" }
		LOD 200
        pass
        {
			NAME "OUTLINE"
            Cull Front
            CGPROGRAM
			#pragma vertex VertMain
			#pragma fragment FragMain
			#include "UnityCG.cginc"

			float _outlineThickness;
			float _outlineShift;
			float4 _OutlineColor;
			struct a2f
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;

			};

			struct v2f
			{
				float4 pos: SV_POSITION;
			};

			v2f VertMain(a2f IN)
			{
				v2f OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;
				
				float4 deformedPosition = mul(World, IN.vertex);
				fixed3 norm = normalize(mul(  IN.normal.xyz , WorldInverseTranspose ).xyz);	
				
				half3 pixelToLightSource =_WorldSpaceLightPos0.xyz - (deformedPosition.xyz *_WorldSpaceLightPos0.w);
				fixed3 lightDirection = normalize(-pixelToLightSource);
				fixed diffuse = saturate(ceil(dot(IN.normal, lightDirection)));				
				
				deformedPosition.xyz += ( norm * _outlineThickness) + (lightDirection * _outlineShift);
			
				deformedPosition.xyz = mul(WorldInverseTranspose, float4 (deformedPosition.xyz, 1)).xyz * 1.0;

				OUT.pos = mul(WorldViewProjection, deformedPosition);

				return OUT;
			}

			fixed4 FragMain(v2f IN):COLOR
			{
				return float4(_OutlineColor.rgb, 1); 
			}

            ENDCG
        }

		pass
		{
			Tags { "LightMode"="ForwardBase" }
		
			CGPROGRAM
		
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fwdbase
		
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
		
			fixed4 _DiffuseColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _SpecularColor;
			float _Shininess;
			fixed4 _DiffuseSegment;
			fixed _SpecularSegment;
		
			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			}; 
		
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				SHADOW_COORDS(3)
			};
			
			v2f vert (a2v v) 
			{
				v2f o;
				
				o.pos = UnityObjectToClipPos( v.vertex); 
				o.worldNormal  = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				TRANSFER_SHADOW(o);
		    	
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{ 
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				fixed3 worldViewDir = UnityWorldSpaceViewDir(i.worldPos);
				fixed3 worldHalfDir = normalize(worldViewDir + worldLightDir);
				
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
		    	
				fixed diff = dot(worldNormal, worldLightDir);
				diff = diff * 0.5 + 0.5;
				fixed spec = max(0, dot(worldNormal, worldHalfDir));
				spec = pow(spec, _Shininess);
				
				fixed w = fwidth(diff) * 2.0;
				if (diff < _DiffuseSegment.x + w) {
					diff = lerp(_DiffuseSegment.x, _DiffuseSegment.y, smoothstep(_DiffuseSegment.x - w, _DiffuseSegment.x + w, diff));
//					diff = lerp(_DiffuseSegment.x, _DiffuseSegment.y, clamp(0.5 * (diff - _DiffuseSegment.x) / w, 0, 1));
				} else if (diff < _DiffuseSegment.y + w) {
					diff = lerp(_DiffuseSegment.y, _DiffuseSegment.z, smoothstep(_DiffuseSegment.y - w, _DiffuseSegment.y + w, diff));
//					diff = lerp(_DiffuseSegment.y, _DiffuseSegment.z, clamp(0.5 * (diff - _DiffuseSegment.y) / w, 0, 1));
				} else if (diff < _DiffuseSegment.z + w) {
					diff = lerp(_DiffuseSegment.z, _DiffuseSegment.w, smoothstep(_DiffuseSegment.z - w, _DiffuseSegment.z + w, diff));
//					diff = lerp(_DiffuseSegment.z, _DiffuseSegment.w, clamp(0.5 * (diff - _DiffuseSegment.z) / w, 0, 1));
				} else {
					diff = _DiffuseSegment.w;
				}
				
				w = fwidth(spec);
				if (spec < _SpecularSegment + w) {
					spec = lerp(0, 1, smoothstep(_SpecularSegment - w, _SpecularSegment + w, spec));
				} else {
					spec = 1;
				}
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT;
				
				
				fixed3 texColor = tex2D(_MainTex, i.uv).rgb;
				fixed3 diffuse = diff * _LightColor0.rgb * _DiffuseColor.rgb * texColor;
				fixed3 specular = spec * _LightColor0.rgb * _SpecularColor.rgb;
				
				return fixed4(ambient + (diffuse + specular) * atten, 1);
			}
		
			ENDCG
		}
		
		Pass 
		{
			Tags { "LightMode"="ForwardAdd" }
			
			Blend One One
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fwdadd
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			
			
			fixed4 _DiffuseColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _SpecularColor;
			float _Shininess;
			fixed4 _DiffuseSegment;
			fixed _SpecularSegment;
			
			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			}; 
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				SHADOW_COORDS(3)
			};
			
			v2f vert (a2v v) 
			{
				v2f o;
			
				o.pos = UnityObjectToClipPos( v.vertex); 
				o.worldNormal  = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				TRANSFER_SHADOW(o);
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target { 
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				fixed3 worldViewDir = UnityWorldSpaceViewDir(i.worldPos);
				fixed3 worldHalfDir = normalize(worldViewDir + worldLightDir);
				
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				
				fixed diff = dot(worldNormal, worldLightDir);
				diff = (diff * 0.5 + 0.5) * atten;
				fixed spec = max(0, dot(worldNormal, worldHalfDir));
				spec = pow(spec, _Shininess);
				
				fixed w = fwidth(diff) * 2.0;
				if (diff < _DiffuseSegment.x + w) {
					diff = lerp(_DiffuseSegment.x, _DiffuseSegment.y, smoothstep(_DiffuseSegment.x - w, _DiffuseSegment.x + w, diff));
//					diff = lerp(_DiffuseSegment.x, _DiffuseSegment.y, clamp(0.5 * (diff - _DiffuseSegment.x) / w, 0, 1));
				} else if (diff < _DiffuseSegment.y + w) {
					diff = lerp(_DiffuseSegment.y, _DiffuseSegment.z, smoothstep(_DiffuseSegment.y - w, _DiffuseSegment.y + w, diff));
//					diff = lerp(_DiffuseSegment.y, _DiffuseSegment.z, clamp(0.5 * (diff - _DiffuseSegment.y) / w, 0, 1));
				} else if (diff < _DiffuseSegment.z + w) {
					diff = lerp(_DiffuseSegment.z, _DiffuseSegment.w, smoothstep(_DiffuseSegment.z - w, _DiffuseSegment.z + w, diff));
//					diff = lerp(_DiffuseSegment.z, _DiffuseSegment.w, clamp(0.5 * (diff - _DiffuseSegment.z) / w, 0, 1));
				} else {
					diff = _DiffuseSegment.w;
				}
				
				w = fwidth(spec);
				if (spec < _SpecularSegment + w) {
					spec = lerp(0, _SpecularSegment, smoothstep(_SpecularSegment - w, _SpecularSegment + w, spec));
				} else {
					spec = _SpecularSegment;
				}
				
				fixed3 texColor = tex2D(_MainTex, i.uv).rgb;
				fixed3 diffuse = diff * _LightColor0.rgb * _DiffuseColor.rgb * texColor;
				fixed3 specular = spec * _LightColor0.rgb * _SpecularColor.rgb;
				
				return fixed4((diffuse + specular) * atten, 1);
			}
			
			ENDCG
		}
    }
}