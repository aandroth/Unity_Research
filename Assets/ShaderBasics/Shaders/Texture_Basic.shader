Shader "ShaderLearning/Texture_Basic"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
		_BaseTexture ("Base Texture", 2D) = "white" {}
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
                "LightMode" = "SRPDefaultUnlit"
            }

            ZWrite On
            ZTest LEqual

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
                float4 outputColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv);
                return outputColor * _BaseColor;
            }

            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask R
            
            HLSLPROGRAM

            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata // also known as VertexInput or appdata or Attributes
            {
                float4 positionOS : POSITION;
            }; // MUST have a semicolon at the end

            struct v2f // also known as VertexOutput or Varyings
            {
                float4 positionCS  : SV_POSITION;
            };

            v2f depthOnlyVert(appdata v) 
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            float depthOnlyFrag(v2f i) : SV_TARGET
            {
                return i.positionCS.z;
            }

            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }
                        
            ZWrite On
            
            HLSLPROGRAM

            #pragma vertex depthNormalsVert
            #pragma fragment depthNormalsFrag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata // also known as VertexInput or appdata or Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            }; // MUST have a semicolon at the end

            struct v2f // also known as VertexOutput or Varyings
            {
                float4 positionCS  : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            v2f depthNormalsVert(appdata v) 
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.normalWS = NormalizeNormalPerVertex(normalWS);
                return o;
            }

            float4 depthNormalsFrag(v2f i) : SV_TARGET
            {
                float3 normalWS = NormalizeNormalPerPixel(i.normalWS);
                return float4(normalWS, 0.0f);
            }

            ENDHLSL
        }
    }
}
