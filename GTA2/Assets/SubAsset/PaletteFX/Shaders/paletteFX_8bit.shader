Shader "PaletteFX/8bit" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_MaskTex("Mask", 2D) = "white" {}
		_PaletteTex("Palette", 2D) = "white" {}
		_ShadeCount("Shade Count", Int) = 4
		_HueCount("HueCount", Int) = 8
		_ShadeShift("Shade Shift", Vector) = (0, 0, 0, 0)
		_HueShift("Hue Shift", Vector) = (0, 0, 0, 0)			

	}
		SubShader{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			LOD 100

			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile _ PIXELSNAP_ON

#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	sampler2D _PaletteTex;
	sampler2D _MaskTex;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
#ifdef PIXELSNAP_ON
		o.vertex = UnityPixelSnap(o.vertex);
#endif
		return o;
	}

	fixed _RedChannelShift;
	fixed _GreenChannelShift;
	fixed _BlueChannelShift;
	fixed _ShadeCount;
	fixed _HueCount;

	fixed4 _ShadeShift;
	fixed4 _HueShift;

	fixed4 _Color;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.texcoord);
		fixed alpha = col.a;

		int colorCount = _ShadeCount * _HueCount;
		fixed index = col.r * colorCount;
		fixed temp = index / _ShadeCount;
		fixed hue = floor(temp);
		fixed shade = floor(frac(temp) * _ShadeCount);
		
		fixed3 mask = tex2D(_MaskTex, i.texcoord).rgb;
		fixed shadeShift = dot(mask, _ShadeShift.rgb);

		shade += shadeShift;
		shade = clamp(shade, 0, _ShadeCount-1);

		fixed hueShift = dot(mask, _HueShift.rgb);
		hue = floor(hue + hueShift);
		hue = clamp(hue, 0, max(1, _HueCount - 1));
		
		index = hue * _ShadeCount + shade;		
		col = tex2D(_PaletteTex, fixed2(index / colorCount, 0.5)) * _Color;

		col.a *= alpha;

		return col;
	}
		ENDCG
	}
	}

}

