using FarmaciaBelen.Models;
using Microsoft.IdentityModel.Tokens; // viene con System.IdentityModel.Tokens.Jwt
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

[RoutePrefix("api/auth")]
public class AuthController : ApiController
{
    // DEMO: cambia esta validación por tu consulta a BD (Usuarios)
    private bool ValidateUser(string username, string password, out int userId, out string role)
    {
        // Ejemplo duro: dina.admin / Solola/2025
        if (string.Equals(username, "dina.admin", StringComparison.OrdinalIgnoreCase)
            && password == "Solola/2025")
        {
            userId = 1;
            role = "admin";
            return true;
        }

        userId = 0;
        role = "user";
        return false;
    }

    [HttpPost]
    [Route("login")]
    public IHttpActionResult Login([FromBody] LoginDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Debe enviar username y password.");

        if (!ValidateUser(dto.Username, dto.Password, out var userId, out var role))
            return Unauthorized(); // 401

        var key = ConfigurationManager.AppSettings["JwtKey"];
        var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        var audience = ConfigurationManager.AppSettings["JwtAudience"];
        var minsStr = ConfigurationManager.AppSettings["JwtExpiresMinutes"] ?? "60";
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(minsStr));

        var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("name", dto.Username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var seconds = (int)(expires - DateTime.UtcNow).TotalSeconds;

        return Ok(new LoginResponse { Token = tokenString, ExpiresIn = seconds });
    }
}
