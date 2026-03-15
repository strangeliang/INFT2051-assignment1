namespace parcel_station1;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new parcel_station1.Pages.MainPage());
    }
}