#pragma surface surf Journey fullforwardshadows
float4 LightingJourney (SurfaceOutput s, fixed3 viewDir, UnityGI gi)
{
    float3 diffuseColor = DiffuseColor    ();
    float3 rimColor     = RimLighting     ();
    float3 oceanColor   = OceanSpecular   ();
    float3 glitterColor = GlitterSpecular ();
    float3 specularColor = saturate(max(rimColor, oceanColor));
    float3 color = diffuseColor + specularColor + glitterColor;
    
    return float4(color * s.Albedo, 1);
}