using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

// HistoryPage 是包裹历史记录页面
// 主要功能：显示当前登录用户添加过的所有包裹记录
// 新增功能：鼠标移到包裹卡片上时显示 Collected / Clear 操作按钮
public partial class HistoryPage : ContentPage
{
    // SQLite 数据库对象，用于读取、更新和删除当前用户的包裹历史
    private readonly ParcelDatabase _parcelDatabase;

    // 当前登录用户的用户名
    // 用于确保历史记录只显示当前用户自己的包裹
    private readonly string _username;

    // ObservableCollection 用于绑定到 CollectionView
    // 当集合内容变化时，页面会自动更新显示
    public ObservableCollection<ParcelHistoryItem> ParcelHistory { get; set; } = new();

    // HistoryPage 构造函数
    // parcelDatabase：数据库对象
    // username：当前登录用户
    public HistoryPage(ParcelDatabase parcelDatabase, string username)
    {
        InitializeComponent();

        _parcelDatabase = parcelDatabase;
        _username = username;

        // 将 ParcelHistory 集合绑定到页面中的 HistoryCollectionView
        HistoryCollectionView.ItemsSource = ParcelHistory;
    }

    // 页面每次显示时都会执行
    // 例如从其他页面返回 HistoryPage 时，会重新刷新历史记录
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 设置页面顶部欢迎文字
        HistoryWelcomeLabel.Text = $"Parcel history for {_username}";

        // 初始化数据库，确保数据库表已经创建
        await _parcelDatabase.InitAsync();

