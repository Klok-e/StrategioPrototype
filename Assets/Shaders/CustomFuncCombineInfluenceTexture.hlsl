
#ifndef CombineInflTexture_float_INCLUDED
#define CombineInflTexture_float_INCLUDED

void CombineInflTexture_float(float Influence,
                            float Interp,
                            float4 BgTexture,
                            float4 Side1Col,
                            float4 Side2Col,
                            float InflGrad,
                            float4 SeparatorColor,
                            float Margin,
                            float Gamma,
                            float PowerMult,
                            out float4 FinalColor)
{
    float4 sideColor;
    float power;
    if(Influence > 0.5+Margin){
        sideColor = Side1Col;
        power = (Influence-(0.5+Margin))/(0.5-Margin);
    }
    else if (Influence < 0.5-Margin){
        sideColor = Side2Col;
        power = 1. - Influence/(0.5-Margin);
    }
    else{
        sideColor = SeparatorColor;
        power = abs(Influence-0.5)/Margin;
    }
    // now mix tex and col
    FinalColor = lerp(BgTexture,sideColor,Interp+pow(abs(power*PowerMult),1./Gamma));
}

#endif