Shader "Hidden/STS_clean_alpha" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}		
	}

	SubShader 
	{
		Pass
		{
		
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag		
		
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;		
		
		fixed4 frag(v2f_img i) : COLOR
		{
			fixed4 c = tex2D(_MainTex, i.uv);
			
			if (c.a < 0.9)
				c.a = 0;
			
			return c;
			
		}

		ENDCG
		} 
	}
}