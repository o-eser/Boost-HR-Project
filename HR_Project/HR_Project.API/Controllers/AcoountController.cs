﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR_Project.Application.Services.PersonelServices;
using HR_Project.Common.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HR_Project.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AcoountController : ControllerBase
	{
		private readonly IPersonelService _personelService;
		private readonly IConfiguration _configuration;


		public AcoountController(IPersonelService personelService, IConfiguration configuration)
		{
			_personelService = personelService;
			_configuration = configuration;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDTO model)
		{
			var user = await _personelService.Login(model);

			if (user.Succeeded)
			{
				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, model.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				};

				var userRoles = await _personelService.GetRoles(model.Email);

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var token = GetToken(authClaims);

				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expiration = token.ValidTo
				});
			}
			else
			{
				return Unauthorized();
			}
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{
			// Kullanıcının token'ını iptal etme işlemi
			var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			_personelService.Logout(accessToken);

			return Ok(new { message = "Logout successful" });
		}

		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:secretKey"]));
			var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				_configuration["JwtSettings:validIssuer"],
				_configuration["JwtSettings:validAudience"],
				authClaims,
				expires: DateTime.UtcNow.AddMinutes(15),
				signingCredentials: signIn);

			return token;
		}
	}
}