using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Lab2
{
	[Activity(Label = "EditActivity")]
	public class EditActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_edit);

			FindViewById<TextView>(Resource.Id.datetextEdit).Text += Intent.GetStringExtra("date");
			EditText edit = FindViewById<EditText>(Resource.Id.eventdataEdit);
			edit.Text = Intent.GetStringExtra("message");
			TimePicker tp = FindViewById<TimePicker>(Resource.Id.timePickerEdit);
			tp.SetIs24HourView(Java.Lang.Boolean.True);
			var time = Intent.GetStringExtra("time").Split(':');
			tp.Hour = int.Parse(time[0]);
			tp.Minute = int.Parse(time[1]);

			string tmp = "";

			edit.TextChanged += (o, e) => tmp = e.Text.ToString();
			FindViewById<Button>(Resource.Id.savebtn).Click += (o, e) =>
			{
				if (tmp != "")
				{
					Intent newact = new Intent(this, typeof(MainActivity));
					newact.PutExtra("data", tmp);
					newact.PutExtra("hour", tp.Hour);
					newact.PutExtra("min", tp.Minute);
					newact.PutExtra("remove", false);
					newact.PutExtra("id", Intent.GetIntExtra("id", -1));
					SetResult(Result.Ok, newact);
					Finish();
				}
			};

			FindViewById<Button>(Resource.Id.remove).Click += (o, e) =>
			{
				Intent newact = new Intent(this, typeof(MainActivity));
				newact.PutExtra("remove", true);
				newact.PutExtra("id", Intent.GetIntExtra("id", -1));
				SetResult(Result.Ok, newact);
				Finish();
			};
		}
	}
}