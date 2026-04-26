using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace parcel_station1.Models
{
    public class Parcel : INotifyPropertyChanged
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

        // ShowActions 只用于控制 UI 显示按钮
        // 不需要保存进 SQLite 数据库，所以加 [Ignore]
        private bool _showActions;

        [Ignore]
        public bool ShowActions
        {
            get => _showActions;
            set
            {
                if (_showActions != value)
                {
                    _showActions = value;
                    OnPropertyChanged();
                }
            }
        }

        // 当 ShowActions 改变时，通知 XAML UI 刷新
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}