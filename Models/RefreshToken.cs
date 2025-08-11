using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUseASP_test_.Models
{
    public class RefreshToken
    {
        [Key] // Khóa chính
        public int TokenID { get; set; }

        [ForeignKey("User")] // Khóa ngoại tới Users
        public int UserID { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }

        public User User { get; set; }
    }
}
