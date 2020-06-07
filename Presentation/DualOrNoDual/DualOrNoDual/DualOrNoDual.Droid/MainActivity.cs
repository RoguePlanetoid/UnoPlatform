using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;

namespace DualOrNoDual.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = Uno.UI.ActivityHelper.AllConfigChanges | ConfigChanges.Orientation | ConfigChanges.ScreenSize,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
	}
}

