Shader "Unlit/Circle"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _CircleSize("CircleSize", Float) = 0.4
        _CircleThickness("CircleThickness", Float) = 0.1
        _Alpha("Alpha", Float) = 1
        //_CircleScale("CirlceScale", Float) = 

    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}

        Blend One OneMinusSrcAlpha        
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float _CircleSize;
            float _CircleThickness;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed r = distance(i.uv, fixed2(0.5, 0.5));
                fixed c = step(_CircleSize, r) * _Alpha;
                fixed4 c_final = lerp(c, fixed4(0.0, 0.0, 0.0, 0), step(_CircleSize + _CircleThickness, r)) * _BaseColor;
                return c_final;
            }
            ENDCG
        }
    }
}
