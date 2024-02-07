// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_MergeMasks" 
{
Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}		
	}
SubShader {
  Pass{
 CGPROGRAM
#include "UnityCG.cginc"
#pragma vertex vert
#pragma fragment frag	

uniform sampler2D _MainTex ;
uniform sampler2D _ObstMaskTex;

half4 _MainTex_TexelSize;

struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;		
		#if UNITY_UV_STARTS_AT_TOP	
		float2 uv1 : TEXCOORD1;				
		#else
		#endif
	};


v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );	
	#if UNITY_UV_STARTS_AT_TOP	
	o.uv1 = o.uv;
	if (_MainTex_TexelSize.y < 0)
		o.uv1.y = 1-o.uv1.y;		
	#else
	#endif
	return o;
}
 

//Fragment Shader
 fixed4 frag (v2f i) : COLOR{
 	fixed4 mT = tex2D(_MainTex , i.uv);
	
	#if UNITY_UV_STARTS_AT_TOP
	fixed4 oms = tex2D(_ObstMaskTex, i.uv1);		
	#else
	fixed4 oms = tex2D(_ObstMaskTex, i.uv);	
	#endif
	
	if (mT.a > 0.1 && mT.a < 0.9)
		oms.r = 0;
	
	return oms;
}
 ENDCG
}
} 
}