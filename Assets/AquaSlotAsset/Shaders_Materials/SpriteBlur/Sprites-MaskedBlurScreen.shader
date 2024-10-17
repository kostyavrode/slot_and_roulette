// Upgrade NOTE: upgraded instancing buffer 'PerDrawSprite' to new syntax.
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Description
// Sprite blur screen shader - blur all sprites under this
// use r-channel -  0 - not blured, 1 - blured, mask opacity is also propotional - _RendererColor.a
// used grabtexture and 2 blur passes horizontal and vertical
// normal blur size range 0-4

Shader "Custom/SpritesMaskedBlurScreen"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		_Size("Blur", Range(0, 30)) = 1
	}

		SubShader
	{
		// Horizontal blur
		GrabPass
	{

	}

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex SpriteVert
#pragma fragment SpriteFrag
#pragma target 2.0
#pragma multi_compile_instancing
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

#ifndef UNITY_SPRITES_INCLUDED
#define UNITY_SPRITES_INCLUDED

#include "UnityCG.cginc"
#ifdef UNITY_INSTANCING_ENABLED

		UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
		// SpriteRenderer.Color while Non-Batched/Instanced.
		UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
		// this could be smaller but that's how bit each entry is regardless of type
		UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
		UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

#define _RendererColor UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
#define _Flip UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

#endif // instancing

		CBUFFER_START(UnityPerDrawSprite)
#ifndef UNITY_INSTANCING_ENABLED
		fixed4 _RendererColor;
		fixed2 _Flip;
#endif
	float _EnableExternalAlpha;
	CBUFFER_END

		// Material Color.
		fixed4 _Color;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float4 uvgrab : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f SpriteVert(appdata_t IN)
	{
		v2f OUT;

		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

#ifdef UNITY_INSTANCING_ENABLED
		IN.vertex.xy *= _Flip.xy;
#endif

		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
		OUT.uvgrab  = ComputeGrabScreenPos(OUT.vertex);
		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	sampler2D _GrabTexture;
	float4 _GrabTexture_TexelSize;	//https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html  {TextureName}_TexelSize - a float4 property contains texture size information:(x contains 1.0 / width, y contains 1.0 / height, z contains width, w contains height)
	float _Size; // blur size
	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
		fixed4 alpha = tex2D(_AlphaTex, uv);
		color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
#endif

		return color;
	}

	fixed4 SpriteFrag(v2f i) : SV_Target
	{
		//fixed4 c = SampleSpriteTexture(i.texcoord) * i.color; c.rgb *= c.a; return c;  //default

		half4 sum = half4(0,0,0,0);
		fixed4 c = SampleSpriteTexture(i.texcoord);
		float alpha = c.r * i.color.a;;

		// UNITY_PROJ_COORD(a) - Given a 4 - component vector, this returns a Texture coordinate suitable for projected Texture reads. On most platforms this returns the given value directly.
		#define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Size * alpha, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
		
		sum += GRABPIXEL(0.05, -4.0);
		sum += GRABPIXEL(0.09, -3.0);
		sum += GRABPIXEL(0.12, -2.0);
		sum += GRABPIXEL(0.15, -1.0);
		sum += GRABPIXEL(0.18,  0.0);
		sum += GRABPIXEL(0.15, +1.0);
		sum += GRABPIXEL(0.12, +2.0);
		sum += GRABPIXEL(0.09, +3.0);
		sum += GRABPIXEL(0.05, +4.0);

		return sum;
	}

#endif // UNITY_SPRITES_INCLUDED
		ENDCG
	}

	// Vertical blur
	GrabPass
	{

	}
	Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex SpriteVert
#pragma fragment SpriteFrag
#pragma target 2.0
#pragma multi_compile_instancing
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

#ifndef UNITY_SPRITES_INCLUDED
#define UNITY_SPRITES_INCLUDED

#include "UnityCG.cginc"
#ifdef UNITY_INSTANCING_ENABLED

		UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
		// SpriteRenderer.Color while Non-Batched/Instanced.
		UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
		// this could be smaller but that's how bit each entry is regardless of type
		UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
		UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

#define _RendererColor UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
#define _Flip UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

#endif // instancing

		CBUFFER_START(UnityPerDrawSprite)
#ifndef UNITY_INSTANCING_ENABLED
		fixed4 _RendererColor;
	fixed2 _Flip;
#endif
	float _EnableExternalAlpha;
	CBUFFER_END

		// Material Color.
		fixed4 _Color;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float4 uvgrab : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f SpriteVert(appdata_t IN)
	{
		v2f OUT;

		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

#ifdef UNITY_INSTANCING_ENABLED
		IN.vertex.xy *= _Flip.xy;
#endif

		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
		//	OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y) + OUT.vertex.w) * 0.5;
		//	OUT.uvgrab.zw = OUT.vertex.zw;
		OUT.uvgrab = ComputeGrabScreenPos(OUT.vertex);
		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	sampler2D _GrabTexture;
	float4 _GrabTexture_TexelSize;	//https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html  {TextureName}_TexelSize - a float4 property contains texture size information:(x contains 1.0 / width, y contains 1.0 / height, z contains width, w contains height)
	float _Size; // blur size
	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
		fixed4 alpha = tex2D(_AlphaTex, uv);
		color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
#endif

		return color;
	}

	fixed4 SpriteFrag(v2f i) : SV_Target
	{
		half4 sum = half4(0,0,0,0);
		fixed4 c = SampleSpriteTexture(i.texcoord);// tex2D(_MaskTex, i.texcoord).r;
		float alpha = c.r *i.color.a;

		// UNITY_PROJ_COORD(a) - Given a 4 - component vector, this returns a Texture coordinate suitable for projected Texture reads. On most platforms this returns the given value directly.
		#define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely * _Size * alpha, i.uvgrab.z, i.uvgrab.w))) * weight
		sum += GRABPIXEL(0.05, -4.0);
		sum += GRABPIXEL(0.09, -3.0);
		sum += GRABPIXEL(0.12, -2.0);
		sum += GRABPIXEL(0.15, -1.0);
		sum += GRABPIXEL(0.18,  0.0);
		sum += GRABPIXEL(0.15, +1.0);
		sum += GRABPIXEL(0.12, +2.0);
		sum += GRABPIXEL(0.09, +3.0);
		sum += GRABPIXEL(0.05, +4.0);

		return sum;
	}

#endif // UNITY_SPRITES_INCLUDED
		ENDCG
	}

	}
}
