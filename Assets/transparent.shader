// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Flat Color"{
        Properties //interface in unity
        { 
                _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
            
        }
        SubShader //de shader (kan meerdere hebben per shader file voor verschillende platformen)
        { 
            Tags
            {
                "RenderType" = "Tranparent"
                "Queue" = "Transparent"
            }
            
            Pass //effect
            {
                CGPROGRAM //CG is een shader taal van nvidea
                
                #pragma vertex vertexFunc
                #pragma fragment fragmentFunc
        
                uniform float4 _Color;
        
                struct vertexInput
                {
                    float4 vertex : POSITION;
                };
        
                struct vertexOutput
                {
                    float4 pos : SV_POSITION; //sv_position is directx11
                };
        
                vertexOutput vertexFunc(vertexInput vi)
                {
                    vertexOutput vo;
                    vo.pos = UnityObjectToClipPos(vi.vertex); //vertex position + unity matrix to unity
                    
                    return vo;
                }
        
                float4 fragmentFunc(vertexOutput vo) : COLOR
                {
                    return _Color;
                }
                
                ENDCG
            }
        }
    Fallback "Diffuse" //als de shader niet werkt zal de shader dit gebruiken
    }