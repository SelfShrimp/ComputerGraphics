/*struct my_struct
{
   float4 position : SV_POSITION;
   float4 v : POS;
   
};*/

cbuffer cBuf
{
   matrix transform;
};

struct VS_OUT
{
   float4 pos : SV_POSITION;
   float4 col : COLOR;
};
struct PS_IN
{
   float4 pos : SV_POSITION;
   float4 col : COLOR;
};

VS_OUT VSmain(float4 position : POSITION, float4 color : COLOR)
{
   VS_OUT output = (VS_OUT)0;
   output.pos = mul(transform, position);
   output.col = color;
   //res.v = position;
   return output;
   //return position;
}

/*float4 PSmain( PS_IN input ) : SV_Target
{
   //return float4(input.pos.x/2.0+0.5, input.pos.y/2.0+0.5, input.pos.z/2.0+0.5, 1.0);
   return input.col;
}*/
float4 PSmain(VS_OUT input) : SV_TARGET
{
   //return float4(position.x/2.0+0.5, position.y/2.0+0.5, position.z/2.0+0.5, 1.0);
   /*if(input.col.w>1)
   {
      return float4(1,0,0,1);
   } else
   {
      return float4(0,255,0,1);
   }*/
   //return  input.col;
   return float4(input.col.y, input.col.z, input.col.w, input.col.x);
}