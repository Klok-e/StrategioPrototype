
#ifndef CombineInflTexture_float_INCLUDED
#define CombineInflTexture_float_INCLUDED

void CombineInflTexture_float(float Influence,
                            float Interp,
                            float4 BgTexture,
                            float4 Side1Col,
                            float4 Side2Col,
                            float InflGrad,
                            float4 SeparatorColor,
                            out float4 FinalColor)
{
    float4 sideColor;
    /*if(InflGrad>0.05)
        sideColor = SeparatorColor;
    else*/ if(Influence > 0.5)
        sideColor = Side1Col;
    else
        sideColor = Side2Col;
    // now mix tex and col
    FinalColor = lerp(sideColor,BgTexture,Interp);
}

#endif