float4 VSmain(float4 position : POSITION) : SV_POSITION
{
   return position;
}

cbuffer cBuf 
{
	matrix transform
}

float4 PSmain(float4 position : SV_POSITION) : SV_TARGET
{
   return mul(float4(1.0, 1.0, 1.0, 1.0), transform);
}