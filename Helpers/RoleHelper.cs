namespace WebUseASP_test_.Helpers
{
    public static class RoleHelper
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";

        // Convert từ RoleID (int/string) sang RoleName
        public static string GetRoleName(string roleId)
        {
            return roleId switch
            {
                "1" => Admin,
                "2" => Teacher,
                "3" => Student,
                _ => Student // mặc định
            };
        }

        // Convert RoleName sang ID (nếu cần dùng)
        public static int GetRoleId(string roleName)
        {
            return roleName switch
            {
                Admin => 1,
                Teacher => 2,
                Student => 3,
                _ => 3
            };
        }
    }
}
