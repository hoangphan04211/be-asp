using FluentValidation;
using QLKHO_PhanVanHoang.DTOs;

namespace QLKHO_PhanVanHoang.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.SkuCode)
                .NotEmpty().WithMessage("Mã hàng hóa (SKU) không được để trống")
                .MaximumLength(50).WithMessage("Mã hàng hóa không được quá 50 ký tự");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(200).WithMessage("Tên sản phẩm không được quá 200 ký tự");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Danh mục sản phẩm không hợp lệ");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Đơn vị tính không được để trống");

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá vốn không được âm");

            RuleFor(x => x.SellingPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá bán không được âm");
        }
    }

    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống");
        }
    }

    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                .MinimumLength(4).WithMessage("Tên đăng nhập phải có ít nhất 4 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ");
        }
    }

    public class CreateReceivingVoucherDtoValidator : AbstractValidator<CreateReceivingVoucherDto>
    {
        public CreateReceivingVoucherDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Mã phiếu nhập không được để trống");

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("Kho nhận không hợp lệ");

            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("Phiếu nhập phải có ít nhất một mặt hàng");

            RuleForEach(x => x.Details).SetValidator(new CreateReceivingVoucherDetailDtoValidator());
        }
    }

    public class CreateReceivingVoucherDetailDtoValidator : AbstractValidator<CreateReceivingVoucherDetailDto>
    {
        public CreateReceivingVoucherDetailDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Sản phẩm không hợp lệ");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng nhập phải lớn hơn 0");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Đơn giá không được âm");
        }
    }
}
