#if ANDROID
using Android.Media;
#endif

namespace parcel_station1.Services;

// BeepService plays a system notification sound after successful actions.
public static class BeepService
{
    public static void PlaySuccessBeep()
    {
        try
        {
#if ANDROID
            // Get the default Android notification sound.
            Android.Net.Uri notificationUri =
                Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);

            // Create a ringtone object from the notification sound URI.
            var ringtone =
                Android.Media.RingtoneManager.GetRingtone(
                    Android.App.Application.Context,
                    notificationUri);

            ringtone?.Play();
#endif
        }
        catch
        {
            // Ignore sound errors so the main app flow is not interrupted.
        }
    }
}