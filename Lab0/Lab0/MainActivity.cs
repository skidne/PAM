using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System;
using Android.Graphics;

namespace Lab0
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
		TextView hello;
		Random rand = new Random();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			hello = FindViewById<TextView>(Resource.Id.hello);

			FindViewById<Button>(Resource.Id.colbutton).Click += (o, e) =>
				hello.SetTextColor(Color.Rgb(rand.Next(255), rand.Next(255), rand.Next(255)));
		}
    }
}