// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/STS_compose" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorMask ("Color mask", 2D) = "black" {}
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
fixed _BlurSpilling;
fixed _ColorStrength;

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
	#if defined(UNITY_HALF_TEXEL_OFFSET)		
		o.uv1.y = o.uv.y - _MainTex_TexelSize * 2;
    #else		
	#endif
	
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
	fixed4 ms = tex2D(_MaskTex, i.uv1);
	fixed4 oms = tex2D(_ObstTrigMaskTex, i.uv1);	
	
	fixed cmax = max(ms.r,ms.g);
	cmax = max(cmax,ms.b);
	
	fixed4 colormask = saturate(ms/cmax * _ColorStrength);
	
	bT = bT * _TintColor * 2 * ((1,1,1,1) - colormask);
 
 	fixed4 c = lerp(mT,bT,ms.a * saturate(_BlurSpilling + oms.r));	
			
	return c;	
}
 ENDCG
}
} 
}