Shader "ShaderLearning/Texture_Scroll"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
		_BaseTexture ("Base Texture", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Vector) = (0, 0, 0, 0)
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
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"
             // _BaseTexture_ST: This is the tiling and offset property for the _MainTex texture. The tiling values are stored in the x and y components, while the offset values are stored in the z and w components.
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseTexture_ST;
                float2 _ScrollSpeed;
            CBUFFER_END                
            
            TEXTURE2D(_BaseTexture); // This is the texture property for the _MainTex texture.
            SAMPLER(sampler_BaseTexture); // This is the sampler property for the _MainTex texture. We need this to sample the texture in the fragment shader.
           
            struct appdata // also known as VertexInput or appdata or Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            }; // MUST have a semicolon at the end

            struct v2f // also known as VertexOutput or Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionCS  : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            v2f vert(appdata v) 
            {
                // Declaring the output object (OUT) with the Varyings struct.
                v2f o = (v2f)0;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTexture); // If image is a sprite THIS WILL NOT WORK
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float2 uv = i.uv + _ScrollSpeed * _Time.y;
                // Defining the color variable and returning it.
                float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_LinearRepeat, uv);
                return textureColor;
            }

            ENDHLSL
        }
    }
}
