// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_BasicKjh_Ver01_Add"
{
	Properties
	{
		_Emission("Emission", Float) = 1
		_MatinTX("Matin TX", 2D) = "white" {}
		_MainPow("Main Pow", Float) = 1
		[Toggle(_SUBPOLAR_ON)] _SubPolar("----Sub Polar ?------------------------------------------------------------", Float) = 0
		_SubTX("Sub TX", 2D) = "white" {}
		_SubMinimumBright("Sub Minimum Bright", Float) = 0.05
		_SubCeilStrength("Sub Ceil Strength", Range( 0.05 , 20)) = 3
		_SubSpeedXYPolarTilingWZ("Sub Speed=XY / PolarTiling=WZ", Vector) = (0,0,1,1)
		[Toggle(_DISTPOLAR_ON)] _DistPolar("----Dist Polar ?------------------------------------------------------------", Float) = 0
		_DistTX("Dist TX", 2D) = "white" {}
		_DistPivotRelocation("Dist Pivot Relocation", Float) = -0.5
		_DistSpeedXYPolarTilingWZ("Dist Speed=XY / PolarTiling=WZ", Vector) = (0,0,1,1)
		[Toggle(_STEPPOLAR_ON)] _StepPolar("----Step Polar ?------------------------------------------------------------", Float) = 0
		[Toggle(_STEPMASKSHARING_ON)] _StepMaskSharing("Step Mask Sharing ?", Float) = 0
		_StepTX("Step TX", 2D) = "white" {}
		_StepSpeedXYPolarTilingWZ("Step Speed=XY / PolarTiling=WZ", Vector) = (0,0,1,1)
		[Toggle(_MASKPOLAR_ON)] _MaskPolar("----Mask Polar ?------------------------------------------------------------", Float) = 0
		_MaskTX("Mask TX", 2D) = "white" {}
		_MaskPolarXTiling("Mask Polar X Tiling", Float) = 1
		_MaskPolarYTiling("Mask Polar Y Tiling", Float) = 1
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One , One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.5
		#pragma shader_feature_local _DISTPOLAR_ON
		#pragma shader_feature_local _SUBPOLAR_ON
		#pragma shader_feature_local _STEPMASKSHARING_ON
		#pragma shader_feature_local _STEPPOLAR_ON
		#pragma shader_feature_local _MASKPOLAR_ON
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
			float4 uv4_tex4coord4;
		};

		uniform sampler2D _MatinTX;
		uniform float4 _MatinTX_ST;
		uniform sampler2D _DistTX;
		uniform float4 _DistSpeedXYPolarTilingWZ;
		uniform float4 _DistTX_ST;
		uniform float _DistPivotRelocation;
		uniform float _MainPow;
		uniform sampler2D _SubTX;
		uniform float4 _SubSpeedXYPolarTilingWZ;
		uniform float4 _SubTX_ST;
		uniform float _SubMinimumBright;
		uniform float _SubCeilStrength;
		uniform sampler2D _StepTX;
		uniform float4 _StepSpeedXYPolarTilingWZ;
		uniform float4 _StepTX_ST;
		uniform sampler2D _MaskTX;
		uniform float4 _MaskTX_ST;
		uniform float _MaskPolarXTiling;
		uniform float _MaskPolarYTiling;
		uniform float _Emission;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MatinTX = i.uv_texcoord * _MatinTX_ST.xy + _MatinTX_ST.zw;
			float2 appendResult167 = (float2(i.uv2_tex4coord2.x , i.uv2_tex4coord2.y));
			float2 appendResult169 = (float2(i.uv2_tex4coord2.z , i.uv2_tex4coord2.w));
			float2 appendResult203 = (float2(_DistSpeedXYPolarTilingWZ.x , _DistSpeedXYPolarTilingWZ.y));
			float2 uv_DistTX = i.uv_texcoord * _DistTX_ST.xy + _DistTX_ST.zw;
			float2 CenteredUV15_g1 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g1 = CenteredUV15_g1;
			float2 appendResult23_g1 = (float2(( length( CenteredUV15_g1 ) * 1.0 * 2.0 ) , ( atan2( break17_g1.x , break17_g1.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
			float2 temp_output_195_0 = appendResult23_g1;
			float2 appendResult198 = (float2(_DistSpeedXYPolarTilingWZ.z , _DistSpeedXYPolarTilingWZ.w));
			#ifdef _DISTPOLAR_ON
				float2 staticSwitch202 = ( temp_output_195_0 * appendResult198 );
			#else
				float2 staticSwitch202 = uv_DistTX;
			#endif
			float2 panner204 = ( 1.0 * _Time.y * appendResult203 + staticSwitch202);
			float4 tex2DNode206 = tex2D( _DistTX, panner204 );
			float4 temp_cast_0 = (_MainPow).xxxx;
			float2 appendResult180 = (float2(_SubSpeedXYPolarTilingWZ.x , _SubSpeedXYPolarTilingWZ.y));
			float2 uv_SubTX = i.uv_texcoord * _SubTX_ST.xy + _SubTX_ST.zw;
			float2 appendResult176 = (float2(_SubSpeedXYPolarTilingWZ.z , _SubSpeedXYPolarTilingWZ.w));
			#ifdef _SUBPOLAR_ON
				float2 staticSwitch179 = ( temp_output_195_0 * appendResult176 );
			#else
				float2 staticSwitch179 = uv_SubTX;
			#endif
			float2 panner181 = ( 1.0 * _Time.y * appendResult180 + staticSwitch179);
			float clampResult186 = clamp( ( tex2D( _SubTX, panner181 ).r + _SubMinimumBright ) , 0.0 , 1.0 );
			float2 appendResult222 = (float2(_StepSpeedXYPolarTilingWZ.x , _StepSpeedXYPolarTilingWZ.y));
			float2 uv_StepTX = i.uv_texcoord * _StepTX_ST.xy + _StepTX_ST.zw;
			float2 appendResult219 = (float2(_StepSpeedXYPolarTilingWZ.z , _StepSpeedXYPolarTilingWZ.w));
			#ifdef _STEPPOLAR_ON
				float2 staticSwitch223 = ( temp_output_195_0 * appendResult219 );
			#else
				float2 staticSwitch223 = uv_StepTX;
			#endif
			float2 panner224 = ( 1.0 * _Time.y * appendResult222 + staticSwitch223);
			float4 tex2DNode225 = tex2D( _StepTX, panner224 );
			float2 uv_MaskTX = i.uv_texcoord * _MaskTX_ST.xy + _MaskTX_ST.zw;
			float2 appendResult238 = (float2(i.uv4_tex4coord4.x , i.uv4_tex4coord4.w));
			float2 appendResult235 = (float2(_MaskPolarXTiling , _MaskPolarYTiling));
			float2 appendResult237 = (float2(i.uv4_tex4coord4.x , i.uv4_tex4coord4.w));
			#ifdef _MASKPOLAR_ON
				float2 staticSwitch242 = ( ( temp_output_195_0 * appendResult235 ) + appendResult237 );
			#else
				float2 staticSwitch242 = ( uv_MaskTX + appendResult238 );
			#endif
			float4 tex2DNode243 = tex2D( _MaskTX, staticSwitch242 );
			#ifdef _STEPMASKSHARING_ON
				float staticSwitch227 = ( tex2DNode225.r * tex2DNode243.r );
			#else
				float staticSwitch227 = tex2DNode225.r;
			#endif
			float4 temp_output_246_0 = ( ( ( pow( tex2D( _MatinTX, ( ( ( uv_MatinTX * appendResult167 ) + appendResult169 ) + ( ( tex2DNode206.r * i.uv4_tex4coord4.y ) + ( i.uv4_tex4coord4.y * _DistPivotRelocation ) ) ) ) , temp_cast_0 ) * ( ceil( ( clampResult186 * _SubCeilStrength ) ) / _SubCeilStrength ) ) * step( i.uv4_tex4coord4.z , staticSwitch227 ) ) * tex2DNode243.r );
			o.Emission = ( ( ( i.vertexColor * temp_output_246_0 ) * ( i.vertexColor.a * temp_output_246_0 ) ) * _Emission ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18707
0;0;2048;1091;6909.579;1846.248;2.895173;True;False
Node;AmplifyShaderEditor.CommentaryNode;214;-5745.764,-487.3827;Inherit;False;2885.378;598.621;dist;13;197;198;200;203;207;209;211;201;202;204;206;210;245;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;193;-4954.005,208.6381;Inherit;False;2964.44;624.609;Sub;16;175;176;177;178;179;180;181;183;184;185;186;187;188;189;190;191;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;175;-4904.005,624.2471;Inherit;False;Property;_SubSpeedXYPolarTilingWZ;Sub Speed=XY / PolarTiling=WZ;8;0;Create;True;0;0;False;0;False;0,0,1,1;0,0,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;197;-5674.964,-213.4554;Inherit;False;Property;_DistSpeedXYPolarTilingWZ;Dist Speed=XY / PolarTiling=WZ;12;0;Create;True;0;0;False;0;False;0,0,1,1;0,0,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;176;-4569.687,569.805;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;198;-5310.14,-22.56159;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;195;-5523.503,675.8881;Inherit;False;Polar Coordinates;-1;;1;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;-5043.096,-96.7215;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-4346.68,473.878;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;233;-4018.916,1717.78;Inherit;False;2383.893;789.9116;Mask;12;240;237;236;235;234;244;243;242;241;239;238;249;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;232;-4367.394,1100.726;Inherit;False;2671.087;514.7212;Step;12;225;226;227;228;231;218;219;220;221;222;223;224;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;201;-5560.839,-418.3816;Inherit;False;0;206;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;178;-4773.534,382.7561;Inherit;False;0;183;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;218;-4317.394,1406.448;Inherit;False;Property;_StepSpeedXYPolarTilingWZ;Step Speed=XY / PolarTiling=WZ;16;0;Create;True;0;0;False;0;False;0,0,1,1;0,0,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;179;-4145.175,377.434;Inherit;False;Property;_SubPolar;----Sub Polar ?------------------------------------------------------------;4;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;249;-3908.375,2356.112;Inherit;False;Property;_MaskPolarYTiling;Mask Polar Y Tiling;20;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;202;-4949.43,-414.834;Inherit;False;Property;_DistPolar;----Dist Polar ?------------------------------------------------------------;9;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;203;-4784.571,-190.8395;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;234;-3905.283,2273.518;Inherit;False;Property;_MaskPolarXTiling;Mask Polar X Tiling;19;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;180;-4109.762,622.773;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;204;-4622.279,-336.6511;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;165;-3883.387,-682.3821;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;181;-3843.841,381.422;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;215;-5258.201,1348.896;Inherit;False;3;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;235;-3634.235,2311.187;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;219;-3986.968,1344.3;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;237;-3253.11,2388.706;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;239;-3636.093,1843.365;Inherit;False;0;243;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;220;-4253.771,1175.207;Inherit;False;0;225;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;238;-3403.104,1976.744;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;166;-4100.815,-818.05;Inherit;False;0;173;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;207;-4233.69,-141.9713;Inherit;False;Property;_DistPivotRelocation;Dist Pivot Relocation;11;0;Create;True;0;0;False;0;False;-0.5;-0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;-3823.735,1295.826;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;167;-3607.676,-716.3942;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;183;-3601.334,355.0731;Inherit;True;Property;_SubTX;Sub TX;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;206;-4394.227,-365.0289;Inherit;True;Property;_DistTX;Dist TX;10;0;Create;True;0;0;False;0;False;-1;None;6746a119be6efd04c89045dac5c428cf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;236;-3387.104,2130.301;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;184;-3476.732,572.312;Inherit;False;Property;_SubMinimumBright;Sub Minimum Bright;6;0;Create;True;0;0;False;0;False;0.05;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;241;-3163.103,1864.744;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;169;-3427.074,-626.8521;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-3239.361,383.8201;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;240;-3079.356,2252.154;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-3416.451,-813.5244;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-3627.779,-396.295;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;222;-3660.509,1428.298;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-3696.181,-160.1383;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;223;-3693.972,1193.186;Inherit;False;Property;_StepPolar;----Step Polar ?------------------------------------------------------------;13;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;242;-2782.444,1853.03;Inherit;False;Property;_MaskPolar;----Mask Polar ?------------------------------------------------------------;17;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-3134.122,577.462;Inherit;False;Property;_SubCeilStrength;Sub Ceil Strength;7;0;Create;True;0;0;False;0;False;3;3;0.05;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-3200.943,-713.3593;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;211;-3354.938,-281.375;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;224;-3394.585,1186.948;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;186;-3064.561,382.8201;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;149;-3001.507,-715.6154;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;225;-3147.236,1337.624;Inherit;True;Property;_StepTX;Step TX;15;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;243;-2451.844,1829.763;Inherit;True;Property;_MaskTX;Mask TX;18;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-2874.716,379.3871;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;173;-2626.628,-744.6044;Inherit;True;Property;_MatinTX;Matin TX;2;0;Create;True;0;0;False;0;False;-1;None;6cd65d156de14f742a864b226c66b9f6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CeilOpNode;189;-2699.317,381.787;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-2769.853,1445.373;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-2502.169,-505.1408;Inherit;False;Property;_MainPow;Main Pow;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;191;-2528.129,384.576;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;227;-2585.466,1359.062;Inherit;False;Property;_StepMaskSharing;Step Mask Sharing ?;14;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;152;-2275.894,-596.1865;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;231;-1848.706,1150.726;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-1802.291,101.4814;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-1258.061,197.1546;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;155;-1282.12,-93.23182;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-1073.437,296.7506;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-786.3271,340.5675;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-839.1199,47.76819;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;248;-578.3271,160.5675;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-445.3177,347.0468;Inherit;False;Property;_Emission;Emission;1;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-2544.694,258.6382;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;245;-3898.646,-268.202;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-340.1199,145.7682;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;244;-2144.416,1767.78;Inherit;False;Constant;_Float4;Float 4;16;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-2547.99,1204.139;Inherit;False;Constant;_Float6;Float 6;18;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;198.2154,150.6541;Float;False;True;-1;3;ASEMaterialInspector;0;0;Unlit;SH_BasicKjh_Ver01_Add;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;176;0;175;3
WireConnection;176;1;175;4
WireConnection;198;0;197;3
WireConnection;198;1;197;4
WireConnection;200;0;195;0
WireConnection;200;1;198;0
WireConnection;177;0;195;0
WireConnection;177;1;176;0
WireConnection;179;1;178;0
WireConnection;179;0;177;0
WireConnection;202;1;201;0
WireConnection;202;0;200;0
WireConnection;203;0;197;1
WireConnection;203;1;197;2
WireConnection;180;0;175;1
WireConnection;180;1;175;2
WireConnection;204;0;202;0
WireConnection;204;2;203;0
WireConnection;181;0;179;0
WireConnection;181;2;180;0
WireConnection;235;0;234;0
WireConnection;235;1;249;0
WireConnection;219;0;218;3
WireConnection;219;1;218;4
WireConnection;237;0;215;1
WireConnection;237;1;215;4
WireConnection;238;0;215;1
WireConnection;238;1;215;4
WireConnection;221;0;195;0
WireConnection;221;1;219;0
WireConnection;167;0;165;1
WireConnection;167;1;165;2
WireConnection;183;1;181;0
WireConnection;206;1;204;0
WireConnection;236;0;195;0
WireConnection;236;1;235;0
WireConnection;241;0;239;0
WireConnection;241;1;238;0
WireConnection;169;0;165;3
WireConnection;169;1;165;4
WireConnection;185;0;183;1
WireConnection;185;1;184;0
WireConnection;240;0;236;0
WireConnection;240;1;237;0
WireConnection;168;0;166;0
WireConnection;168;1;167;0
WireConnection;210;0;206;1
WireConnection;210;1;215;2
WireConnection;222;0;218;1
WireConnection;222;1;218;2
WireConnection;209;0;215;2
WireConnection;209;1;207;0
WireConnection;223;1;220;0
WireConnection;223;0;221;0
WireConnection;242;1;241;0
WireConnection;242;0;240;0
WireConnection;170;0;168;0
WireConnection;170;1;169;0
WireConnection;211;0;210;0
WireConnection;211;1;209;0
WireConnection;224;0;223;0
WireConnection;224;2;222;0
WireConnection;186;0;185;0
WireConnection;149;0;170;0
WireConnection;149;1;211;0
WireConnection;225;1;224;0
WireConnection;243;1;242;0
WireConnection;188;0;186;0
WireConnection;188;1;187;0
WireConnection;173;1;149;0
WireConnection;189;0;188;0
WireConnection;226;0;225;1
WireConnection;226;1;243;1
WireConnection;191;0;189;0
WireConnection;191;1;187;0
WireConnection;227;1;225;1
WireConnection;227;0;226;0
WireConnection;152;0;173;0
WireConnection;152;1;174;0
WireConnection;231;0;215;3
WireConnection;231;1;227;0
WireConnection;153;0;152;0
WireConnection;153;1;191;0
WireConnection;157;0;153;0
WireConnection;157;1;231;0
WireConnection;246;0;157;0
WireConnection;246;1;243;1
WireConnection;247;0;155;4
WireConnection;247;1;246;0
WireConnection;161;0;155;0
WireConnection;161;1;246;0
WireConnection;248;0;161;0
WireConnection;248;1;247;0
WireConnection;245;0;206;1
WireConnection;245;1;207;0
WireConnection;163;0;248;0
WireConnection;163;1;162;0
WireConnection;0;2;163;0
ASEEND*/
//CHKSM=8C783B65ABA693D5DB7E65F0C21A05AB228C976A