using SQLite;

namespace parcel_station1.Models
{
    public class Parcel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // 这个包裹属于哪个用户
        public string Username { get; set; } = "";

        public string ParcelCode { get; set; } = "";
        public string Status { get; set; } = "";
        public string Location { get; set; } = "";
        public string CollectionCode { get; set; } = "";
        public string PickupDeadline { get; set; } = "";
    }
}