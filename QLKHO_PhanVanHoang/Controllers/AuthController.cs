using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Repositories;
using System.Linq;
using QLKHO_PhanVanHoang.Helpers;

namespace QLKHO_PhanVanHoang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var userList = await _unitOfWork.SystemUsers.FindAsync(u => u.Username == loginDto.Username);
            var user = userList.FirstOrDefault();
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(ApiResponse<object>.FailureResult("Sai tài khoản hoặc mật khẩu"));
            }

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
            var roleName = role?.Name ?? "Employee";

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyStr = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyStr)) throw new Exception("Jwt:Key is missing in configuration.");
            
            var key = Encoding.ASCII.GetBytes(keyStr);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, roleName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"] ?? "120")),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var response = new LoginResponseDto 
            { 
                 Token = tokenHandler.WriteToken(token),
                 FullName = user.FullName,
                 Role = roleName
            };
            
            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(response, "Đăng nhập thành công"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            var existingUsers = await _unitOfWork.SystemUsers.FindAsync(u => u.Username == registerDto.Username);
            if (existingUsers.Any())
            {
                return BadRequest(ApiResponse<object>.FailureResult("Tên đăng nhập đã tồn tại"));
            }

            int roleId = registerDto.RoleId;
            if (roleId <= 0)
            {
                var employeeRole = (await _unitOfWork.Roles.FindAsync(r => r.Name == "Employee")).FirstOrDefault();
                if (employeeRole != null)
                {
                    roleId = employeeRole.Id;
                }
                else
                {
                    var newRole = new QLKHO_PhanVanHoang.Models.Role { Name = "Employee", Description = "Nhân viên kho" };
                    await _unitOfWork.Roles.AddAsync(newRole);
                    await _unitOfWork.CompleteAsync(); 
                    roleId = newRole.Id;
                }
            }

            var newUser = new QLKHO_PhanVanHoang.Models.SystemUser
            {
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password), 
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                RoleId = roleId,
                IsActive = true
            };

            await _unitOfWork.SystemUsers.AddAsync(newUser);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(new { UserId = newUser.Id }, "Đăng ký thành công! Bạn có thể dùng tài khoản này để Login."));
        }
    }
}
