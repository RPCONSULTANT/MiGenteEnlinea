namespace MiGenteEnLinea.Infrastructure.Options;

public sealed class AuthLinksOptions
{
    public const string SectionName = "AuthLinks";

    public string PublicWebBaseUrl { get; set; } = "https://plattaformv2.migenteenlinea.do";
}
