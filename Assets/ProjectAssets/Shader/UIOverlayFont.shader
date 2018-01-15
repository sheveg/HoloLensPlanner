Shader "UI/Overlay/Text" {
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
	_Color("Text Color", Color) = (1,1,1,1)
	}

		SubShader{
		Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off 
		Cull Off 
		ZWrite Off
		ZTest Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
		Color[_Color]
		SetTexture[_MainTex]{
		combine primary, texture * primary
	}
	}
	}
}