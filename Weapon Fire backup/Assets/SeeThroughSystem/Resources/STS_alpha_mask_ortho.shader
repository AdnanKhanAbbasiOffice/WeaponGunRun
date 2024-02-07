Shader "Hidden/STS_alpha_mask_ortho" {
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
			o.pos = UnityObjectToClipPos(v.vertex);	
			o.scrPos = ComputeScreenPos(o.pos);			
			COMPUTE_EYEDEPTH(o.scrPos.z);
			return o;
		}		

		fixed4 frag(v2f i) : COLOR 
		{ 			
			
			float origDepth = ( (1.0-_ProjectionParams.x)/2.0 + _ProjectionParams.x * SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r) * (_ProjectionParams.z - _ProjectionParams.y)  + _ProjectionParams.y;
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