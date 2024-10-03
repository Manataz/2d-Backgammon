Shader "Unlit/VolumeTransparentForAnim"
{
  Properties
  {
    _Volume("Volume", 3D) = "" {}
    _Scale ("Scale (x, y, z, and w=mul)", vector) = (1,1,1,1)
	_Offset ("Offset (x, y, z, and w=div)", vector) = (0,0,0,1)
  }

  CGINCLUDE
  #include "UnityCG.cginc"

  struct appdata
  {
    half4 vertex : POSITION;
  };

  struct v2f
  {
    half4 vertex : SV_POSITION;
    half3 uv : TEXCOORD0;
  };

  sampler3D _Volume;
  half4 _Scale;
  half4 _Offset;

  v2f vert(appdata v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    half4 sclVec = v.vertex;
    sclVec.xyz /= _Scale.xyz*_Scale.w;
    half4 ofs = _Offset+half4(0,0,(_Offset.w-1)/2+1,0);
    o.uv = sclVec.xyz + (ofs.xyz / ofs.w) + 0.5;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target
  {
    return tex3D(_Volume, i.uv);
  }
  ENDCG

  SubShader
  {
    Tags 
    { 
      "RenderType" = "Transparent" //"Opaque" // 
	  "Queue" = "Transparent" // "Geometry" // 
    }

    Pass
    {
	  Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
	  ZWrite Off
      Cull Back
      Offset 0,0
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
    }
  }
}