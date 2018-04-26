Shader "Hidden/ImageEffects/SelectableInversion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_MidCol("Half Inverted Color", 2D) = "gray" {}
		_IsColoured("Coloured Inversion", int) = 0
		_UseMaskCol("Use Mask Color", int) = 0
	}
	SubShader
	{
		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Mask;
			float4 _Mask_ST;

			uniform float4 _MidCol;
			
			uniform int _IsColoured;
			uniform int _UseMaskCol;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _Mask);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 Col = tex2D(_MainTex, i.uv1);
				fixed4 MaskCol = tex2D(_Mask, i.uv2);
				MaskCol = MaskCol < 0 ? 0 : (MaskCol > 1 ? 1 : MaskCol);
				float4 ColInv = float4(Col.a - Col.rgb, Col.a);
				float Ratio = (MaskCol.r + MaskCol.g + MaskCol.b) / 3.0;
				if (_IsColoured > 0)
				{
					float MidColStrength = 1.0 - 2 * abs(0.5 - Ratio);
					float4 BlendCol = _UseMaskCol ? MaskCol : _MidCol;
					if (Ratio > 0.5) { return MidColStrength * BlendCol + (1 - MidColStrength) * ColInv; }
					else { return MidColStrength * BlendCol + (1 - MidColStrength) * Col; }
				}
				else { return Ratio * ColInv + (1 - Ratio) * Col; }
			}
			ENDCG
		}
	}
}
