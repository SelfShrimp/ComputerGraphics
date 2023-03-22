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

cbuffer cBuf
{
   row_major matrix transform;
   float4 lightDirection;
   //float3 ambientColor;
   float3 diffuseColor;
   //float3 specularColor;
   //float shininess;
};

struct VS_IN
{
   float4 pos : POSITION;
   float3 normal	: NORMAL0;
   float2 tex : TEXCOORD0;
   float4 dif : DIF;
};

struct PS_IN
{
   float4 pos : SV_POSITION;
   float3 normal	: NORMAL0;
   float2 tex : TEXCOORD0;
   float4 dif : DIF  ;
};

PS_IN VSmain(VS_IN input)
{
   PS_IN output = (PS_IN)0;
   output.pos = mul(input.pos, transform);
   output.normal = mul(float4(input.normal,0), transform);
   output.tex = input.tex.xy;
   output.dif = input.dif;
   return output;
}

Texture2D		Picture		: register(t0);
SamplerState	Sampler			: register(s0);

float4 PSmain( PS_IN input ) : SV_Target
{
   float4 color = Picture.Sample(Sampler, input.tex);
   float3 N = normalize(input.normal);
   float3 L = normalize(float3(1.0f, 10.0f, 5.0f));
   float3 R = reflect(L, N);
   float4 diffuseColor = input.dif;
   float4 diffuse = saturate(dot(N, L)) * diffuseColor;
   color *= diffuse;

   float3 ambientColor = float3(0.4f,0.4f,0.4f);
   color *= float4(ambientColor, 1.0f);

   float3 V = normalize(-input.pos.xyz);
   float shininess = 0.4f;
   float3 specularColor = float3(0.4f,0.4f,0.4f);
   float specular = pow(saturate(dot(R, V)), shininess);
   color += float4(specularColor, 1.0f) * specular;
   
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