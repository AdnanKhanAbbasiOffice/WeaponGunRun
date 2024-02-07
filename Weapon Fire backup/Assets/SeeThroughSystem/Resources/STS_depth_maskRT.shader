// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_depth_mask_RT" {

	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader
	{
	    Tags { "RenderType"="Opaque" }
	    Cull Off
	    Lighting Off
		ZWrite On	
	    Pass 
	    {
	    Fog { Mode Off }
		CGPROGRAM		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
								
		struct v2f {
			float4 pos : SV_POSITION;			
			float2 depth : TEXCOORD0;
		};		
				
		v2f vert (appdata_base v)
		{
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f, o);
			o.pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_DEPTH(o.depth);
			return o;
		}		

		half4 frag(v2f i) : SV_Target 
		{ 						
			UNITY_OUTPUT_DEPTH(i.depth);
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
	    Pass 
	    {
	    Fog { Mode Off }
		CGPROGRAM		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
								
		struct v2f {
			float4 pos : SV_POSITION;			
			float2 depth : TEXCOORD0;
		};		
				
		v2f vert (appdata_base v)
		{
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f, o);
			o.pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_DEPTH(o.depth);
			return o;
		}		

		half4 frag(v2f i) : SV_Target 
		{ 						
			UNITY_OUTPUT_DEPTH(i.depth);
		}
		
		ENDCG
	    }
	}
	
	SubShader 
	{
	
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	Lighting Off
	Cull Off
	ZWrite On
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 pos : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;				
				half2 texcoord : TEXCOORD0;	
				float2 depth : TEXCOORD1;
			};

			sampler2D _MainTex;			
			float4 _MainTex_ST;			
			float _alphaDiscard;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.pos = UnityObjectToClipPos(v.pos);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_DEPTH(o.depth);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				
				if (col.a < _alphaDiscard)				
					discard;				
					
				UNITY_OUTPUT_DEPTH(i.depth);
			}
		ENDCG
	}
} 

	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}		
		
		Lighting Off
		Cull Off		
		ZWrite On

		Pass 
		{  
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 pos : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float2 depth : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;				
			

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_INITIALIZE_OUTPUT(v2f, o);
					o.pos = UnityObjectToClipPos(v.pos);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_DEPTH(o.depth);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					
					if (col.a < _Cutoff)
						discard;
						
					UNITY_OUTPUT_DEPTH(i.depth);
				}
			ENDCG
		}
	}
	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TreeTransparentCutout"}		
		
		Lighting Off
		Cull Off		
		ZWrite On

		Pass 
		{  
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 pos : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float2 depth : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;				
			

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_INITIALIZE_OUTPUT(v2f, o);
					o.pos = UnityObjectToClipPos(v.pos);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_DEPTH(o.depth);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					
					if (col.a < _Cutoff)
						discard;
						
					UNITY_OUTPUT_DEPTH(i.depth);
				}
			ENDCG
		}
	}
}