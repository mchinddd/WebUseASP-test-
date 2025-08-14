using System.Data;

namespace WebUseASP_test_.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int RoleID { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public Role Role { get; set; }

        // Thêm 2 navigation properties
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
    }
}