        // 加载当前用户的包裹历史记录
        await LoadHistoryAsync();
    }

    // 加载包裹历史记录的方法
    private async Task LoadHistoryAsync()
    {
        // 先清空旧数据，避免重复显示
        ParcelHistory.Clear();

        // 从数据库获取当前用户的所有包裹
        var parcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        // 让最新添加的包裹显示在最上面
        var orderedParcels = parcels
            .OrderByDescending(p => p.Id)
            .ToList();

        // 把数据库中的 Parcel 数据转换成页面显示用的 ParcelHistoryItem
        foreach (var parcel in orderedParcels)
        {
            ParcelHistory.Add(new ParcelHistoryItem
            {
                // 保存原始 Parcel 对象
                // 后面点击 Collected 或 Clear 时，需要用它更新数据库
                OriginalParcel = parcel,

                // 显示包裹编号
                ParcelCode = $"Parcel Code: {parcel.ParcelCode}",

                // 显示包裹状态
                Status = parcel.Status ?? "",

                // 根据包裹状态设置不同颜色
                StatusColor = GetStatusColor(parcel.Status),

                // 显示取件位置
                LocationDisplay = $"Location: {parcel.Location}",

                // 显示取件截止日期
                PickupDeadlineDisplay = $"Deadline: {parcel.PickupDeadline}",

                // 显示取件码
                CollectionCodeDisplay = $"Collection Code: {parcel.CollectionCode}",

                // 默认不显示右侧操作按钮
                ShowActions = false
            });
        }

        // 如果没有历史记录，就显示空状态提示
        EmptyStateFrame.IsVisible = ParcelHistory.Count == 0;

        // 如果有历史记录，就显示历史记录列表
        HistoryCollectionView.IsVisible = ParcelHistory.Count > 0;
    }

    // 鼠标移到包裹卡片上时执行
    // 作用：显示右侧操作按钮 Collected / Clear
    private void OnParcelPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Border border && border.BindingContext is ParcelHistoryItem item)
        {
            item.ShowActions = true;
        }
    }

    // 鼠标离开包裹卡片时执行
    // 作用：隐藏右侧操作按钮
    private void OnParcelPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Border border && border.BindingContext is ParcelHistoryItem item)
        {
            item.ShowActions = false;
        }
    }

    // 当用户点击 Collected 按钮时执行
    // 作用：把该包裹状态更新为 Collected，并保存到 SQLite 数据库
    private async void OnCollectedClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not ParcelHistoryItem item)
            return;

        if (item.OriginalParcel == null)
        {
            await DisplayAlertAsync("Error", "Unable to find this parcel record.", "OK");
            return;
        }

        try
        {
            // 更新原始数据库对象的状态
            item.OriginalParcel.Status = "Collected";

            // 保存状态变化到 SQLite
            await _parcelDatabase.UpdateParcelAsync(item.OriginalParcel);

            // 页面显示模型也同步更新
            item.Status = "Collected";
            item.StatusColor = GetStatusColor("Collected");
            item.ShowActions = false;

            await DisplayAlertAsync("Success", "Parcel marked as collected.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to update parcel: {ex.Message}", "OK");
        }
    }

    // 当用户点击 Clear 按钮时执行
    // 作用：从 SQLite 数据库删除该包裹记录，并从页面中移除
    private async void OnClearClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not ParcelHistoryItem item)
            return;

        if (item.OriginalParcel == null)
        {
            await DisplayAlertAsync("Error", "Unable to find this parcel record.", "OK");
            return;
        }

        bool confirm = await DisplayAlertAsync(
            "Confirm Clear",
            $"Do you want to clear {item.ParcelCode} from history?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            // 从 SQLite 删除该包裹
            await _parcelDatabase.DeleteParcelAsync(item.OriginalParcel);

            // 从页面集合中移除该记录
            ParcelHistory.Remove(item);

            // 如果删除后没有记录，显示 empty state
            EmptyStateFrame.IsVisible = ParcelHistory.Count == 0;
            HistoryCollectionView.IsVisible = ParcelHistory.Count > 0;

            await DisplayAlertAsync("Success", "Parcel record cleared.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to clear parcel: {ex.Message}", "OK");
        }
    }

    // 根据包裹状态返回对应颜色
    // 这个颜色通常用于状态标签或状态文字
    private string GetStatusColor(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return "#6B7280";

        return status.Trim().ToLower() switch
        {
            // Pending 使用偏黄色，表示等待处理
            "pending" => "#A16207",

            // Ready / Ready for Collection 使用蓝紫色，表示可以领取
            "ready" => "#4F46E5",
            "ready for collection" => "#4F46E5",

            // Collected 使用绿色，表示已经领取
            "collected" => "#15803D",

            // 其他未知状态使用灰色
            _ => "#6B7280"
        };
    }

    // 当用户点击返回按钮时执行
    private async void OnBackClicked(object sender, EventArgs e)
    {
        // 返回上一页，例如 SearchPage / Dashboard 页面
        // SearchPage 如果写了 OnAppearing 刷新统计，返回后 Pending / Ready / Collected 会自动更新
        await Navigation.PopAsync();
    }
}

// ParcelHistoryItem 是历史记录页面专门用来显示的数据模型
// 它不是数据库表，只是为了让 CollectionView 更方便显示内容
public class ParcelHistoryItem : INotifyPropertyChanged
{
    // 保存原始 Parcel 数据库对象
    // 点击 Collected 或 Clear 时，需要用它更新 SQLite
    public Parcel? OriginalParcel { get; set; }

    private string _parcelCode = "";
    public string ParcelCode
    {
        get => _parcelCode;
        set
        {
            if (_parcelCode != value)
            {
                _parcelCode = value;
                OnPropertyChanged();
            }
        }
    }

    private string _status = "";
    public string Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnPropertyChanged();
            }
        }
    }

    private string _statusColor = "#6B7280";
    public string StatusColor
    {
        get => _statusColor;
        set
        {
            if (_statusColor != value)
            {
                _statusColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LocationDisplay { get; set; } = "";

    public string PickupDeadlineDisplay { get; set; } = "";

    public string CollectionCodeDisplay { get; set; } = "";

    private bool _showActions;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}