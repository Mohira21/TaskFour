using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskFour.Models;
using Newtonsoft.Json;

namespace TaskFour.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly string _filePath;
    public UsersController(IOptions<JsonFileOption> options)
    {
        _filePath = options.Value.FilePath ?? "file.json";
    }
    [HttpGet]
    public IActionResult SignIn(string token )
    {
        return Ok(User.FindFirst(ClaimTypes.Name)?.Value);
    }
    [AllowAnonymous]
    [HttpPost]
    public IActionResult SignUp(User user)
    {
        var keyByte = System.Text.Encoding.UTF8.GetBytes("jkljshjdbjnmsnmk");
        var securityKey = new SigningCredentials(new SymmetricSecurityKey(keyByte),SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("Password", user.Password!),
                new Claim(ClaimTypes.Role, user.Role!)
            };
        var security = new JwtSecurityToken(
            issuer: "Project",
            audience: "Room",
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: securityKey);

        var identity = new ClaimsIdentity(claims);

        var principal = new ClaimsPrincipal(identity);


        var token = new JwtSecurityTokenHandler().WriteToken(security);

        var users = ReadUsers(principal);
        users ??= new List<User>();

        users.Add(user);

        SaveUsers(users);

        return Ok(token);
    }
    private void SaveUsers(List<User> users)
    {
        var jsonData = JsonConvert.SerializeObject(users);
        System.IO.File.WriteAllText(_filePath, jsonData);
    }
    private List<User>? ReadUsers(ClaimsPrincipal principal)
    {
        if(!System.IO.File.Exists(_filePath))
        {
            return null;
        }
        var jsonData = System.IO.File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<User>>(jsonData);
    }
}
