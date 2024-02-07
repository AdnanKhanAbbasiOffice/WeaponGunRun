// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_alpha_mask" {
	SubShader
	{
	    Tags { "RenderType"="Opaque" }
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
			float depth = i.scrPos.z  + _STSObstDepthShift;
			
			
			fixed a = 0.5;
			
			if (depth > origDepth)
				a = 1;
				
			//a = saturate(0.5 + saturate((depth - origDepth) * 1000));
				
			fixed4 c = _STScolor;
			
			c.a = a;
			//c.a = saturate(0.5 + saturate((depth - origDepth) * 1000));
			
			return c;			
		}
		
		ENDCG
	    }
	}
}