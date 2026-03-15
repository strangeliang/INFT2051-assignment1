namespace parcel_station1.Pages;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ResultPage());
    }
}