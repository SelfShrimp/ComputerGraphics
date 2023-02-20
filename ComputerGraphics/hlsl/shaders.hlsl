float4 VSmain(float4 position : POSITION) : SV_POSITION
{
   return position;
}

float4 PSmain(float4 position : SV_POSITION) : SV_TARGET
{
   return float4(1.0, 1.0, 1.0, 1.0);
}