using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Lab3
{
	[Activity(Label = "ActivitySecond")]
	public class ActivitySecond : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_sec);

			FindViewById<Button>(Resource.Id.refresh).Click += (o, e) => StartActivity(new Intent(this, typeof(MainActivity)));
		}
	}
}