Shader "STF/RE/Feature/SkyBox/Sky_1"
{
    Properties
    {
        _SkyCube ("SkyBox Tex", Cube) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float4 _cornel[4];
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.worldPos = _cornel[v.uv.x + v.uv.y * 2].xyz;
                return o;
            }
            
            TextureCube _SkyCube;
            SamplerState sampler_SkyCube;
            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                fixed4 col = _SkyCube.Sample(sampler_SkyCube,viewDir);
                //return fixed4(1.0,0.0,0.0,1.0);
                return col;
            }
            ENDCG
        }
    }
}
