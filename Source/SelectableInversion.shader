Shader "Hidden/ImageEffects/SelectableInversion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_MidCol("Half Inverted Color", 2D) = "gray" {}
		_IsColored("Colored Inversion", int) = 0
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
			
			uniform int _IsColored;
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
				fixed4 col = tex2D(_MainTex, i.uv1);
				fixed4 maskCol = tex2D(_Mask, i.uv2);

				maskCol = maskCol < 0 ? 0 : (maskCol > 1 ? 1 : maskCol);
				float4 colInv = float4(1 - col.rgb, col.a);
				float ratio = (maskCol.r + maskCol.g + maskCol.b) / 3.0;

				if (_IsColored > 0)
				{
					float midColStrength = 1.0 - 2 * abs(0.5 - ratio);
					float4 blendCol = _UseMaskCol ? maskCol : _MidCol;

					if (ratio > 0.5) { return midColStrength * blendCol + (1 - midColStrength) * colInv; }
					else { return midColStrength * blendCol + (1 - midColStrength) * col; }
				}
				else { return ratio * colInv + (1 - ratio) * col; }
			}
			ENDCG
		}
	}
}
