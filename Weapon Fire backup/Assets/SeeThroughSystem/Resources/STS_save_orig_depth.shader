Shader "Hidden/STS_save_orig_depth" 
{
	
 SubShader { 

 Pass{
 CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"		

//uniform sampler2D _MainTex;
uniform sampler2D_float _CameraDepthTexture;


//Fragment Shader
 float4 frag (v2f_img i) : COLOR{	
	return tex2D(_CameraDepthTexture, i.uv);	
}
 ENDCG
}
} 
}