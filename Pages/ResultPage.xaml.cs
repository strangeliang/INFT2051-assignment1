using parcel_station1.Models;

namespace parcel_station1.Pages;

// ResultPage displays detailed information for a selected parcel.
public partial class ResultPage : ContentPage
{
    public ResultPage(Parcel parcel)
    {
        InitializeComponent();

        // Display the main parcel details.
        ParcelStatusLabel.Text = parcel.Status;
        ParcelIdLabel.Text = $"Parcel ID: {parcel.ParcelCode}";
        PickupLocationLabel.Text = $"Pickup Location: {parcel.Location}";
        CollectionCodeLabel.Text = $"Collection Code: {parcel.CollectionCode}";
        PickupDeadlineLabel.Text = $"Pickup Deadline: {parcel.PickupDeadline}";

        // Use the parcel code as the QR code value.
        ParcelQrTextLabel.Text = $"QR content: {parcel.ParcelCode}";
        ParcelQrCode.Value = parcel.ParcelCode;

        // Set the status text colour based on the parcel status.
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