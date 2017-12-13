// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef TOD_BASE_INCLUDED
#define TOD_BASE_INCLUDED

#include "UnityCG.cginc"

uniform sampler2D TOD_BayerTexture;
uniform sampler2D TOD_CloudTexture;

uniform float4x4 TOD_World2Sky;
uniform float4x4 TOD_Sky2World;

uniform float3 TOD_SunLightColor;
uniform float3 TOD_MoonLightColor;

uniform float3 TOD_SunSkyColor;
uniform float3 TOD_MoonSkyColor;

uniform float3 TOD_SunMeshColor;
uniform float3 TOD_MoonMeshColor;

uniform float3 TOD_SunCloudColor;
uniform float3 TOD_MoonCloudColor;

uniform float3 TOD_FogColor;
uniform float3 TOD_GroundColor;
uniform float3 TOD_AmbientColor;

uniform float3 TOD_SunDirection;
uniform float3 TOD_MoonDirection;
uniform float3 TOD_LightDirection;

uniform float3 TOD_LocalSunDirection;
uniform float3 TOD_LocalMoonDirection;
uniform float3 TOD_LocalLightDirection;

uniform float TOD_Contrast;
uniform float TOD_Brightness;
uniform float TOD_Fogginess;

uniform float TOD_MoonHaloPower;
uniform float3 TOD_MoonHaloColor;

uniform float TOD_CloudOpacity;
uniform float TOD_CloudCoverage;
uniform float TOD_CloudSharpness;
uniform float TOD_CloudDensity;
uniform float TOD_CloudColoring;
uniform float TOD_CloudAttenuation;
uniform float TOD_CloudSaturation;
uniform float TOD_CloudScattering;
uniform float TOD_CloudBrightness;
uniform float3 TOD_CloudOffset;
uniform float3 TOD_CloudWind;
uniform float3 TOD_CloudSize;

uniform float TOD_CloudShadowCutoff;
uniform float TOD_CloudShadowFade;
uniform float TOD_CloudShadowIntensity;

uniform float TOD_StarSize;
uniform float TOD_StarBrightness;
uniform float TOD_StarVisibility;

uniform float TOD_SunMeshContrast;
uniform float TOD_SunMeshBrightness;

uniform float TOD_MoonMeshContrast;
uniform float TOD_MoonMeshBrightness;

uniform float3 TOD_ScatterDensity;

uniform float3 TOD_kBetaMie;
uniform float4 TOD_kSun;
uniform float4 TOD_k4PI;
uniform float4 TOD_kRadius;
uniform float4 TOD_kScale;

// Vertex transform used by the entire sky dome
#if UNITY_VERSION >= 540
#define TOD_TRANSFORM_VERT(vert) UnityObjectToClipPos(vert)
#else
#define TOD_TRANSFORM_VERT(vert) UnityObjectToClipPos(vert)
#endif

// UV rotation matrix constructor
#define TOD_ROTATION_UV(angle) float2x2(cos(angle), -sin(angle), sin(angle), cos(angle))

// Fast and simple tonemapping
#define TOD_HDR2LDR(color) (1.0 - exp2(-TOD_Brightness * color))

// Approximates gamma by 2.0 instead of 2.2
#define TOD_GAMMA2LINEAR(color) (color * color)
#define TOD_LINEAR2GAMMA(color) sqrt(color)

// Matrices
#if UNITY_VERSION >= 540
#define TOD_Object2World unity_ObjectToWorld
#define TOD_World2Object unity_WorldToObject
#else
#define TOD_Object2World unity_ObjectToWorld
#define TOD_World2Object unity_WorldToObject
#endif

// Screen space adjust
#if UNITY_VERSION >= 540
#define TOD_UV(x, y) UnityStereoScreenSpaceUVAdjust(x, y)
#else
#define TOD_UV(x, y) x
#endif

// Stereo output
#if UNITY_VERSION >= 540
#define TOD_VERTEX_OUTPUT_STEREO UNITY_VERTEX_OUTPUT_STEREO
#define TOD_INITIALIZE_VERTEX_OUTPUT_STEREO(o) UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
#else
#define TOD_VERTEX_OUTPUT_STEREO
#define TOD_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
#endif

// Instancing
#if UNITY_VERSION >= 540
#define TOD_INSTANCE_ID UNITY_VERTEX_INPUT_INSTANCE_ID
#define TOD_SETUP_INSTANCE_ID(o) UNITY_SETUP_INSTANCE_ID(o)
#else
#define TOD_INSTANCE_ID
#define TOD_SETUP_INSTANCE_ID(o)
#endif

#endif
