using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Repositories;
using System.Linq;
using QLKHO_PhanVanHoang.Helpers;
using System.Security.Cryptography;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration config, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _emailService = emailService;
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

            var accessToken = GenerateAccessToken(user, roleName);
            var refreshToken = GenerateRefreshToken();

            // Lưu Refresh Token vào User
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Hết hạn sau 7 ngày
            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(new LoginResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                Role = roleName
            }, "Đăng nhập thành công"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var principal = GetPrincipalFromExpiredToken(requestDto.AccessToken);
            if (principal == null) return BadRequest(ApiResponse<object>.FailureResult("Token không hợp lệ"));

            string username = principal.Identity?.Name ?? "";
            var user = (await _unitOfWork.SystemUsers.FindAsync(u => u.Username == username)).FirstOrDefault();

            if (user == null || user.RefreshToken != requestDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Refresh Token không hợp lệ hoặc đã hết hạn"));
            }

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
            var newAccessToken = GenerateAccessToken(user, role?.Name ?? "Employee");
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<TokenResponseDto>.SuccessResult(new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var user = (await _unitOfWork.SystemUsers.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();
            if (user == null) return BadRequest(ApiResponse<object>.FailureResult("Email không tồn tại trong hệ thống"));

            // Tạo mã 6 chữ số
            var resetCode = new Random().Next(100000, 999999).ToString();
            user.ResetPasswordCode = resetCode;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddMinutes(15); // Hết hạn sau 15p

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            // Gửi qua Email
            await _emailService.SendEmailAsync(user.Email!, "Mã khôi phục mật khẩu WMS", 
                $"<h3>Chào {user.FullName},</h3><p>Mã khôi phục mật khẩu của bạn là: <b>{resetCode}</b></p><p>Mã có hiệu lực trong 15 phút.</p>");

            return Ok(ApiResponse<object>.SuccessResult(null, "Mã khôi phục đã được gửi về Email của bạn."));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);

            return Ok(ApiResponse<object>.SuccessResult(new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Email,
                RoleName = role?.Name ?? "Employee"
            }));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            var user = (await _unitOfWork.SystemUsers.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();
            if (user == null || user.ResetPasswordCode != dto.ResetCode || user.ResetPasswordExpiry <= DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Mã xác nhận không đúng hoặc đã hết hạn"));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetPasswordCode = null; // Xóa mã sau khi dùng
            user.ResetPasswordExpiry = null;

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Mật khẩu đã được cập nhật thành công"));
        }

        #region Helpers
        private string GenerateAccessToken(Models.SystemUser user, string roleName)
        {
            var keyStr = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(keyStr!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, roleName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"] ?? "120")),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                ValidateLifetime = false // Quan trọng: cho phép lấy info từ token đã hết hạn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        #endregion
    }
}
