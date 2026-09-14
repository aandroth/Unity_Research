Shader "ShaderLearning/Texture_Z_Depth"
{
    Properties
    {
        _ForegroundColor("Foreground Color", Color) = (0,0,0,0)
        _BackgroundColor("Background Color", Color) = (1,1,1,1)
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
            Tags 
            { 
                "RenderPipeline" = "SRPDefaultUnlit"
            }

            ZWrite On
            ZTest Always

            HLSLPROGRAM

            // These assign the vert and frag fns as the functions to use for the vertex and fragment shader stages, respectively.
            // We must do this becasue the names vert and frag aren't keywords or anything. The names are arbitrary.
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
             // _BaseTexture_ST: This is the tiling and offset property for the _MainTex texture. The tiling values are stored in the x and y components, while the offset values are stored in the z and w components.
            CBUFFER_START(UnityPerMaterial)
                float4 _ForegroundColor;
                float4 _BackgroundColor;
            CBUFFER_END                
            
            TEXTURE2D(_BaseTexture); // This is the texture property for the _MainTex texture.
            SAMPLER(sampler_BaseTexture); // This is the sampler property for the _MainTex texture. We need this to sample the texture in the fragment shader.
           
            struct appdata // also known as VertexInput or appdata or Attributes
            {
                float4 positionOS : POSITION;
            }; // MUST have a semicolon at the end

            struct v2f // also known as VertexOutput or Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionCS  : SV_POSITION;
                float4 positionSS  : TEXCOORD0;
            };

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            v2f vert(appdata v) 
            {
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionSS = ComputeScreenPos(o.positionCS);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float2 screenUV = i.positionSS.xy / i.positionSS.w;
                float rawDepth = SampleSceneDepth(screenUV);

                return lerp(_ForegroundColor, _BackgroundColor, rawDepth);
            }

            ENDHLSL
        }
    }
}
