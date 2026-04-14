using parcel_station1.Models;

namespace parcel_station1.Pages;

public partial class ResultPage : ContentPage
{
    public ResultPage(Parcel parcel)
    {
        InitializeComponent();

        ParcelStatusLabel.Text = parcel.Status;
        ParcelIdLabel.Text = $"Parcel ID: {parcel.ParcelCode}";
        PickupLocationLabel.Text = $"Pickup Location: {parcel.Location}";
        CollectionCodeLabel.Text = $"Collection Code: {parcel.CollectionCode}";
        PickupDeadlineLabel.Text = $"Pickup Deadline: {parcel.PickupDeadline}";

        ParcelQrTextLabel.Text = $"QR content: {parcel.ParcelCode}";
        ParcelQrCode.Value = parcel.ParcelCode;

        if (!string.IsNullOrWhiteSpace(parcel.Status) &&
            (parcel.Status.Equals("Ready for Collection", StringComparison.OrdinalIgnoreCase) ||
             parcel.Status.Equals("Ready", StringComparison.OrdinalIgnoreCase)))
        {
            ParcelStatusLabel.TextColor = Color.FromArgb("#15803D");
        }
        else if (!string.IsNullOrWhiteSpace(parcel.Status) &&
                 parcel.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            ParcelStatusLabel.TextColor = Color.FromArgb("#D97706");
        }
        else if (!string.IsNullOrWhiteSpace(parcel.Status) &&
                 parcel.Status.Equals("Collected", StringComparison.OrdinalIgnoreCase))
        {
            ParcelStatusLabel.TextColor = Color.FromArgb("#2563EB");
        }
        else
        {
            ParcelStatusLabel.TextColor = Color.FromArgb("#111827");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnBackToMainMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}