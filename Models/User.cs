using System.ComponentModel.DataAnnotations;
using System.Data;

namespace WebUseASP_test_.Models
{
    public class User
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "Tài khoản là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Phone { get; set; }
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public int RoleID { get; set; }
        
        public bool IsActive { get; set; }

        public Role Role { get; set; }

        public DateTime CreatedDate { get; set; }

        // Thêm 2 navigation properties
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
    }
}
