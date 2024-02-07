// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_red_mask_RT_TM" {

	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_STS_TransparencyMask ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader
	{
	    Tags { "RenderType"="Opaque" }
	    Cull Off
	    Lighting Off
		ZWrite On	
	    Pass {  
		Fog { Mode Off }
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};
			
			sampler2D _STS_TransparencyMask;
			float4 _STS_TransparencyMask_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _STS_TransparencyMask);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_STS_TransparencyMask, i.texcoord);
				return fixed4(saturate(c.r*2),0.0,0.0,0.0);
			}
		ENDCG
	}
	}
	
	SubShader
	{
	    Tags { "RenderType"="TreeOpaque" }
	    Cull Off
	    Lighting Off
		ZWrite On	
	    Pass {  
			Fog { Mode Off }
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};
				
				sampler2D _STS_TransparencyMask;
				float4 _STS_TransparencyMask_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _STS_TransparencyMask);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 c = tex2D(_STS_TransparencyMask, i.texcoord);
					return fixed4(saturate(c.r*2),0.0,0.0,0.0);
				}
			ENDCG
		}
	}
	
	SubShader 
	{
	
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	Lighting Off
	Cull Off
	ZWrite Off	
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordM : TEXCOORD1;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 texcoordM : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _alphaDiscard;
			sampler2D _STS_TransparencyMask;
			float4 _STS_TransparencyMask_ST;			
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoordM = TRANSFORM_TEX(v.texcoord, _STS_TransparencyMask);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				if (col.a < _alphaDiscard) discard;
				fixed4 c = tex2D(_STS_TransparencyMask, i.texcoordM);
				return fixed4(saturate(c.r*2),0.0,0.0,0.0);
			}
		ENDCG
	}
} 

	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}		

		ZWrite On
		Lighting Off
		Cull Off		

		Pass 
		{  
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float2 texcoordM : TEXCOORD1;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					half2 texcoordM : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				sampler2D _STS_TransparencyMask;
				float4 _STS_TransparencyMask_ST;
			
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoordM = TRANSFORM_TEX(v.texcoord, _STS_TransparencyMask);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					if (col.a < _Cutoff) discard;
					fixed4 c = tex2D(_STS_TransparencyMask, i.texcoordM);
					return fixed4(saturate(c.r*2),0.0,0.0,0.0);
				}
			ENDCG
		}
	}
	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TreeTransparentCutout"}		

		ZWrite On
		Lighting Off
		Cull Off		

		Pass 
		{  
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float2 texcoordM : TEXCOORD1;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					half2 texcoordM : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				sampler2D _STS_TransparencyMask;
				float4 _STS_TransparencyMask_ST;
			
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoordM = TRANSFORM_TEX(v.texcoord, _STS_TransparencyMask);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					if (col.a < _Cutoff) discard;
					fixed4 c = tex2D(_STS_TransparencyMask, i.texcoordM);
					return fixed4(saturate(c.r*2),0.0,0.0,0.0);
				}
			ENDCG
		}
	}
}