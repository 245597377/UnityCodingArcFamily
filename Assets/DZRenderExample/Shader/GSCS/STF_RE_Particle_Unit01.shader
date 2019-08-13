Shader "STF/RE/Particle/CS_Unit01"
{
     Properties
    {
        _ColorLow("Color Slow Speed", Color) = (0, 0, 0.5, 1)
        _ColorHigh("Color High Speed", Color) = (1, 0, 0, 1)
        _HighSpeedValue("High speed Value", Range(0, 50)) = 25
    }

    SubShader
    {
        Pass
        {
            Blend SrcAlpha one

            CGPROGRAM
            #pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Particle's data
            struct Particle
            {
                float3 position;
                float3 velocity;
            };

            // Pixel shader input
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
            };

            // Particle's data, shared with the compute shader
            StructuredBuffer<Particle> Particles;

            // Properties variables
            uniform float4 _ColorLow;
            uniform float4 _ColorHigh;
            uniform float _HighSpeedValue;

            // Vertex shader
            PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
            {
                PS_INPUT o = (PS_INPUT)0;

                // Color
                float speed = length(Particles[instance_id].velocity);
                float lerpValue = clamp(speed / _HighSpeedValue, 0.0f, 1.0f);
                o.color = lerp(_ColorLow, _ColorHigh, lerpValue);
                // Position
                o.position = UnityObjectToClipPos(float4(Particles[instance_id].position, 1.0f));

                return o;
            }

            // Pixel shader
            float4 frag(PS_INPUT i) : COLOR
            {
                return i.color;
            }

            ENDCG
        }
    }
}