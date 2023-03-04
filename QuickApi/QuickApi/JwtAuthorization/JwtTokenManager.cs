using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace QuickApi.JwtAuthorization;

public class JwtTokenManager
{
    private JwtBearerOptions _jwtBearerOptions { get; set; }
    private JwtOptions _jwtOptions { get; set; }
    private SigningCredentials _signingCredentials { get; set; }

    public JwtTokenManager(IOptions<JwtBearerOptions> jwtBearerOptions,
        IOptions<JwtOptions> jwtOptions,
        SigningCredentials signingCredentials)
    {
        _jwtBearerOptions = jwtBearerOptions.Value;
        _jwtOptions = jwtOptions.Value;
        _signingCredentials = signingCredentials;
    }

    public string CreateToken(List<Claim> claims)
    {
        if(!claims.Exists(it=>it.Type==JwtClaimTypes.Role))
            Debug.WriteLine($"你应该在jwt的payload中包含JwtClaimTypes.Role属性");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            // 必须 Utc，默认30分钟
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
            SigningCredentials = _signingCredentials,
            EncryptingCredentials = new EncryptingCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SymmetricSecurityKeyString)),
                JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes128CbcHmacSha256)
        };

        var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                      ?? new JwtSecurityTokenHandler();
        var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
        var token = handler.WriteToken(securityToken);
        return token;
    }

    /// <summary>
    /// 解析Token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public List<Claim>? ResolveToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenSaml = jsonToken as JwtSecurityToken;
            var claims = tokenSaml.Claims.ToList();
            return claims;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    // resolve token 
    
}