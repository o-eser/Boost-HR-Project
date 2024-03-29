﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using HR_Project.Application.IoC.Models.DTOs;
using HR_Project.Application.Services.FileService;
using HR_Project.Application.Services.PersonelServices;
using HR_Project.Common;
using HR_Project.Common.Models.DTOs;
using HR_Project.Domain.Entities.Concrete;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HR_Project.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IPersonnelService _personnelService;
		private readonly IConfiguration _configuration;
		private readonly UserManager<Personnel> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IProfileImageService _profileImageService;
		private readonly RoleManager<IdentityRole<Guid>> _roleManager;

		public AccountController(IPersonnelService personnelService, IConfiguration configuration, UserManager<Personnel> userManager, IHttpContextAccessor httpContextAccessor, IProfileImageService profileImageService, RoleManager<IdentityRole<Guid>> roleManager)
		{
			_personnelService = personnelService;
			_configuration = configuration;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
			_profileImageService = profileImageService;
			_roleManager = roleManager;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDTO model)
		{
			//await _personnelService.Register(new RegisterDTO
			//{
			//	Name = "admin",
			//	Surname = "admin",
			//	Title = "admin",
			//	Email = "admin",
			//	Address = "asdasdasdasd",
			//	PhoneNumber = "admin",
			//	UserName = "admin1",
			//	Password = "admin123",
			//	DepartmentId = 1,
			//	CompanyId = 2,
			//	HireDate = DateTime.Now,
			//	CityId = 1,
			//	RegionId = 1,
			//	BloodType = Domain.Enum.BloodType.BRhNegative,
			//	Gender = Domain.Enum.Gender.Male,
			//	Nation = Domain.Enum.Nation.TR,
			//	BirthDate = DateTime.Now,
			//});




			await CreateRoles();

			var user = await _personnelService.Login(model);

			if (user.Succeeded)
			{
				Personnel personnel = await _userManager.FindByEmailAsync(model.Email);
				var isManager=await _personnelService.IsManager(personnel.Id);
                if (!personnel.IsAccountConfirmed)
					return Unauthorized("Kullanıcı hesabı onaylanmamıştır.");
				//Admine Rol atamak için
				//await _userManager.AddToRoleAsync(personnel, "Admin");
				//await _userManager.AddToRoleAsync(personnel, "CompanyManager");
				//await _userManager.AddToRoleAsync(personnel, "Personnel");

				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, model.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(ClaimTypes.NameIdentifier, personnel.Id.ToString()),
					new Claim(ClaimTypes.Name, personnel.Name),
					new Claim(ClaimTypes.Surname, personnel.Surname),
					new Claim(ClaimTypes.Thumbprint, await _profileImageService.GetFileById(personnel.Id.ToString())),
					new Claim(ClaimTypes.WindowsSubAuthority , personnel.CompanyId.ToString()),
					new Claim(ClaimTypes.UserData, isManager?"Manager":"Not" )
                    //new Claim("Company",personnel.CompanyId.ToString()),
                    //new Claim("Department",personnel.DepartmentId.ToString()),
                };


				var userRoles = await _personnelService.GetRoles(model.Email);

				if (userRoles.Length == 0)
					return Unauthorized("Kullanıcının Rolü mevcut değil");
				
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

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
		{
            Personnel personnel = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value);


            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, personnel.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, personnel.Id.ToString()),
                    new Claim(ClaimTypes.Name, personnel.Name),
                    new Claim(ClaimTypes.Surname, personnel.Surname),
                    new Claim(ClaimTypes.Thumbprint, await _profileImageService.GetFileById(personnel.Id.ToString()))
                   
                };


            var userRoles = await _personnelService.GetRoles(personnel.Email);

            if (userRoles.Length == 0)
                return Unauthorized("Kullanıcının Rolü mevcut değil");
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


		[HttpPost("logout")]
		public IActionResult Logout()
		{
			// Kullanıcının token'ını iptal etme işlemi
			var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			_personnelService.Logout(accessToken);

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


		[HttpPost("Confirm")]
		public async Task<IActionResult> Confirm(MailConfirmDTO model)
		{
			Personnel personnel = await _userManager.FindByIdAsync(model.Id);
			var result = await _userManager.ConfirmEmailAsync(personnel, model.Token);

			return Ok(result.Succeeded);
		}


		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register()
		{


			try
			{
				var form = await Request.ReadFormAsync();
				var jsonString = form["model"]; // JSON verisi

				// JSON verisini RegisterDTO'ya çözme
				var registerModel = JsonConvert.DeserializeObject<RegisterDTO>(jsonString);

				// IFormFile'ı almak için
				var file = form.Files.GetFile("UploadImage");

				registerModel.UploadImage = file;

				IdentityResult result = await _personnelService.Register(registerModel);
				if (result.Succeeded)
				{
					Personnel personnel = await _userManager.FindByEmailAsync(registerModel.Email);
					var token = await _userManager.GenerateEmailConfirmationTokenAsync(personnel);



					return Ok(new { id = personnel.Id, token });
					//await _emailService.SendEmailRegisterAsync(personnel.Email, subject, body);
				}

				return BadRequest(result);

			}
			catch (Exception ex)
			{

				throw ex;
			}


		}

		private async Task CreateRoles()
		{
			string[] roleNames = { "Admin", "CompanyManager", "Personnel" };

			IdentityResult roleResult;

			foreach (var roleName in roleNames)
			{
				var roleExist = await _roleManager.RoleExistsAsync(roleName);

				if (!roleExist)
				{
					roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
				}
			}
		}

	}
}
