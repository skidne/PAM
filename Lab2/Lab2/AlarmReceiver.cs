using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Lab2
{
	[BroadcastReceiver]
	public class AlarmReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var title = intent.GetStringExtra("title");
			var message = intent.GetStringExtra("message");
			var resultIntent = new Intent(context, typeof(MainActivity));

			resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

			var pending = PendingIntent.GetActivity(context, 0,
				resultIntent,
				PendingIntentFlags.CancelCurrent);

			var builder = new NotificationCompat.Builder(context)
					.SetContentTitle(title)
					.SetContentText(message)
					.SetSmallIcon(Resource.Drawable.notification_template_icon_bg);

			builder.SetContentIntent(pending);

			var notification = builder.Build();

			var manager = NotificationManager.FromContext(context);
			manager.Notify(1337, notification);
		}
	}
}