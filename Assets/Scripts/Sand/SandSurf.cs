void surf (Input IN, inout SurfaceOutput o)
{
    o.Albedo = _SandColor;
    o.Alpha = 1;
    float3 N = float3(0, 0, 1);
    N = RipplesNormal(N);
    N = SandNormal   (N);
    o.Normal = N;
}