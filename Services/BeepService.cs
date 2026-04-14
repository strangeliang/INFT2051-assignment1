#if ANDROID
using Android.Media;
#endif

namespace parcel_station1.Services;

public static class BeepService
{
    public static void PlaySuccessBeep()
    {
        try
        {
#if ANDROID
            Android.Net.Uri notificationUri =
                Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);

            var ringtone =
                Android.Media.RingtoneManager.GetRingtone(Android.App.Application.Context, notificationUri);

            ringtone?.Play();
#endif
        }
        catch
        {
        }
    }
}