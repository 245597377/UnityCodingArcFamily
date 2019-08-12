Shader "STFRender/Effect/Effect_UPD"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_height_param("_height_param", Float) = 0
		_alphaScale_param("_alphaScale_param", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Cull off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            struct v2f
            {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float  _height_param;
			float _YFactor;
			float _alphaScale_param;
            v2f vert (appdata_base  v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.texcoord;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float factor = ceil((i.worldPos.y - _YFactor) - _height_param);
				factor = 1 - clamp(factor, 0.0, 1.0);
				factor = factor * _alphaScale_param;
				if (factor < 0)
				{
					discard;
				}
				float4 color = tex2D(_MainTex, i.uv);
				color.a = factor;
				return color;
            }
            ENDCG
        }
    }
}
