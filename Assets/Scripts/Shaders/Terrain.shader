// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'




Shader "Noice/Terrain"
{
    Properties {
		[MaterialSlider] _shine("shiny", Range(0.0, 1.0)) = 0
		//[MaterialSlider] _alpha("alpha", Range(0.0, 1.0)) = 1
		[MaterialSlider] _metal("metalic", Range(0.0, 1.0)) = 0
		[MaterialSlider] _dark("darkness", Range(1.0, 30.0)) = 1
        [MaterialToggle] _shading("shading", Float) = 0
        [MaterialSlider] _strength("strength", Range(1.0, 99.0)) = 0
    }
	

    //idek what any of this is...
    SubShader {
        Tags {"RenderType" = "Opaque"}
        LOD 200

        //UsePass "OutlineToolkit/Outline/OUTLINE"

        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #pragma target 3.0
        #include "UnityCG.cginc"
		uniform float _shine;
		uniform float _dark;
		uniform float _metal;
        uniform float _shading;
        uniform float _strength;

        struct Input {
            float4 vertexColor; // Vertex color stored here by vert() method
            half3 worldNormal : TEXCOORD0;
            float3 worldPos;
        };

        float GlobalLight;

        void vert (inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float camDist = distance(IN.worldPos , _WorldSpaceCameraPos.xyz);
            float d = pow(2,camDist/_strength);
            if(camDist <= 1) camDist = 1;

            UNITY_CALC_FOG_FACTOR_RAW(camDist);

            if(_shading) {
                float y = (IN.worldNormal.y * _strength / 100) % 1 + 1;
                float x = (IN.worldNormal.x * _strength / 100) % 1 + 1;
                float z = (IN.worldNormal.z * _strength / 100) % 1 + 1;

                o.Albedo = IN.vertexColor/d * x * y * z; //set as vertex colour by light
                //o.Albedo = x*y*z;
            }else
                o.Albedo = IN.vertexColor/d;
                //o.Albedo = lerp(IN.vertexColor/_dark, _FogColor, l);
			o.Smoothness = _shine;
			o.Metallic = _metal;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
