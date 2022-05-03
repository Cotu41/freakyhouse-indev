Shader "Custom/WeirdScreen"
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
        #define VARYINGS_NEED_POSITION_WS
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
        float3 positionWS;
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
        float3 WorldSpacePosition;
        float4 ScreenPosition;
        float4 uv0;
        float3 TimeParameters;
    };
    struct VertexDescriptionInputs
    {
    };
    struct PackedVaryings
    {
        float4 positionCS : SV_POSITION;
        float3 interp0 : TEXCOORD0;
        float4 interp1 : TEXCOORD1;
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
        output.interp0.xyz = input.positionWS;
        output.interp1.xyzw = input.texCoord0;
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
        output.positionWS = input.interp0.xyz;
        output.texCoord0 = input.interp1.xyzw;
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

    void Unity_InvertColors_float(float In, float InvertColors, out float Out)
    {
        Out = abs(InvertColors - In);
    }


    inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
    {
        float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
        UV = frac(sin(mul(UV, m)));
        return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
    }

    void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
    {
        float2 g = floor(UV * CellDensity);
        float2 f = frac(UV * CellDensity);
        float t = 8.0;
        float3 res = float3(8.0, 0.0, 0.0);

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                float2 lattice = float2(x,y);
                float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                float d = distance(lattice + offset, f);

                if (d < res.x)
                {
                    res = float3(d, offset.x, offset.y);
                    Out = res.x;
                    Cells = res.y;
                }
            }
        }
    }

    void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
    {
        float2 uv = ScreenPosition.xy * _ScreenParams.xy;
        float DITHER_THRESHOLDS[16] =
        {
            1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
            13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
            4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
            16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
        };
        uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
        Out = In - DITHER_THRESHOLDS[index];
    }

    void Unity_Blend_Exclusion_float(float Base, float Blend, out float Out, float Opacity)
    {
        Out = Blend + Base - (2.0 * Blend * Base);
        Out = lerp(Base, Out, Opacity);
    }

    void Unity_Multiply_float(float A, float B, out float Out)
    {
        Out = A * B;
    }


    inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
    {
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
    {
        return (1.0 - t) * a + (t * b);
    }


    inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
    {
        float2 i = floor(uv);
        float2 f = frac(uv);
        f = f * f * (3.0 - 2.0 * f);

        uv = abs(frac(uv) - 0.5);
        float2 c0 = i + float2(0.0, 0.0);
        float2 c1 = i + float2(1.0, 0.0);
        float2 c2 = i + float2(0.0, 1.0);
        float2 c3 = i + float2(1.0, 1.0);
        float r0 = Unity_SimpleNoise_RandomValue_float(c0);
        float r1 = Unity_SimpleNoise_RandomValue_float(c1);
        float r2 = Unity_SimpleNoise_RandomValue_float(c2);
        float r3 = Unity_SimpleNoise_RandomValue_float(c3);

        float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
        float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
        float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
        return t;
    }
    void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
    {
        float t = 0.0;

        float freq = pow(2.0, float(0));
        float amp = pow(0.5, float(3 - 0));
        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

        freq = pow(2.0, float(1));
        amp = pow(0.5, float(3 - 1));
        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

        freq = pow(2.0, float(2));
        amp = pow(0.5, float(3 - 2));
        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

        Out = t;
    }

    void Unity_Blend_Burn_float(float Base, float Blend, out float Out, float Opacity)
    {
        Out = 1.0 - (1.0 - Blend) / (Base + 0.000000000001);
        Out = lerp(Base, Out, Opacity);
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

    void Unity_Posterize_float3(float3 In, float3 Steps, out float3 Out)
    {
        Out = floor(In / (1 / Steps)) * (1 / Steps);
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
    Unity_Polygon_float(_Rotate_d83354f6ea3e41f2a420e8f0d530d4b4_Out_3, 5, 0.5, 0.5, _Polygon_cae4b50a129b462393d8aae5bef030e5_Out_4);
    float _InvertColors_815afb6e78124c3dab95657eddafadf7_Out_1;
    float _InvertColors_815afb6e78124c3dab95657eddafadf7_InvertColors = float(0
);    Unity_InvertColors_float(_Polygon_cae4b50a129b462393d8aae5bef030e5_Out_4, _InvertColors_815afb6e78124c3dab95657eddafadf7_InvertColors, _InvertColors_815afb6e78124c3dab95657eddafadf7_Out_1);
    float _Voronoi_ec1d7cc9ae994d1d950dc1653ccca32b_Out_3;
    float _Voronoi_ec1d7cc9ae994d1d950dc1653ccca32b_Cells_4;
    Unity_Voronoi_float(IN.uv0.xy, IN.TimeParameters.z, 5, _Voronoi_ec1d7cc9ae994d1d950dc1653ccca32b_Out_3, _Voronoi_ec1d7cc9ae994d1d950dc1653ccca32b_Cells_4);
    float _Dither_1c1cc636d2f143be9fe5c8b20e80b58f_Out_2;
    Unity_Dither_float(_Voronoi_ec1d7cc9ae994d1d950dc1653ccca32b_Out_3, float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _Dither_1c1cc636d2f143be9fe5c8b20e80b58f_Out_2);
    float _Blend_5a97dabad30a45e0b341eaed98038020_Out_2;
    Unity_Blend_Exclusion_float(_InvertColors_815afb6e78124c3dab95657eddafadf7_Out_1, -0.01, _Blend_5a97dabad30a45e0b341eaed98038020_Out_2, _Dither_1c1cc636d2f143be9fe5c8b20e80b58f_Out_2);
    float _Multiply_f8c099ce569f4ff18debcd8f0b816956_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, -2, _Multiply_f8c099ce569f4ff18debcd8f0b816956_Out_2);
    float2 _Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_f8c099ce569f4ff18debcd8f0b816956_Out_2, _Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3);
    float _Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4;
    Unity_Polygon_float(_Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3, 3, 1, 1, _Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4);
    float _SimpleNoise_34cd6b3bbe70454e99df98ee2168f002_Out_2;
    Unity_SimpleNoise_float(_Rotate_27cf8bb17b0e42b3b1a9d2a93d3bf385_Out_3, 27.1, _SimpleNoise_34cd6b3bbe70454e99df98ee2168f002_Out_2);
    float _Blend_b01fa0322df94737b88232d59a7910dd_Out_2;
    Unity_Blend_Burn_float(_Blend_5a97dabad30a45e0b341eaed98038020_Out_2, _Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4, _Blend_b01fa0322df94737b88232d59a7910dd_Out_2, _SimpleNoise_34cd6b3bbe70454e99df98ee2168f002_Out_2);
    float3 _Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4;
    Unity_Checkerboard_float((_Blend_b01fa0322df94737b88232d59a7910dd_Out_2.xx), IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0)), IsGammaSpace() ? float3(1, 0, 0) : SRGBToLinear(float3(1, 0, 0)), (_Polygon_c1b41c7e5b214954888dbdca21ba0bc2_Out_4.xx), _Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4);
    float3 _Posterize_7f8c24f7b0ef4cfc906ddb584d911335_Out_2;
    Unity_Posterize_float3(_Checkerboard_34ace9b1953f4e2d9aa8ec3770fe4c82_Out_4, float3(1, 1, 1), _Posterize_7f8c24f7b0ef4cfc906ddb584d911335_Out_2);
    surface.Out = all(isfinite(float3(0, 0, 0))) ? half4(_Posterize_7f8c24f7b0ef4cfc906ddb584d911335_Out_2.x, _Posterize_7f8c24f7b0ef4cfc906ddb584d911335_Out_2.y, _Posterize_7f8c24f7b0ef4cfc906ddb584d911335_Out_2.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
    output.TimeParameters = _Time; // This is mainly for LW as HD overwrite this value
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