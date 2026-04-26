using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

// RegisterPage handles new user account creation.
public partial class RegisterPage : ContentPage
{
    // Database object used to check and save user accounts.
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

        // Validate that both registration fields have been completed.
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync("Warning", "Please enter username and password.", "OK");
            return;
        }

        // Check whether the username has already been registered.
        var existingUser = await _parcelDatabase.GetUserByUsernameAsync(username);

        if (existingUser != null)
        {
            await DisplayAlertAsync("Warning", "This username already exists.", "OK");
            return;
        }

        // Create and save the new user account in the local database.
        var user = new User
        {
            Username = username,
            Password = password
        };

        await _parcelDatabase.SaveUserAsync(user);

        await DisplayAlertAsync("Success", "Registration successful. Please log in.", "OK");

        // Return to the login page after successful registration.
        await Navigation.PopAsync();
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}