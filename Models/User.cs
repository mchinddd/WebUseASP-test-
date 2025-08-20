using System.Data;
using System.ComponentModel.DataAnnotations;

namespace WebUseASP_test_.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        [MinLength(6, ErrorMessage = "Password phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Range(1, 3, ErrorMessage = "Vai trò không hợp lệ")]
        public int RoleID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }

        public Role Role { get; set; }

        // Cho phép null
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
