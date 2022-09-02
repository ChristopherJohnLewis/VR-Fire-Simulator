// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Tree Bark Override" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_MainBurnedTex("Burned Base (RGB) Alpha (A)", 2D) = "black" {}
    _BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
    _TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}
    _Cutoff("Alpha cutoff", Range(0,1)) = 0.3
	[HideInInspector] _BurnMap ("Time of Arrival Reference Map", 2D) = "white" {}
	_FireSize ("Burning time", Float) = 0
	_FirePos ("Burning time start offset", Float) = 0
	[HideInInspector] _RelativePosition ("Terrain Size", Vector) = (1,1,0,0)

    // These are here only to provide default values
    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
    [HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
    [HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
    [HideInInspector] _SquashAmount ("Squash", Float) = 1
}

SubShader {
    Tags { "IgnoreProjector"="True" "RenderType"="TreeBark" }
    LOD 200

CGPROGRAM
#pragma surface surf BlinnPhong vertex:TreeVertBark addshadow nolightmap
#pragma multi_compile __ BILLBOARD_FACE_CAMERA_POS _EMISSION
#include "UnityBuiltin3xTreeLibrary.cginc"
#include "BlendFuncs.cginc"

sampler2D _MainTex;
sampler2D _MainBurnedTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

half3 _EmissionColor;
float _FireSize;
float _FirePos;

struct Input {
    float2 uv_MainTex;
    fixed4 color : COLOR;
#if defined(BILLBOARD_FACE_CAMERA_POS)
    float4 screenPos;
#endif
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb * IN.color.rgb * IN.color.a;

    fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
    o.Gloss = trngls.a * _Color.r;
    o.Alpha = c.a;

	//Get burn time based on progression
	fixed3 worldPos = mul(unity_ObjectToWorld, fixed4(0, 0, 0, 1)).xyz;
	BurnState simState = currentBurnState(_FireSize, _FirePos, worldPos);
	o.Emission = _EmissionColor * paperBurnBlend(IN.uv_MainTex, 1 - simState.fireLine, 0.1) *2.0f;
	
	//modify tree texture to burn
	o.Albedo = lerp(tex2D(_MainBurnedTex, IN.uv_MainTex), o.Albedo, simState.burnState);

#if defined(BILLBOARD_FACE_CAMERA_POS)
    float coverage = 1.0;
    if (_TreeInstanceColor.a < 1.0)
        coverage = ComputeAlphaCoverage(IN.screenPos, _TreeInstanceColor.a);
    o.Alpha *= coverage;
#endif
    half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
    o.Specular = norspc.r;
    o.Normal = UnpackNormalDXT5nm(norspc);
}
ENDCG
}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Bark Rendertex"
}
