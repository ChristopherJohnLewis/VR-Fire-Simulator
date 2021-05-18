// This shader is built off the Unity 2018.3 standard terrain shader, modified to handle special blending modes
// based on texture height data and splatmap properties

Shader "Custom/2018Terrain"
{
	Properties{
		// used in fallback on old cards & base map
		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)

		//Definitions for special transition textures
		//TODO add these transition textures
		_BlendControl("Rock Blend Control", 2D) = "white" {}
		[HideInInspector] _BurnMap("Burn Map", 2D) = "black" {}
		_VegMap("Vegetation Map", 2D) = "black" {}
		_BurnMaxVal("Simulation End Time", Float) = 10.0
		_Power("Rock Power", Float) = 0.5
		_RockControl("Rock Control", Float) = 0.5
		_FireFrontSize("Fire Line Size", Float) = 1
		_FireFrontPosition("Fire Line Position", Float) = 1
		_FireFrontColor("Fire line color", Color) = (1,1,1,1)
		_SatelliteMap("Satellite View", 2D) = "black" {}
	}

		SubShader{
			Tags {
				"Queue" = "Geometry-100"
				"RenderType" = "Opaque"
			}

			CGPROGRAM
			#pragma surface surf Standard vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer addshadow fullforwardshadows
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
			#pragma multi_compile_fog // needed because finalcolor oppresses fog code generation.
			#pragma target 3.0
			// needs more than 8 texcoords
			#pragma exclude_renderers gles
			#include "UnityPBSLighting.cginc"

			#pragma multi_compile __ _NORMALMAP

			#define TERRAIN_STANDARD_SHADER
			#define TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard
			#include "BlendFuncs.cginc"
			#include "SplatmapOverride.cginc"

			half _Metallic0;
			half _Metallic1;
			half _Metallic2;
			half _Metallic3;

			half _Smoothness0;
			half _Smoothness1;
			half _Smoothness2;
			half _Smoothness3;

			sampler2D _BlendControl;
			sampler2D _VegMap;
			sampler2D _SatelliteMap;
			half _Power;
			half _RockControl;
			float _BurnMaxVal;
			float _FireFrontSize;
			float _FireFrontPosition;
			fixed4 _FireFrontColor;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				//pack visual data
				VisualData data;
				data.rockHeight = _BlendControl;
				data.rockPower = _Power;
				data.rockControl = _RockControl;
				data.burnProgress = _BurnProgress;
				data.maxBurnVal = _BurnMaxVal;
				data.burnMap = _BurnMap;
				data.fireFrontSize = _FireFrontSize;
				data.fireFrontPos = _FireFrontPosition;
				data.fireFrontColor = _FireFrontColor;
				data.vegMap = _VegMap;
				data.satMap = _SatelliteMap;

				half4 splat_control;
				half weight;
				fixed4 mixedDiffuse;
				fixed3 emissionMap;
				half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);

				//TODO update smoothness based on burn map ()
				SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal, data, emissionMap);
				o.Albedo = mixedDiffuse.rgb;
				o.Alpha = 1.0f;
				o.Smoothness = mixedDiffuse.a;
				o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
				//TODO update fire line to glow
				o.Emission = emissionMap;
			}
			ENDCG

			UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
			UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
		}

	Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Standard-AddPass"
	Dependency "BaseMapShader" = "Hidden/TerrainEngine/Splatmap/Standard-Base"
	Dependency "BaseMapGenShader" = "Hidden/TerrainEngine/Splatmap/Standard-BaseGen"

	Fallback "Nature/Terrain/Diffuse"
}
