Shader "Custom/TestShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags
        {
            // RenderPipeline: <None>
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        Pass
        {
            // Name: <None>
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        // RenderState: <None>

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_PREVIEW
    #define SHADERGRAPH_PREVIEW
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
    #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
    #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float4 uv0 : TEXCOORD0;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float4 texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
        float4 uv0;
        float3 TimeParameters;
    };
    struct VertexDescriptionInputs
    {
    };
    struct PackedVaryings
    {
        float4 positionCS : SV_POSITION;
        float4 interp0 : TEXCOORD0;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
CBUFFER_END

// Object and Global properties

    // Graph Functions

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
        //rotation matrix
        UV -= Center;
        float s = sin(Rotation);
        float c = cos(Rotation);

        //center rotation matrix
        float2x2 rMatrix = float2x2(c, -s, s, c);
        rMatrix *= 0.5;
        rMatrix += 0.5;
        rMatrix = rMatrix * 2 - 1;

        //multiply the UVs by the rotation matrix
        UV.xy = mul(UV.xy, rMatrix);
        UV += Center;

        Out = UV;
    }

    void Unity_Polygon_float(float2 UV, float Sides, float Width, float Height, out float Out)
    {
        float pi = 3.14159265359;
        float aWidth = Width * cos(pi / Sides);
        float aHeight = Height * cos(pi / Sides);
        float2 uv = (UV * 2 - 1) / float2(aWidth, aHeight);
        uv.y *= -1;
        float pCoord = atan2(uv.x, uv.y);
        float r = 2 * pi / Sides;
        float distance = cos(floor(0.5 + pCoord / r) * r - pCoord) * length(uv);
        Out = saturate((1 - distance) / fwidth(distance));
    }

    void Unity_Multiply_float(float A, float B, out float Out)
    {
        Out = A * B;
    }

    void Unity_Checkerboard_float(float2 UV, float3 ColorA, float3 ColorB, float2 Frequency, out float3 Out)
    {
        UV = (UV.xy + 0.5) * Frequency;
        float4 derivatives = float4(ddx(UV), ddy(UV));
        float2 duv_length = sqrt(float2(dot(derivatives.xz, derivatives.xz), dot(derivatives.yw, derivatives.yw)));
        float width = 1.0;
        float2 distance3 = 4.0 * abs(frac(UV + 0.25) - 0.5) - width;
        float2 scale = 0.35 / duv_length.xy;
        float freqLimiter = sqrt(clamp(1.1f - max(duv_length.x, duv_length.y), 0.0, 1.0));
        float2 vector_alpha = clamp(distance3 * scale.xy, -1.0, 1.0);
        float alpha = saturate(0.5f + 0.5f * vector_alpha.x * vector_alpha.y * freqLimiter);
        Out = lerp(ColorA, ColorB, alpha.xxx);
    }

    // Graph Vertex
    // GraphVertex: <None>

    // Graph Pixel
    struct SurfaceDescription
{
    float4 Out;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float2 _Rotate_d83354f6ea3e41f2a420e8f0d530d4b4_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), IN.TimeParameters.x, _Rotate_d83354f6ea3e41f2a420e8f0d530d4b4_Out_3);
    float _Polygon_cae4b50a129b462393d8aae5bef030e5_Out_4;
    Unity_Polygon_float(_Rotate_d83354f6ea3e41f2a420e8f0d530d4b4_Out_3, 6, 0.5, 0.5, _Polygon_cae4b50a129b462393d8aae5bef030e5_Out_4);
    float _Multiply_f8c099ce569f4ff18debcd8f0b816956_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, -2, _Multiply_f8c099ce569f4ff18debcd8f0b816956_Out_2);
    float2 _Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_f8c099ce569f4ff18debcd8f0b816956_Out_2, _Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3);
    float _Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4;
    Unity_Polygon_float(_Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3, 3, 1, 1, _Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4);
    float3 _Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4;
    Unity_Checkerboard_float((_Polygon_cae4b50a129b462393d8aae5bef030e5_Out_4.xx), IsGammaSpace() ? float3(1, 0, 0) : SRGBToLinear(float3(1, 0, 0)), IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0)), (_Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4.xx), _Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4);
    surface.Out = all(isfinite(_Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4)) ? half4(_Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4.x, _Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4.y, _Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





    output.uv0 = input.texCoord0;
    output.TimeParameters = _Time*3; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

// --------------------------------------------------
// Main

#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/PreviewVaryings.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/PreviewPass.hlsl"

    ENDHLSL
}
    }
        FallBack "Hidden/Shader Graph/FallbackError"
}