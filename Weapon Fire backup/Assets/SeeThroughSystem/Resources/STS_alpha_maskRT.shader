// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_alpha_mask_RT" {

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
			float4 scrPos : TEXCOORD0;			
		};

		sampler2D_float _CameraDepthTexture;
		
		half4 _MainTex_TexelSize;
		fixed4 _STScolor;
		float _STSObstDepthShift;
		
		v2f vert (appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);		
			o.scrPos = ComputeScreenPos(o.pos);			
			COMPUTE_EYEDEPTH(o.scrPos.z);
			return o;
		}		

		fixed4 frag(v2f i) : COLOR 
		{ 			
			
			float origDepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
			float depth = i.scrPos.z;
			
			
			fixed a = 0.5;
			if (depth + _STSObstDepthShift > origDepth)
				a = 1;
			fixed4 c = _STScolor;			
			c.a = a;
			
			return c;			
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
			float4 scrPos : TEXCOORD0;			
		};

		sampler2D_float _CameraDepthTexture;
		
		half4 _MainTex_TexelSize;
		fixed4 _STScolor;
		float _STSObstDepthShift;
		
		v2f vert (appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);		
			o.scrPos = ComputeScreenPos(o.pos);			
			COMPUTE_EYEDEPTH(o.scrPos.z);
			return o;
		}		

		fixed4 frag(v2f i) : COLOR 
		{ 			
			
			float origDepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
			float depth = i.scrPos.z;
			
			
			fixed a = 0.5;
			if (depth + _STSObstDepthShift > origDepth)
				a = 1;
			fixed4 c = _STScolor;			
			c.a = a;
			
			return c;			
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
			};

			struct v2f {
				float4 vertex : SV_POSITION;				
				half2 texcoord : TEXCOORD0;	
				float4 scrPos : TEXCOORD1;
			};

			sampler2D _MainTex;			
			float4 _MainTex_ST;
			sampler2D_float _CameraDepthTexture;
			fixed4 _STScolor;
			float _STSObstDepthShift;
			float _alphaDiscard;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.scrPos = ComputeScreenPos(o.vertex);			
				COMPUTE_EYEDEPTH(o.scrPos.z);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				if (col.a < _alphaDiscard) discard;
				float origDepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
				float depth = i.scrPos.z;
						
				fixed a = 0.5;
				if (depth + _STSObstDepthShift > origDepth)
					a = 1;
				fixed4 c = _STScolor;			
				c.a = a;
				return c;
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
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float4 scrPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				sampler2D_float _CameraDepthTexture;
				fixed4 _STScolor;
				float _STSObstDepthShift;
			

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.scrPos = ComputeScreenPos(o.vertex);			
					COMPUTE_EYEDEPTH(o.scrPos.z);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					if (col.a < _Cutoff) discard;
					
					float origDepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
					float depth = i.scrPos.z;
						
					fixed a = 0.5;
					if (depth + _STSObstDepthShift > origDepth)
						a = 1;
					fixed4 c = _STScolor;			
					c.a = a;
					
					return c;
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
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float4 scrPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				sampler2D_float _CameraDepthTexture;
				fixed4 _STScolor;
				float _STSObstDepthShift;
			

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.scrPos = ComputeScreenPos(o.vertex);			
					COMPUTE_EYEDEPTH(o.scrPos.z);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					if (col.a < _Cutoff) discard;
					
					float origDepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
					float depth = i.scrPos.z;
						
					fixed a = 0.5;
					if (depth + _STSObstDepthShift > origDepth)
						a = 1;
					fixed4 c = _STScolor;			
					c.a = a;
					
					return c;
				}
			ENDCG
		}
	}
}