using DAL.Enums;
using System.Text.Json.Serialization;

namespace PersonnelManagementAPI.DTO
{
    public class SaveLeaveRequestDTO
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]

        public LeaveType LeaveType { get; set; }

        public string Reason { get; set; }
    }
}
