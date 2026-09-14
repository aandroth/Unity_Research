Shader "ShaderLearning/Toon_Lit"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
    }
    SubShader
    {
        Tags 
        { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            HLSLPROGRAM

            // These assign the vert and frag fns as the functions to use for the vertex and fragment shader stages, respectively.
            // We must do this becasue the names vert and frag aren't keywords or anything. The names are arbitrary.
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                sampler2D _MainTex;
            CBUFFER_END

            struct appdata // also known as VertexInput or appdata or Attributes
            {
                float4 positionOS : POSITION;
                float2 uv_MainTex : TEXCOORD0;
            }; // MUST have a semicolon at the end

            struct v2f // also known as VertexOutput or Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionCS  : SV_POSITION;
            };

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            v2f vert(appdata IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                v2f OUT = (v2f)0;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Returning the output.
                return OUT;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Defining the color variable and returning it.
                return _BaseColor;
            }

            ENDHLSL
        }
    }
}
