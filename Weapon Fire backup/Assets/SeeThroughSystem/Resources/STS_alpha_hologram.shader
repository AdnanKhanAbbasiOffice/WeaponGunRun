Shader "See-Through System/Alpha_Hologram" {
Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Pass {
			// Draw front and back faces
			Cull Off
			// Don't write to depth buffer 
			ZWrite Off
			// in order not to occlude other objects
			//ZTest Always

			Blend SrcAlpha One // Additive blending
			
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            fixed4 _Color;
			fixed4 _sts_effect_color;
			float _sts_color_factor;

            fixed4 frag(v2f_img i) : COLOR 
			{
                fixed4 c = tex2D(_MainTex, i.uv);
				c = c * _sts_effect_color * c.a * _Color.a;
				fixed brightness =  c.r * 0.21 + c.g * 0.72 + c.b * 0.07;
				fixed4 nocol = _sts_effect_color * (brightness * 0.7 +0.3);
				nocol = nocol * nocol.a * _Color.a;
				c = lerp(nocol,c,_sts_color_factor);
				return c;
            }
            ENDCG
        }
    }
}