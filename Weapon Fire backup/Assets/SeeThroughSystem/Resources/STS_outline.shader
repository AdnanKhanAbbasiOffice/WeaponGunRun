Shader "See-Through System/Outline" {
	Properties {
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
	}
 
CGINCLUDE
#include "UnityCG.cginc"
 
struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;	
};
 
struct v2f {
	float4 pos : POSITION;
	fixed4 color : COLOR;
	float4 scrPos : TEXCOORD0;
};
 
uniform float _stw_outline;
uniform float4 _sts_effect_color;
 
v2f vert(appdata v) {
	// just make a copy of incoming vertex data but scaled according to normal direction
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	
	o.scrPos = ComputeScreenPos(o.pos);
	COMPUTE_EYEDEPTH(o.scrPos.z);
 
	float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
	float2 offset = TransformViewToProjection(norm.xy);
	
	float t = unity_CameraProjection._m11;
    float Rad2Deg = 180 / UNITY_PI;
    float fov = atan(1.0f / t ) * 2.0 * Rad2Deg;
 
	o.pos.xy += offset * o.pos.z * _stw_outline * o.scrPos.z/10 * fov/60;
	o.color = _sts_effect_color;
	return o;
}
ENDCG
 
	SubShader {
		Tags { "Queue" = "Transparent" }
 
		Pass {
			Name "BASE"
			Cull Back
			//Blend Zero One
 
			// uncomment this to hide inner details:
			Offset -8, -8
 
			SetTexture [_sts_effect_color] {
				ConstantColor (0,0,0,0)
				Combine constant
			}
		}
 
		// note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
 
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
 
half4 frag(v2f i) :COLOR {
	return i.color;
}
ENDCG
		}
 
 
	}
 
	Fallback "Diffuse"
}