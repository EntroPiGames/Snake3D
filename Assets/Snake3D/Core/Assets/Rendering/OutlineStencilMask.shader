Shader "Hidden/Outline Stencil Mask"
{
	Properties
	{
		_OutlineSize("Outline Size", Range(0,10)) = 1	
	}
	
	SubShader
	{
		ColorMask 0
		Pass
		{
			ZTest GEqual
			ZWrite Off

			Stencil
			{
				Ref 1
				Comp always
				Pass replace
			}

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			float _OutlineSize;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.pos = mul(unity_ObjectToWorld, v.vertex);

				float distance = length(_WorldSpaceCameraPos - o.pos.xyz);

				float3 worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;
				o.pos.xyz -= worldNormal * ((_OutlineSize / 100) * distance);
				o.pos = mul(UNITY_MATRIX_VP, o.pos);

				return o;
			}
			
			half4 frag(v2f i) : COLOR
			{
				return half4(0,0,1,1);
			}
			
			ENDCG
		}
	}
}