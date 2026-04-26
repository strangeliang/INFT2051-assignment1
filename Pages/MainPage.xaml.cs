using Microsoft.Maui.Devices;
using parcel_station1.Data;
using parcel_station1.Services;

namespace parcel_station1.Pages;

// MainPage handles user login and navigation to registration.
public partial class MainPage : ContentPage
{
    // Database object used for user authentication.
    private readonly ParcelDatabase _parcelDatabase;

    // Prevents repeated login or navigation actions while a process is running.
    private bool _isBusy;

    public MainPage(ParcelDatabase parcelDatabase)
    {
        InitializeComponent();
        _parcelDatabase = parcelDatabase;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Ensure the required database tables are created before login.
            await _parcelDatabase.InitAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Database Error", $"Failed to initialize database: {ex.Message}", "OK");
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isBusy)
            return;

        try
        {
            _isBusy = true;

            string username = UsernameEntry.Text?.Trim() ?? string.Empty;
            string password = PasswordEntry.Text?.Trim() ?? string.Empty;

            // Validate that both login fields have been completed.
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlertAsync("Warning", "Please enter username and password.", "OK");
                return;
            }

            // Check the entered username and password against the local database.
            var user = await _parcelDatabase.GetUserAsync(username, password);

            if (user == null)
            {
                await DisplayAlertAsync("Login Failed", "Invalid username or password.", "OK");
                return;
            }

            // Provide feedback after a successful login.
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                // Vibration may not be supported on all devices.
            }

            BeepService.PlaySuccessBeep();

            await DisplayAlertAsync("Success", "Login successful.", "OK");

            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;

            // Navigate to the dashboard/search page with the current username.
            await Navigation.PushAsync(new SearchPage(_parcelDatabase, username));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Something went wrong during login: {ex.Message}", "OK");
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        if (_isBusy)
            return;

        try
        {
            await Navigation.PushAsync(new RegisterPage(_parcelDatabase));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Navigation Error", $"Unable to open register page: {ex.Message}", "OK");
        }
    }
}