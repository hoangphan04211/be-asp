namespace QLKHO_PhanVanHoang.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new();
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new();
        public bool Require2FA { get; set; } = false;
        public string? TwoFactorType { get; set; }
        public string? PreAuthToken { get; set; }
        public int SessionId { get; set; }
        public int UserId { get; set; }
    }

    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int RoleId { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string ResetCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminResetPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> PermissionCodes { get; set; } = new();
    }

    public class UpdateRolePermissionsDto
    {
        public List<string> PermissionCodes { get; set; } = new();
    }

    public class Verify2FaRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PreAuthToken { get; set; } = string.Empty;
    }

    public class Setup2FaAppResponseDto
    {
        public string SecretKey { get; set; } = string.Empty;
        public string QrCodeUri { get; set; } = string.Empty;
    }

    public class Verify2FaSetupRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }

    public class Disable2FaRequestDto
    {
        public string Code { get; set; } = string.Empty;
    }

    public class TwoFactorStatusDto
    {
        public bool Enabled { get; set; }
        public string? Type { get; set; }
        public string? Email { get; set; }
        public bool HasAppSecret { get; set; }
    }
}
