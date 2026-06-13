using QLKHO_PhanVanHoang.Constants;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Repositories;
using System.Linq;
using QLKHO_PhanVanHoang.Helpers;
using System.Security.Cryptography;
using QLKHO_PhanVanHoang.Services;
using OtpNet;

namespace QLKHO_PhanVanHoang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration config, IEmailService emailService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            try 
            {
                var userList = await _unitOfWork.SystemUsers.FindAsync(u => u.Username == loginDto.Username, "Role.Permissions");
                var user = userList.FirstOrDefault();
                
                bool isPasswordValid = false;
                try 
                {
                    isPasswordValid = user != null && !string.IsNullOrEmpty(user.PasswordHash) && BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
                }
                catch { /* Invalid hash format */ }

                if (!isPasswordValid)
                {
                    return Unauthorized(ApiResponse<object>.FailureResult("Sai tÃ i khoáº£n hoáº·c máº­t kháº©u hoáº·c Ä‘á»‹nh dáº¡ng máº­t kháº©u khÃ´ng há»£p lá»‡"));
                }

                var roleName = user!.Role?.Name ?? "Employee";
                var permissions = user.Role?.Permissions?.Select(p => p.Code).ToList() ?? new List<string>();

                // Kiá»ƒm tra XÃ¡c thá»±c 2 lá»›p (2FA)
                if (user.TwoFactorEnabled)
                {
                    var preAuthToken = GeneratePreAuthToken(user);

                    if (user.TwoFactorType == "Email")
                    {
                        if (string.IsNullOrEmpty(user.Email))
                        {
                            return BadRequest(ApiResponse<object>.FailureResult("TÃ i khoáº£n Ä‘Ã£ báº­t 2FA qua Email nhÆ°ng chÆ°a cÃ³ Ä‘á»‹a chá»‰ Email!"));
                        }

                        // Sinh mÃ£ OTP 6 sá»‘ ngáº«u nhiÃªn
                        var emailOtp = new Random().Next(100000, 999999).ToString();
                        user.TwoFactorTempCode = emailOtp;
                        user.TwoFactorTempCodeExpiry = DateTime.UtcNow.AddMinutes(5); // Háº¿t háº¡n sau 5p

                        _unitOfWork.SystemUsers.Update(user);
                        await _unitOfWork.CompleteAsync();

                        // Gá»­i mÃ£ OTP qua Email
                        try
                        {
                            await _emailService.SendEmailAsync(user.Email, "MÃ£ xÃ¡c thá»±c 2 lá»›p (2FA) Ä‘Äƒng nháº­p WMS",
                                $"<h3>ChÃ o {user.FullName},</h3>" +
                                $"<p>MÃ£ xÃ¡c thá»±c Ä‘Äƒng nháº­p WMS cá»§a báº¡n lÃ : <b style='font-size: 20px; color: #1890ff;'>{emailOtp}</b></p>" +
                                $"<p>MÃ£ nÃ y cÃ³ hiá»‡u lá»±c trong 5 phÃºt. Vui lÃ²ng khÃ´ng chia sáº» mÃ£ nÃ y cho báº¥t ká»³ ai.</p>");
                        }
                        catch (Exception emailEx)
                        {
                            return StatusCode(500, ApiResponse<object>.FailureResult($"ÄÄƒng nháº­p Ä‘Ãºng máº­t kháº©u, nhÆ°ng há»‡ thá»‘ng khÃ´ng gá»­i Ä‘Æ°á»£c Email OTP: {emailEx.Message}"));
                        }
                    }

                    return Ok(ApiResponse<LoginResponseDto>.SuccessResult(new LoginResponseDto
                    {
                        Require2FA = true,
                        TwoFactorType = user.TwoFactorType,
                        PreAuthToken = preAuthToken,
                        FullName = user.FullName,
                        Role = roleName,
                        PermissionCodes = permissions
                    }, "Yêu cầu xác thực hai lớp (2FA) để hoàn tất đăng nhập."));
                }

                var refreshToken = GenerateRefreshToken();

                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var session = new Models.UserSession
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    DeviceName = string.IsNullOrEmpty(userAgent) ? "Unknown Device" : (userAgent.Length > 200 ? userAgent.Substring(0, 200) : userAgent),
                    IpAddress = ipAddress,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false
                };

                await _unitOfWork.UserSessions.AddAsync(session);
                await _unitOfWork.CompleteAsync();

                var accessToken = GenerateAccessToken(user, roleName, session.Id);

                return Ok(ApiResponse<LoginResponseDto>.SuccessResult(new LoginResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    FullName = user.FullName,
                    Role = roleName,
                    PermissionCodes = permissions,
                    SessionId = session.Id,
                    UserId = user.Id
                }, "Đăng nhập thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailureResult($"Lỗi hệ thống: {ex.Message} | Trace: {ex.StackTrace}"));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var principal = GetPrincipalFromExpiredToken(requestDto.AccessToken);
            if (principal == null) return BadRequest(ApiResponse<object>.FailureResult("Token không hợp lệ"));

            string username = principal.Identity?.Name ?? "";
            var user = (await _unitOfWork.SystemUsers.FindAsync(u => u.Username == username, "Role.Permissions")).FirstOrDefault();

            if (user == null)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Token không hợp lệ"));
            }

            var sessionList = await _unitOfWork.UserSessions.FindAsync(s => s.UserId == user.Id && s.RefreshToken == requestDto.RefreshToken && !s.IsRevoked);
            var session = sessionList.FirstOrDefault();

            if (session == null || session.ExpiresAt <= DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Refresh Token không hợp lệ hoặc đã hết hạn"));
            }

            var newRefreshToken = GenerateRefreshToken();

            session.RefreshToken = newRefreshToken;
            session.ExpiresAt = DateTime.UtcNow.AddDays(7);
            session.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            _unitOfWork.UserSessions.Update(session);
            await _unitOfWork.CompleteAsync();

            var roleName = user.Role?.Name ?? "Employee";
            var permissions = user.Role?.Permissions?.Select(p => p.Code).ToList() ?? new List<string>();
            var newAccessToken = GenerateAccessToken(user, roleName, session.Id);

            return Ok(ApiResponse<TokenResponseDto>.SuccessResult(new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                FullName = user.FullName,
                Role = roleName,
                PermissionCodes = permissions
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng."));

            // Xác thực mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return BadRequest(ApiResponse<object>.FailureResult("Mật khẩu cũ không chính xác."));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Đổi mật khẩu thành công."));
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaRequestDto dto)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(dto.PreAuthToken);
                if (principal == null) return BadRequest(ApiResponse<object>.FailureResult("Mã phiên đăng nhập không hợp lệ"));

                var isPreAuth = principal.FindFirst("PreAuth")?.Value == "true";
                if (!isPreAuth) return BadRequest(ApiResponse<object>.FailureResult("Phiên đăng nhập không hợp lệ"));

                string username = principal.Identity?.Name ?? "";
                var userList = await _unitOfWork.SystemUsers.FindAsync(u => u.Username == username, "Role.Permissions");
                var user = userList.FirstOrDefault();

                if (user == null || user.Username != dto.Username)
                {
                    return BadRequest(ApiResponse<object>.FailureResult("Không tìm thấy thông tin tài khoản hoặc phiên không khớp"));
                }

                bool isCodeValid = false;

                if (user.TwoFactorType == "Email")
                {
                    if (user.TwoFactorTempCode == dto.Code && user.TwoFactorTempCodeExpiry >= DateTime.UtcNow)
                    {
                        isCodeValid = true;
                        user.TwoFactorTempCode = null;
                        user.TwoFactorTempCodeExpiry = null;
                    }
                }
                else if (user.TwoFactorType == "App")
                {
                    if (!string.IsNullOrEmpty(user.TwoFactorSecret))
                    {
                        var secretBytes = Base32Encoding.ToBytes(user.TwoFactorSecret);
                        var totp = new Totp(secretBytes);
                        isCodeValid = totp.VerifyTotp(dto.Code, out long timeStepMatched, new VerificationWindow(1, 1));
                    }
                }

                if (!isCodeValid)
                {
                    return BadRequest(ApiResponse<object>.FailureResult("Mã xác thực 2FA không chính xác hoặc đã hết hạn"));
                }

                var roleName = user.Role?.Name ?? "Employee";
                var permissions = user.Role?.Permissions?.Select(p => p.Code).ToList() ?? new List<string>();

                var refreshToken = GenerateRefreshToken();

                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var session = new Models.UserSession
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    DeviceName = string.IsNullOrEmpty(userAgent) ? "Unknown Device" : (userAgent.Length > 200 ? userAgent.Substring(0, 200) : userAgent),
                    IpAddress = ipAddress,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false
                };

                await _unitOfWork.UserSessions.AddAsync(session);
                await _unitOfWork.CompleteAsync();

                var accessToken = GenerateAccessToken(user, roleName, session.Id);

                return Ok(ApiResponse<LoginResponseDto>.SuccessResult(new LoginResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    FullName = user.FullName,
                    Role = roleName,
                    PermissionCodes = permissions,
                    SessionId = session.Id,
                    UserId = user.Id
                }, "Xác thực hai lớp thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailureResult($"Lỗi hệ thống: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpGet("two-factor-status")]
        public async Task<IActionResult> GetTwoFactorStatus()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            return Ok(ApiResponse<TwoFactorStatusDto>.SuccessResult(new TwoFactorStatusDto
            {
                Enabled = user.TwoFactorEnabled,
                Type = user.TwoFactorType,
                Email = user.Email,
                HasAppSecret = !string.IsNullOrEmpty(user.TwoFactorSecret)
            }));
        }

        [Authorize]
        [HttpPost("enable-2fa-email")]
        public async Task<IActionResult> Enable2FaEmail()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(ApiResponse<object>.FailureResult("Tài khoản chưa cấu hình Email. Vui lòng cập nhật Email trước khi bật 2FA!"));
            }

            user.TwoFactorEnabled = true;
            user.TwoFactorType = "Email";

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Đã kích hoạt xác thực 2 lớp qua Email thành công"));
        }

        [Authorize]
        [HttpGet("setup-2fa-app")]
        public async Task<IActionResult> Setup2FaApp()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            var secretBytes = KeyGeneration.GenerateRandomKey(20);
            var secretBase32 = Base32Encoding.ToString(secretBytes);

            var issuer = "WMS-PhanHoang";
            var label = $"{issuer}:{user.Username}";
            var qrCodeUri = $"otpauth://totp/{Uri.EscapeDataString(label)}?secret={secretBase32}&issuer={Uri.EscapeDataString(issuer)}";

            return Ok(ApiResponse<Setup2FaAppResponseDto>.SuccessResult(new Setup2FaAppResponseDto
            {
                SecretKey = secretBase32,
                QrCodeUri = qrCodeUri
            }));
        }

        [Authorize]
        [HttpPost("verify-and-enable-2fa-app")]
        public async Task<IActionResult> VerifyAndEnable2FaApp([FromBody] Verify2FaSetupRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            if (string.IsNullOrEmpty(dto.SecretKey))
            {
                return BadRequest(ApiResponse<object>.FailureResult("Khóa bí mật không được để trống"));
            }

            var secretBytes = Base32Encoding.ToBytes(dto.SecretKey);
            var totp = new Totp(secretBytes);
            bool isValid = totp.VerifyTotp(dto.Code, out long timeStepMatched, new VerificationWindow(1, 1));

            if (!isValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Mã xác thực không chính xác. Vui lòng quét lại QR code hoặc thử lại!"));
            }

            user.TwoFactorEnabled = true;
            user.TwoFactorType = "App";
            user.TwoFactorSecret = dto.SecretKey;

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Đã kích hoạt xác thực 2 lớp qua ứng dụng Authenticator thành công"));
        }

        [Authorize]
        [HttpPost("send-disable-2fa-otp")]
        public async Task<IActionResult> SendDisable2FaEmailOtp()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            if (user.TwoFactorType != "Email")
            {
                return BadRequest(ApiResponse<object>.FailureResult("Phương thức 2FA của tài khoản không phải là Email."));
            }

            var code = new Random().Next(100000, 999999).ToString();
            user.TwoFactorTempCode = code;
            user.TwoFactorTempCodeExpiry = DateTime.UtcNow.AddMinutes(5);

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            await _emailService.SendEmailAsync(user.Email!, "Mã xác thực tắt 2FA WMS",
                $"<h3>Chào {user.FullName},</h3><p>Mã xác thực để tắt tính năng 2FA của bạn là: <b style='font-size: 20px; color: #1890ff;'>{code}</b></p><p>Mã có hiệu lực trong 5 phút.</p>");

            return Ok(ApiResponse<object>.SuccessResult(null, "Mã xác thực đã được gửi về Email của bạn."));
        }

        [Authorize]
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> Disable2Fa([FromBody] Disable2FaRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng"));

            if (!user.TwoFactorEnabled)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Tài khoản hiện chưa bật xác thực 2 lớp"));
            }

            bool isCodeValid = false;
            if (user.TwoFactorType == "Email")
            {
                if (user.TwoFactorTempCode == dto.Code && user.TwoFactorTempCodeExpiry >= DateTime.UtcNow)
                {
                    isCodeValid = true;
                    user.TwoFactorTempCode = null;
                    user.TwoFactorTempCodeExpiry = null;
                }
            }
            else if (user.TwoFactorType == "App")
            {
                if (!string.IsNullOrEmpty(user.TwoFactorSecret))
                {
                    var secretBytes = Base32Encoding.ToBytes(user.TwoFactorSecret);
                    var totp = new Totp(secretBytes);
                    isCodeValid = totp.VerifyTotp(dto.Code, out long timeStepMatched, new VerificationWindow(1, 1));
                }
            }

            if (!isCodeValid)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Mã xác thực 2FA không chính xác hoặc đã hết hạn"));
            }

            user.TwoFactorEnabled = false;
            user.TwoFactorType = null;
            user.TwoFactorSecret = null;

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Đã tắt tính năng xác thực 2 lớp thành công."));
        }

        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var sessions = await _unitOfWork.UserSessions.FindAsync(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow);
            
            var result = sessions.Select(s => new 
            {
                s.Id,
                s.DeviceName,
                s.IpAddress,
                s.CreatedAt,
                s.ExpiresAt,
                IsCurrentSession = false // Can be determined by matching the current token if needed
            }).OrderByDescending(s => s.CreatedAt).ToList();

            return Ok(ApiResponse<object>.SuccessResult(result));
        }

        [Authorize]
        [HttpPost("revoke-session/{sessionId}")]
        public async Task<IActionResult> RevokeSession(int sessionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var session = await _unitOfWork.UserSessions.GetByIdAsync(sessionId);
            
            if (session == null || session.UserId != userId)
            {
                return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiên đăng nhập"));
            }

            session.IsRevoked = true;
            _unitOfWork.UserSessions.Update(session);
            await _unitOfWork.CompleteAsync();

            await _notificationService.SendForceLogoutToSessionAsync(sessionId.ToString());

            return Ok(ApiResponse<object>.SuccessResult(null, "Đã đăng xuất thiết bị thành công"));
        }

        #region Helpers
        private string GenerateAccessToken(Models.SystemUser user, string roleName, int? sessionId = null)
        {
            var keyStr = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(keyStr!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleName)
            };

            if (sessionId.HasValue)
            {
                claims.Add(new Claim("SessionId", sessionId.Value.ToString()));
            }

            if (user.Role?.Permissions != null)
            {
                foreach (var permission in user.Role.Permissions)
                {
                    claims.Add(new Claim("Permission", permission.Code));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"] ?? "120")),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        private string GeneratePreAuthToken(Models.SystemUser user)
        {
            var keyStr = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(keyStr!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("PreAuth", "true")
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
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
                ValidateLifetime = false // Quan trá»ng: cho phÃ©p láº¥y info tá»« token Ä‘Ã£ háº¿t háº¡n
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


