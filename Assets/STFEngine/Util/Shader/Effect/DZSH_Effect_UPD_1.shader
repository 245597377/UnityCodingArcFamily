Shader "DZShader/Effect/Effect_UPD_1"
{
    Properties
    {
		_MainColor("Color", Color) = (0,0,0,1)
		_Exp("_Exp", Float) = 0
		_BM("_BM", Float) = 0
		_height_param("_height_param", Float) = 0
		_alphaScale_param("_alphaScale_param", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
		AlphaTest Greater 0.5
		Blend SrcAlpha OneMinusSrcAlpha
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
			float4 _MainColor;
			float  _height_param;
			float _YFactor;
			float _Exp;
			float _BM;
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
				
				float4 color = _MainColor;

				//float y = dot(float4(0.3, 0.59, 0.11, 1), c);
				float yd = _Exp * (_Exp / _BM + 1) / (_Exp + 1);
				color = color * yd;
				color.a = factor;
				return color;
            }
            ENDCG
        }
    }
}
