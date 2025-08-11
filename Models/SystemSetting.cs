using System.ComponentModel.DataAnnotations;

namespace WebUseASP_test_.Models
{
    public class SystemSetting
    {
        [Key] // Khóa chính
        public int SettingID { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
    }
}
