Shader "LowPoly/Water" {
	Properties {
		[HideInInspector] _ReflectionTex("", 2D) = "white" {}
		[HideInInspector] _RefractionTex("", 2D) = "white" {}
		_Tint("Tint Colour", Color) = (1,1,1,1)
		_Speed("Speed", Float) = 1
		_Amplitude("Amplitude", Float) = 1
	}

	CGINCLUDE
		half Fresnel() {

		}
	ENDCG

	SubShader {
		Tags{"RenderType" = "Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "UnityCG.cginc"

			sampler2D _ReflectionTex;
			sampler2D _RefractionTex;
			fixed4 _Tint;
			float _Speed;
			float _Amplitude;

			struct a2v {
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 clipSpaceCoordGrid : TEXCOORD0;
				float4 pos : SV_POSITION;
				float4 clipSpaceCoordReal : TEXCOORD1;
				float3 viewDir : FLOAT;
			};
			
			v2f Vert(a2v v) {
				v2f o;
				o.clipSpaceCoordGrid = ComputeScreenPos(UnityObjectToClipPos(v.pos));		
				o.pos = UnityObjectToClipPos(v.pos);
				o.clipSpaceCoordReal = ComputeScreenPos(o.pos);
				o.viewDir = normalize(WorldSpaceViewDir(v.pos));
				return o;
			}

			fixed4 Frag(v2f i) : SV_Target {
				fixed4 finalcol;
				fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(i.clipSpaceCoordGrid));
				fixed4 refr = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.clipSpaceCoordGrid));
				fixed4 lightmix;
				lightmix = (refl + refr) / 2.0;
				finalcol = lightmix * _Tint;
				return finalcol;
			}
			ENDCG
		}
	}
}