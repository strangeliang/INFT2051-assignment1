using ZXing.Net.Maui;
using parcel_station1.Data;

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

                string scannedValue = first.Value;
                ResultLabel.Text = $"Scanned: {scannedValue}";

                await DisplayAlert("Scan Success", $"QR Code: {scannedValue}", "OK");

                // 之后你可以在这里用 _parcelDatabase 查数据库
                // 也可以带着 scannedValue 跳转去结果页
            }
            finally
            {
                _isProcessing = false;
            }
        });
    }

    private async void OnSimulateScanClicked(object sender, EventArgs e)
    {
        ResultLabel.Text = "Scanned: 1234";
        await DisplayAlert("Simulated Scan", "QR Code: 1234", "OK");
    }
}