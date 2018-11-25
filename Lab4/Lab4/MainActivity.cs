using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;
using System.Threading.Tasks;
using System.Timers;

namespace Lab4
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			var img = FindViewById<ImageView>(Resource.Id.imageView1);
			img.SetBackgroundResource(Resource.Drawable.animat);
			var anim = (AnimationDrawable) img.Background;
			var btn = FindViewById<Button>(Resource.Id.button1);
			btn.Click += async (o, e) =>
			{
				img.Visibility = Android.Views.ViewStates.Visible;
				anim.Start();
				btn.Enabled = false;
				await Task.Delay(5000);
				anim.Stop();
				img.Visibility = Android.Views.ViewStates.Invisible;
				btn.Enabled = true;
			};
		}
	}
}