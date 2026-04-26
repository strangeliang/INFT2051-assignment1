using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using ZXing.Net.Maui.Controls;
using parcel_station1.Data;
using parcel_station1.Pages;

namespace parcel_station1
{
    // MauiProgram configures the MAUI app, third-party libraries, fonts, services, and database.
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()

                // Enables CommunityToolkit.Maui features.
                .UseMauiCommunityToolkit()

                // Enables Syncfusion MAUI toolkit components.
                .ConfigureSyncfusionToolkit()

                // Enables ZXing.Net.MAUI barcode and QR scanning.
                .UseBarcodeReader()

                // Configures platform-specific handlers.
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
                    // Prevents CollectionView selection from changing automatically when focus changes on Windows.
                    Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
                    {
                        handler.PlatformView.SingleSelectionFollowsFocus = false;
                    });

                    // Makes the custom CategoryChart control focusable by keyboard on Windows.
                    Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(Pages.Controls.CategoryChart), (handler, view) =>
                    {
                        if (view is Pages.Controls.CategoryChart && handler.PlatformView is Microsoft.Maui.Platform.ContentPanel contentPanel)
                        {
                            contentPanel.IsTabStop = true;
                        }
                    });
#endif
                })

                // Registers app fonts.
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            // Enables debug logging in Visual Studio output.
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            // =========================
            // Original app services
            // =========================

            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();

            // =========================
            // Parcel Station SQLite database
            // =========================

            // Stores the local SQLite database inside the app data directory.
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "parcel.db3");

            // Registers ParcelDatabase as a singleton so all pages share the same database instance.
            builder.Services.AddSingleton(new ParcelDatabase(dbPath));

            // Registers the login page for dependency injection.
            builder.Services.AddSingleton<MainPage>();

            // =========================
            // Shell route registration
            // =========================

            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            return builder.Build();
        }
    }
}