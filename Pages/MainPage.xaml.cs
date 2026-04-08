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
            await DisplayAlert("Warning", "Please enter username and password.", "OK");
            return;
        }

        var user = await _parcelDatabase.GetUserAsync(username, password);

        if (user == null)
        {
            await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
            return;
        }

        await DisplayAlert("Success", "Login successful.", "OK");
        await Navigation.PushAsync(new SearchPage(_parcelDatabase, username));
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_parcelDatabase));
    }
}