Shader "Unlit/PortalShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FallbackTex ("Fallback Texture", 2D) = "white" {}
		[Toggle] _UseFallback ("Use Fallback", Float) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		// Cull Off
		ZWrite On
		ZTest Less		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID //new for vr
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;

				//new for vr
				UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata v)
			{
				v2f o;

				//new for vr
				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _FallbackTex;
			float _UseFallback;

			fixed4 frag (v2f i) : SV_Target
			{
				//new for vr
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				if (_UseFallback)
				{
					return tex2D(_FallbackTex, i.uv);
				}
				i.screenPos /= i.screenPos.w;
				fixed4 col = tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y));
				
				return col;
			}
			ENDCG
		}
	}
}
