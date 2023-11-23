Shader "Custom/light"
{
    Properties
    {
       _MainTex ("Main Texture", 2D) = "white"
    }
    SubShader
    {
        Tags{"Queue" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
        
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;
        
        fixed4 _Color;
        float _Transparency;
        /*float _Emission;*/

        struct vertexInput
        {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
        };

        struct fragmentInput
        {
            float4 pos : SV_POSITION;
            half2 uv : TEXTCOORD0;
        };

        fragmentInput vert(vertexInput i)
        {
            fragmentInput o;
            o.pos = UnityObjectToClipPos(i.vertex);
            o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);
            return o;
        }
        
        half4 frag(fragmentInput i) : COLOR
        {
            return tex2D( _MainTex, i.uv);
        }
        
        
        ENDCG
        }
    }
    FallBack "Diffuse"
}
