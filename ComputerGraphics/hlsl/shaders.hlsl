/*struct my_struct
{
   float4 position : SV_POSITION;
   float4 v : POS;
   
};*/
/*struct VS_OUT
{
   float4 pos : SV_POSITION;
   float4 col : COLOR;
};
struct PS_IN
{
   float4 pos : SV_POSITION;
   float4 col : COLOR;
};*/

cbuffer viewProjCBuf : register(b0)
{
   row_major matrix transform;
   row_major matrix world;
};

cbuffer lightCBuf : register(b1)
{
   float3 ambient_color;
   float3 diffuse_color;
   float3 specular_color;
   float3 light_position;
   float3 direction;
   row_major matrix light_matrix;
}

struct VS_IN
{
   float4 pos : POSITION;
   float3 normal	: NORMAL0;
   float2 tex : TEXCOORD0;
};

struct PS_IN
{
   float4 pos : SV_POSITION;
   float4 shadowPos : POSITION;
   float3 normal	: NORMAL0;
   float2 tex : TEXCOORD0;
};

PS_IN VSmain(VS_IN input)
{
   PS_IN output = (PS_IN)0;
   output.pos = mul(input.pos, transform);
   output.shadowPos = mul(input.pos, world);
   output.normal = mul(float4(input.normal,0), world);
   output.tex = input.tex.xy;
   return output;
}

Texture2D		Picture : register(t0);
SamplerState	Sampler : register(s0);

Texture2D		depthTexture: register(t1);
SamplerComparisonState    samplerShadow : register(s1);



float CalcShadowFactor(float4 position)
{
    float4 shadowPos = mul(position, light_matrix);
    shadowPos /= shadowPos.w;
    
    float2 shadowUV = 0.5 * shadowPos.xy + float2(0.5, 0.5);
    shadowUV.y = 1 - shadowUV.y;
    float shadowDepth = shadowPos.z;

    float shadowFactor = depthTexture.SampleCmpLevelZero(samplerShadow, shadowUV, shadowDepth).r;
    return shadowFactor;
}

float4 PSmain( PS_IN input ) : SV_Target
{
   float4 color = Picture.Sample(Sampler, input.tex);

   float3 N = normalize(input.normal);
   float3 L = normalize(-direction);
   float3 R = reflect(L, N);

   float4 ambientColor = float4(ambient_color, 0.0f);
   float4 ambient = color * ambientColor;

   float diffuse = max(0.0f, dot(N, L));
   float4 diffuseColor = color * diffuse;

   float3 V = light_position;
   float shininess = 0.1f;
   float4 SpecularColor = float4(specular_color, 0.0f);
   float specular = pow(max(0.0f, dot(V, R)), shininess);
   float4 specularColor = SpecularColor * specular ;

   color = ambient + diffuseColor + specularColor;
   
   float shadowFactor = CalcShadowFactor(input.shadowPos);
   color = color * shadowFactor;
   return color;
}

/*float4 PSmain( PS_IN input ) : SV_Target
{
   //return float4(input.pos.x/2.0+0.5, input.pos.y/2.0+0.5, input.pos.z/2.0+0.5, 1.0);
   return input.col;
}*/
/*float4 PSmain(VS_OUT input) : SV_TARGET
{
   //return float4(position.x/2.0+0.5, position.y/2.0+0.5, position.z/2.0+0.5, 1.0);
   /*if(input.col.w>1)
   {
      return float4(1,0,0,1);
   } else
   {
      return float4(0,255,0,1);
   }#1#
   //return  input.col;
   return float4(1,1,1,1);
   return float4(input.col.y, input.col.z, input.col.w, input.col.x);
}*/

//float CalculateShadowFactor(float4 position)
//{
//    float4 shadowPosition = mul(position, light_matrix);
//    shadowPosition /= shadowPosition.w;
//
//    float2 shadowTexCoord = shadowPosition.xy * 0.5 + 0.5;
//
//    float shadowDepth = depthTexture.Sample(samplerShadow, shadowTexCoord).r;
//    float pixelDepth = shadowPosition.z;
//
//    return (pixelDepth > shadowDepth) ? 0.0f : 1.0f;
//}