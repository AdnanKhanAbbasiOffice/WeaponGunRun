// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_compose_no_color" 
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

uniform sampler2D _MainTex;
uniform sampler2D _BackTex;
uniform sampler2D _MaskTex;
uniform sampler2D _ObstTrigMaskTex;

half4 _MainTex_TexelSize;

fixed4 _TintColor;
float _BlurSpilling;

struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;		
		float2 uv1 : TEXCOORD1;						
	};


v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );	
	
	o.uv1 = o.uv;
	
	#if UNITY_UV_STARTS_AT_TOP		
	if (_MainTex_TexelSize.y < 0)
		o.uv1.y = 1-o.uv1.y;		
	#else
	#endif
	return o;
}
 

//Fragment Shader
 fixed4 frag (v2f i) : COLOR{
	fixed4 mT = tex2D(_MainTex, i.uv);	
	fixed4 bT = tex2D(_BackTex, i.uv1);	
	fixed ms = tex2D(_MaskTex, i.uv1).a;
	fixed oms = tex2D(_ObstTrigMaskTex, i.uv1).r;		

	bT = bT * _TintColor * 2;
	
	fixed coef = ms * saturate(_BlurSpilling + oms);
 
	fixed4 c = lerp(mT,bT,coef);
		
	return c;
}
 ENDCG
}
} 
}