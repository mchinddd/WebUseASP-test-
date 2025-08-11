using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUseASP_test_.Models
{
    public class Authentication
    {
        [Key]  // Đây là khóa chính
        [ForeignKey("User")] // Liên kết tới bảng Users qua UserID
        public int UserID { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }

        public User User { get; set; }
    }
}
