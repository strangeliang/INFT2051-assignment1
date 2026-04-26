using ZXing.Net.Maui;
using parcel_station1.Data;
using parcel_station1.Services;

namespace parcel_station1.Pages;

// ScanPage handles QR code scanning and parcel lookup.
public partial class ScanPage : ContentPage
{
    // Database object used to search for parcels.
    private readonly ParcelDatabase _parcelDatabase;

    // Stores the currently logged-in username so users can only access their own parcels.
    private readonly string _username;

    // Prevents the same QR code from being processed multiple times in a short period.
    private bool _isProcessing;

    public ScanPage(ParcelDatabase parcelDatabase, string username)
    {
        InitializeComponent();

        _parcelDatabase = parcelDatabase;
        _username = username;

        // Configure the barcode reader to scan QR codes and other 2D formats.
        barcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

    // Triggered automatically when the camera detects a barcode or QR code.
    private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing)
            return;

        var first = e.Results?.FirstOrDefault();

        if (first == null)
            return;

        _isProcessing = true;

        // Scanner events may not run on the UI thread, so UI updates and navigation
        // should be handled on the main thread.
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                // Pause detection to avoid repeated scans of the same code.
                barcodeReader.IsDetecting = false;

                string scannedValue = first.Value?.Trim() ?? string.Empty;

                // Display the scanned value on the page.
                ResultLabel.Text = $"Scanned: {scannedValue}";

                if (string.IsNullOrWhiteSpace(scannedValue))
                {
                    await DisplayAlertAsync("Scan Failed", "Scanned QR code is empty.", "OK");
                    barcodeReader.IsDetecting = true;
                    return;
                }

                // Search only within the current user's parcels.
                var parcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(scannedValue, _username);

                if (parcel == null)
                {
                    await DisplayAlertAsync("Not Found", $"No parcel found for code: {scannedValue}", "OK");

                    // Resume detection so the user can scan another parcel.
                    barcodeReader.IsDetecting = true;
                    return;
                }

                // Provide vibration feedback if supported by the device.
                try
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
                }
                catch
                {
                    // Ignore vibration errors on unsupported devices.
                }

                // Play a success sound after a valid parcel is found.
                BeepService.PlaySuccessBeep();

                await DisplayAlertAsync("Scan Success", $"Parcel found: {scannedValue}", "OK");

                // Navigate to the parcel result page.
                await Navigation.PushAsync(new ResultPage(parcel));
            }
            finally
            {
                // Allow the next scan attempt after the current process finishes.
                _isProcessing = false;
            }
        });
    }

    // Simulates scanning for testing or demo purposes when camera scanning is unavailable.
    private async void OnSimulateScanClicked(object sender, EventArgs e)
    {
        // This simulated parcel code must exist in the database to work correctly.
        string simulatedCode = "1234";

        ResultLabel.Text = $"Scanned: {simulatedCode}";

        var parcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(simulatedCode, _username);

        if (parcel == null)
        {
            await DisplayAlertAsync("Not Found", $"No parcel found for code: {simulatedCode}", "OK");
            return;
        }

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
        }
        catch
        {
            // Ignore vibration errors on unsupported devices.
        }

        BeepService.PlaySuccessBeep();

        await DisplayAlertAsync("Simulated Scan", $"Parcel found: {simulatedCode}", "OK");

        await Navigation.PushAsync(new ResultPage(parcel));
    }
}