using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

public partial class MainPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;

    public MainPage(ParcelDatabase parcelDatabase)
    {
        InitializeComponent();
        _parcelDatabase = parcelDatabase;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _parcelDatabase.InitAsync();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync("Warning", "Please enter username and password.", "OK");
            return;
        }

        var parcel = new Parcel
        {
            ParcelCode = "P12345",
            Status = "Ready for Pickup",
            Location = "Locker A1",
            PickupDeadline = "2026-04-10"
        };

        await _parcelDatabase.SaveParcelAsync(parcel);

        await DisplayAlertAsync("Success", "Login successful and parcel saved to database.", "OK");

        await Navigation.PushAsync(new SearchPage());
    }
}