using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace Lab2
{
	[Activity(Label = "AddActivity")]
	public class AddActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_add);

			FindViewById<TextView>(Resource.Id.datetext).Text += Intent.GetStringExtra("date");
			EditText edit = FindViewById<EditText>(Resource.Id.eventdata);
			TimePicker tp = FindViewById<TimePicker>(Resource.Id.timePicker);
			tp.SetIs24HourView(Java.Lang.Boolean.True);

			string tmp = "";

			edit.TextChanged += (o, e) => tmp = e.Text.ToString();
			FindViewById<Button>(Resource.Id.addbtn).Click += (o, e) =>
			{
				if (tmp != "")
				{
					Intent newact = new Intent(this, typeof(MainActivity));
					newact.PutExtra("data", tmp);
					newact.PutExtra("hour", tp.Hour);
					newact.PutExtra("min", tp.Minute);
					newact.PutExtra("remove", false);
					newact.PutExtra("id", -1);
					SetResult(Result.Ok, newact);
					Finish();
				}
			};
		}
	}
}