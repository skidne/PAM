using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using Android.Graphics;
using Android.Hardware.Camera2;

namespace Lab1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
		private string Txtgoogle;
		private readonly int REQUEST_CAMERA = 132;
		private LensFacing CameraFacing;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

			NotificationHandle();
			SearchGoogleHandle();
			CameraHandle();
			PressMe();
		}

		private void NotificationHandle()
		{
			FindViewById<Button>(Resource.Id.notify).Click += (o, e) =>
			{
				Intent intent = new Intent(this, typeof(MainActivity));

				const int pendingIntentId = 0;
				PendingIntent pendingIntent = PendingIntent.GetActivity(this,
						pendingIntentId, intent, PendingIntentFlags.OneShot);

				NotificationCompat.Builder builder = new NotificationCompat
						.Builder(this)
						.SetContentIntent(pendingIntent)
						.SetContentTitle("Notification")
						.SetContentText("Wazzup nigg")
						.SetSmallIcon(Resource.Drawable.notification_icon_background)
						.SetAutoCancel(true)
						.SetPriority(2);

				Notification notification = builder.Build();
				NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);

				const int notif_id = 0;
				Handler handler = new Handler();
				handler.PostDelayed(() => manager.Notify(notif_id, notification), 10000);
			};
		}

		private void SearchGoogleHandle()
		{
			EditText edit = FindViewById<EditText>(Resource.Id.input);
			edit.TextChanged += (o, e) =>
				Txtgoogle = e.Text.ToString().Replace(' ', '+');

			FindViewById<Button>(Resource.Id.search).Click += (o, e) =>
			{
				var uri = Android.Net.Uri.Parse("https://www.google.com/search?q=" + Txtgoogle);
				StartActivity(new Intent(Intent.ActionView, uri));
			};
		}

		private void CameraHandle()
		{
			FindViewById<Button>(Resource.Id.takepic).Click += (o, e) =>
			{
				CameraFacing = FindViewById<RadioButton>(FindViewById<RadioGroup>(Resource.Id.radioGroup)
									.CheckedRadioButtonId).Text == "Front Camera" ? LensFacing.Front
									: LensFacing.Back;
				ActivityCompat.RequestPermissions(this, new string[]{Manifest.Permission.Camera},
					REQUEST_CAMERA);
			};
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
														Permission[] grantResults)
		{
			if (requestCode == REQUEST_CAMERA)
			{
				if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
					OpenCamera();
				else
					CreateSnackBar("Permission not granted.");
			}
			else
				base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		private void OpenCamera()
		{
			CameraManager manager = (CameraManager)GetSystemService(CameraService);
			var facing = -1;

			foreach (var id in manager.GetCameraIdList())
			{
				CameraCharacteristics cam_opt = manager.GetCameraCharacteristics(id);
				facing = (int)(cam_opt.Get(CameraCharacteristics.LensFacing) ?? -1);
				if (facing > -1 && facing == (int)CameraFacing)
					break;
			}

			if (facing > -1)
			{
				Intent intent = new Intent(MediaStore.ActionImageCapture);
				intent.PutExtra("android.intent.extras.CAMERA_FACING", facing);

				CreateSnackBar("Opening the " + (facing == 0 ? "Front" : "Back") + " Camera");

				StartActivityForResult(intent, 0);
			}
			else
				CreateSnackBar("No Camera devices found.");
		}

		protected override void OnActivityResult(int requestCode,
												[GeneratedEnum] Result resultCode,
												Intent data)
		{
			if (resultCode == Result.Ok)
			{
				base.OnActivityResult(requestCode, resultCode, data);
				Bitmap bit = (Bitmap)data.Extras.Get("data");

				Intent nextact = new Intent(this, typeof(PictureActivity));
				nextact.PutExtra("bit", bit);
				StartActivity(nextact);
			}
		}

		private void PressMe()
		{
			FindViewById(Resource.Id.kek).Click += (o, e) =>
				Snackbar.Make(FindViewById(Resource.Id.perm), "KILL URSELF", Snackbar.LengthShort)
						.Show();
		}

		private void CreateSnackBar(string txt)
		{
			Snackbar.Make(FindViewById(Resource.Id.perm), txt, Snackbar.LengthShort).Show();
		}
	}
}