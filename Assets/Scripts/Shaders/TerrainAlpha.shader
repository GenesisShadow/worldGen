


Shader "Noice/TerrainAlpha"
{
    Properties {
		[MaterialSlider] _shine("shiny", Range(0.0, 1.0)) = 0
		[MaterialSlider] _alpha("alpha", Range(0.0, 1.0)) = 1
		[MaterialSlider] _metal("metalic", Range(0.0, 1.0)) = 0
    }
	

    //idek what any of this is...
    SubShader {
        Tags {"RenderType" = "Transparent" "IgnoreProjector" = "true" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert alpha
        #pragma target 3.0
        #include "UnityCG.cginc"
		uniform float _shine;
		uniform float _alpha;
		uniform float _metal;
		

        struct Input {
            float4 vertexColor; // Vertex color stored here by vert() method
            half3 worldNormal : TEXCOORD0;
        };

        float GlobalLight;

        void vert (inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float y =  (IN.worldNormal.y+1)/4 + 0.5f;
            y = y <= 0.001 ? 0.05f : y;

            float x = (IN.worldNormal.x + 1) / 4 + 0.5f;
            x = x <= 0.001 ? 0.05f : x;

            float z = (IN.worldNormal.z + 1) / 4 + 0.5f;
            z = z <= 0.001 ? 0.05f : z;
			
			//float4 colour = float4(In.vertexColor.r)

            o.Albedo = IN.vertexColor * y * x * z; //set as vertex colour by light
			o.Smoothness = _shine;
			o.Alpha = _alpha;
			o.Metallic = _metal;

			/*gl_FragColor = vec4(c.r > c.g && c.r > c.b ? 1.0 : 0.0,
			c.g > c.r && c.g > c.b ? 1.0 : 0.0,
			c.b > c.r && c.b > c.g ? 1.0 : 0.0,
			1.0 );*/
        }
        ENDCG
    }
    FallBack "Diffuse"
}
