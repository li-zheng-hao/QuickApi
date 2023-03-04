using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace QuickApi.JwtAuthorization;

public class JwtOptions
{
    public const string Name = "Jwt";
    public readonly static Encoding DefaultEncoding = Encoding.UTF8;
    public readonly static double DefaultExpiresMinutes = 30d;

    public string Audience { get; set; } = "QuickApi";

    public string Issuer { get; set; } = "QuickApi";

    public double ExpiresMinutes { get; set; } = DefaultExpiresMinutes;

    public Encoding Encoding { get; set; } = DefaultEncoding;

    public string SymmetricSecurityKeyString { get; set; } = "samplekeyyoushoudchangeit_samplekeyyoushoudchangeit_samplekeyyoushoudchangeit_samplekeyyoushoudchangeit_samplekeyyoushoudchangeit";

    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.GetBytes(SymmetricSecurityKeyString));
}