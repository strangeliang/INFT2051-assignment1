using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;

    public RegisterPage(ParcelDatabase parcelDatabase)
    {
        InitializeComponent();
        _parcelDatabase = parcelDatabase;
    }

    private async void OnRegisterSubmitClicked(object sender, EventArgs e)
    {
        string username = NewUsernameEntry.Text?.Trim() ?? "";
        string password = NewPasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Warning", "Please enter username and password.", "OK");
            return;
        }

        var existingUser = await _parcelDatabase.GetUserByUsernameAsync(username);

        if (existingUser != null)
        {
            await DisplayAlert("Warning", "This username already exists.", "OK");
            return;
        }

        var user = new User
        {
            Username = username,
            Password = password
        };

        await _parcelDatabase.SaveUserAsync(user);

        await DisplayAlert("Success", "Registration successful. Please log in.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}