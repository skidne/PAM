using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System.IO;
using System;
using System.Linq;
using Android.Content;
using Android.Runtime;
using System.Collections.Generic;
using Android.Support.Design.Widget;

namespace Lab2
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
		public string filename;
		public DateTime raw;
		public string date;
		public ListView listView;
		ArrayAdapter<string> adapt;
		public List<string> lst;
		public List<PendingIntent> pIntents;
		AlarmManager alarmManager;
		int count;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			filename = Path.Combine(path, "data.txt");
			SaveData(filename, "");
			File.WriteAllLines(filename,
				File.ReadLines(filename).Where(l => false).ToList());

			listView = FindViewById<ListView>(Resource.Id.listView);
			lst = new List<string>();
			adapt = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, lst);
			listView.Adapter = adapt;
			CalendarView calendar = FindViewById<CalendarView>(Resource.Id.calendar);
			alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
			pIntents = new List<PendingIntent>();
			count = 0;
			
			raw = DateTimeOffset.FromUnixTimeMilliseconds(calendar.Date).DateTime;
			date = raw.ToString("dd/MM/yyyy");
			calendar.DateChange += (ob, ev) =>
			{
				raw = new DateTime(ev.Year, ev.Month + 1, ev.DayOfMonth);
				date = raw.ToString("dd/MM/yyyy");
				GoList(date);
			};

			Button add = FindViewById<Button>(Resource.Id.add);
			add.Click += (o, e) => AddPrompt();
			listView.ItemClick += (o, e) =>
			{
				Intent newact = new Intent(this, typeof(EditActivity));
				newact.PutExtra("date", date);
				var line = File.ReadLines(filename).Where(l => l.StartsWith(date))
					.Select(l => { return l.Substring(11); })
					.First(l => l == listView.GetItemAtPosition(e.Position).ToString());
				var msg = line.Split('|');
				newact.PutExtra("time", msg[0]);
				newact.PutExtra("message", msg[1]);
				newact.PutExtra("id", int.Parse(msg[2]));
				StartActivityForResult(newact, 23);
			};

			var edit = FindViewById<EditText>(Resource.Id.inputsearch);
			string txt = null;

			edit.TextChanged += (o, e) => txt = e.Text.ToString();
			FindViewById<Button>(Resource.Id.searchbtn).Click += (o, e) =>
			{
				List<string> list = File.ReadLines(filename)
				.Where(l => l.ToLower().Contains(txt.ToLower())).ToList();
				if (list.Count > 0)
				{
					adapt.Clear();
					lst = list;
					adapt.AddAll(lst);
					listView.LayoutParameters.Height = (adapt.Count + 1) * 130;
					adapt.NotifyDataSetChanged();
				}
				else
					Snackbar.Make(FindViewById(Resource.Id.perm), "Not found.", Snackbar.LengthShort).Show();
			};
		}

		void AddPrompt()
		{
			Intent intent = new Intent(this,typeof(AddActivity));
			intent.PutExtra("date", date);
			StartActivityForResult(intent, 0);
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				var hour = data.GetIntExtra("hour", 0);
				var min = data.GetIntExtra("min", 0);
				var tmpdata = data.GetStringExtra("data");
				var remove = data.GetBooleanExtra("remove", false);
				var id = data.GetIntExtra("id", -1);

				if (requestCode == 23)
				{
					ClearData(filename, id);
					DeleteEvent(id);
				}
				if (!remove)
				{
					SaveData(filename, date + '|' + hour + ':' + min + " | " + tmpdata + "|" + count);
					CreateEvent(tmpdata, hour, min);
				}
				GoList(date);
			}
		}

		private void ClearData(string fn, int id)
		{
			File.WriteAllLines(fn,
					File.ReadLines(fn).Where(l => int.Parse(l.Split('|')[3]) != id).ToList());
		}

		private void DeleteEvent(int id)
		{
			pIntents.ElementAt(id).Cancel();
		}

		private void CreateEvent(string tmpdata, int hour, int min)
		{
			Intent alarmIntent = new Intent(this, typeof(AlarmReceiver));
			alarmIntent.PutExtra("title", "Event Alarm");
			alarmIntent.PutExtra("message", tmpdata);

			var pending = PendingIntent.GetBroadcast(this, count++, alarmIntent, PendingIntentFlags.OneShot);
			var datetime = new DateTime(raw.Year, raw.Month, raw.Day, hour, min, 0);
			var mil = ((DateTimeOffset)datetime).ToUnixTimeMilliseconds();

			alarmManager.Set(AlarmType.Rtc, mil, pending);
			pIntents.Add(pending);
		}

		void GoList(string date)
		{
			lst = File.ReadLines(filename).Where(l => l.StartsWith(date))
					.Select(l => { return l.Substring(11); }).ToList();
			adapt.Clear();
			adapt.AddAll(lst);
			listView.LayoutParameters.Height = (adapt.Count + 1) * 130;
			adapt.NotifyDataSetChanged();
		}

		void SaveData(string fn, string data)
		{
			using (var streamWriter = new StreamWriter(fn, true))
			{
				streamWriter.WriteLine(data);
			}
		}

	}
}