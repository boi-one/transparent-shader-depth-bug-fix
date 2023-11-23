Shader "Custom/transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(0.0,1.0)) = 0.25
        /*_HDR ("HDR", float4) = (0,0,0,0)
        _Emission ("Emission", float) = 0*/
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusDstAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag

        fixed4 _Color;
        float _Transparency;
        /*float _Emission;*/

        struct vertexInput
        {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            fixed4 color : COLOR;
        };

        struct fragmentInput
        {
            float4 pos : SV_POSITION;
            float4 color : COLOR0;
        };

        fragmentInput vert(vertexInput i)
        {
            fragmentInput o;
            o.pos = UnityObjectToClipPos(i.vertex);
            //o.color = _Color;

            //o.color = i.texcoord;
            o.color = float4(1,1,1, _Transparency);
            return o;
        }
        
        float4 frag(fragmentInput i) : COLOR
        {
            return i.color;
        }
        
        
        ENDCG
        }
    }
    FallBack "Diffuse"
}
