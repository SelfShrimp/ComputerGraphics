struct my_struct
{
   float4 position : SV_POSITION;
   float4 v : POS;
   
};

cbuffer cBuf
{
   matrix transform;
};

my_struct VSmain(float4 position : POSITION)
{
   my_struct res = (my_struct) 0;
   res.position = mul(transform, position);
   res.v = position;
   return res;
   //return position;
}

float4 PSmain(my_struct res) : SV_TARGET
{
   return float4(res.v.x/2.0+0.5, res.v.y/2.0+0.5, res.v.z/2.0+0.5, 1.0);
}