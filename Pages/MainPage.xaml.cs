using Microsoft.Maui.Devices;
using parcel_station1.Data;
using parcel_station1.Services;

namespace parcel_station1.Pages;

public partial class MainPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;
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

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlertAsync("Warning", "Please enter username and password.", "OK");
                return;
            }

            var user = await _parcelDatabase.GetUserAsync(username, password);

            if (user == null)
            {
                await DisplayAlertAsync("Login Failed", "Invalid username or password.", "OK");
                return;
            }

            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
            }
            catch
            {
            }

            BeepService.PlaySuccessBeep();

            await DisplayAlertAsync("Success", "Login successful.", "OK");

            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;

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