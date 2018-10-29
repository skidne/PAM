using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Lab1
{
	[Activity(Label = "PictureActivity")]
	public class PictureActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.picture_layout);

			ImageView img = FindViewById<ImageView>(Resource.Id.picture);
			img.SetImageBitmap((Bitmap)Intent.GetParcelableExtra("bit"));

			var btn = FindViewById(Resource.Id.button1);
			btn.Click += (o, e) => Finish();
		}
	}
}