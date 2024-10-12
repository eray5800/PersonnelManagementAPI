namespace DAL.Models
{
    public class Base
    {
        public bool IsActive { get; set; } = true;

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

    }
}
