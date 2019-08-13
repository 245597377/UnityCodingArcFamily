// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "STF/RE/Unity_CGProperty/Tangent"
{
   SubShader
   {
       pass
       {
            CGPROGRAM
            #include "UnityCG.cginc"
            
            #pragma vertex vert_main
            #pragma fragment frag_main

            struct vert_output
            {
                float4 position: SV_POSITION;
                fixed4 color : COLOR0;
            };

            vert_output vert_main(appdata_full pInput)
            {
                vert_output output;
                output.position = UnityObjectToClipPos(pInput.vertex);
                //Visual Tangent
                output.color =  fixed4(pInput.tangent.xyz * 0.5 + fixed3(0.5, 0.5, 0.5), 1.0);
                return output;
            }

            fixed4 frag_main(vert_output pInput):COLOR
            {
                return pInput.color;
            }

            ENDCG
       }
   }
}
