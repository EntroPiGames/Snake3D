Shader "Hidden/Outline"
{
	Properties
	{
		_OutlineSize("Outline Size", Range(0,10)) = 1		
		_OutlineColor("Outline color", Color) = (1, 1, 1, 1)
	}

	CGINCLUDE

	float _OutlineSize;	
	fixed4 _OutlineColor;

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 color : COLOR;
	};

	v2f vert(appdata v)
	{
		v2f o;

		o.pos = mul(unity_ObjectToWorld, v.vertex);

		float distance = length(_WorldSpaceCameraPos - o.pos.xyz);

		float3 worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;
		o.pos.xyz += worldNormal * ((_OutlineSize / 100) * distance);
		o.pos = mul(UNITY_MATRIX_VP, o.pos);

		o.color = _OutlineColor;

		return o;
	}
	
	float4 frag(v2f i) : COLOR
	{
		return i.color;
	}

	ENDCG

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}

		Pass
		{
			Stencil
			{
				Ref 1
				Comp NotEqual
			}

			ZTest GEqual
			Cull Off
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
	}
}