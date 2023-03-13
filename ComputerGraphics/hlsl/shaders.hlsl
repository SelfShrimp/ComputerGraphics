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
   matrix transform;
};

struct VS_IN
{
   float4 pos : POSITION;
   float3 normal	: NORMAL0;
   float2 tex : TEXCOORD0;
};

struct PS_IN
{
   float4 pos : SV_POSITION;
   float3 normal	: NORMAL0;
   float2 tex : TEXCOORD0;
};

PS_IN VSmain(VS_IN input)
{
   PS_IN output = (PS_IN)0;
   output.pos = mul(transform, input.pos);
   output.normal = input.normal;
   output.tex = input.tex.xy;
   
   return output;
}

Texture2D		Picture		: register(t0);
SamplerState	Sampler			: register(s0);

float4 PSmain( PS_IN input ) : SV_Target
{
   float4 color = Picture.Sample(Sampler, input.tex);
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