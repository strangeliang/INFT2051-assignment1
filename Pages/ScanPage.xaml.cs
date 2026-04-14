using ZXing.Net.Maui;
using parcel_station1.Data;
using parcel_station1.Services;

namespace parcel_station1.Pages;

public partial class ScanPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;
    private readonly string _username;
    private bool _isProcessing;

    public ScanPage(ParcelDatabase parcelDatabase, string username)
    {
        InitializeComponent();

        _parcelDatabase = parcelDatabase;
        _username = username;

        barcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

    private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing)
            return;

        var first = e.Results?.FirstOrDefault();
        if (first == null)
            return;

        _isProcessing = true;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                barcodeReader.IsDetecting = false;

                string scannedValue = first.Value?.Trim() ?? string.Empty;
                ResultLabel.Text = $"Scanned: {scannedValue}";

                if (string.IsNullOrWhiteSpace(scannedValue))
                {
                    await DisplayAlertAsync("Scan Failed", "Scanned QR code is empty.", "OK");
                    barcodeReader.IsDetecting = true;
                    return;
                }

                var parcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(scannedValue, _username);

                if (parcel == null)
                {
                    await DisplayAlertAsync("Not Found", $"No parcel found for code: {scannedValue}", "OK");
                    barcodeReader.IsDetecting = true;
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

                await DisplayAlertAsync("Scan Success", $"Parcel found: {scannedValue}", "OK");
                await Navigation.PushAsync(new ResultPage(parcel));
            }
            finally
            {
                _isProcessing = false;
            }
        });
    }

    private async void OnSimulateScanClicked(object sender, EventArgs e)
    {
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
        }

        BeepService.PlaySuccessBeep();

        await DisplayAlertAsync("Simulated Scan", $"Parcel found: {simulatedCode}", "OK");
        await Navigation.PushAsync(new ResultPage(parcel));
    }
}