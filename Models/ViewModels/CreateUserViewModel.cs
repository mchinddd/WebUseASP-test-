using System.ComponentModel.DataAnnotations;

namespace WebUseASP_test_.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        [MinLength(6, ErrorMessage = "Password phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&^#]).{6,}$",
            ErrorMessage = "Password phải có ít nhất 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FullName không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|edu\.id\.vn)$",
            ErrorMessage = "Email phải có định dạng @gmail.com hoặc @edu.id.vn")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role không được để trống")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 số")]
        public string Phone { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
