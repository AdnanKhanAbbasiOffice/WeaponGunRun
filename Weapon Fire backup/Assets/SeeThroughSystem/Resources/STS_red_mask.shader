// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_red_mask" {
SubShader {
    Tags { "RenderType"="Opaque" }
	ZWrite On
    Pass {
        Fog { Mode Off }
CGPROGRAM

#pragma vertex vert
#pragma fragment frag

		struct v2f {
			float4 pos : POSITION;    
		};

		float4 vert(float4 v:POSITION) : SV_POSITION {
			float4 retv = UnityObjectToClipPos (v);			
			return retv;
            }

		half4 frag() : COLOR 
		{ 
			return fixed4(1.0,0.0,0.0,0.0);
		}
		
ENDCG
    }
}
}