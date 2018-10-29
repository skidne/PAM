using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System.Net;
using AngleSharp.Parser.Html;
using Android.Graphics;
using System.Net.Http;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Android.Content;

namespace Lab3
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
		private TextView Errors;
		private EditText PhoneNumberRes;
		private EditText FromWhoRes;
		private EditText MessageRes;
		private EditText CaptchaAnswerRes;
		private ImageView CaptchaImgRes;
		private Button Send;
		
		private string CaptchaSid;
		private string CaptchaToken;
		private string CaptchaURL;
		private string FormBuildID;

		public string Url = "https://www.moldcell.md/rom/sendsms";

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
			Init();
			Send.Click += (o, e) => CheckErrors(PostRequest());
		}

		private string PostRequest()
		{
			using (var client = new HttpClient())
			{
				var reqParams = new Dictionary<string, string>
				{
					["captcha_sid"] = CaptchaSid,
					["captcha_token"] = CaptchaToken,
					["captcha_response"] = CaptchaAnswerRes.Text,
					["message"] = MessageRes.Text,
					["phone"] = PhoneNumberRes.Text,
					["name"] = FromWhoRes.Text,
					["form_build_id"] = FormBuildID,
					["form_id"] = "websms_main_form",
					["conditions"] = "1"
				};

				var resp = client.PostAsync(Url,
					new FormUrlEncodedContent(reqParams)).Result;

				return resp.Content.ReadAsStringAsync().Result;
			}
		}
		
		private void CheckErrors(string html)
		{
			var doc = new HtmlParser().Parse(html);

			var sel = doc.QuerySelector("div.messages");
			if (sel == null)
				Recreate();
			else
				Errors.Text = sel.TextContent;
		}

		private void Init()
		{
			Errors = FindViewById<TextView>(Resource.Id.errors);
			PhoneNumberRes = FindViewById<EditText>(Resource.Id.number);
			FromWhoRes = FindViewById<EditText>(Resource.Id.from);
			MessageRes = FindViewById<EditText>(Resource.Id.message);
			CaptchaAnswerRes = FindViewById<EditText>(Resource.Id.captcha_ans);
			CaptchaImgRes = FindViewById<ImageView>(Resource.Id.captcha_img);
			Send = FindViewById<Button>(Resource.Id.sendMsg);

			GetCaptcha();
			ShowCaptchaImage();
		}

		private void ShowCaptchaImage()
		{
			using (var bm = GetImageFromUrl(CaptchaURL))
				CaptchaImgRes.SetImageBitmap(bm);

			CaptchaImgRes.RequestLayout();
			CaptchaImgRes.LayoutParameters.Height = 150;
			CaptchaImgRes.LayoutParameters.Width = 300;
			CaptchaImgRes.SetScaleType(ImageView.ScaleType.FitXy);
		}

		private Bitmap GetImageFromUrl(string url)
		{
			using (var client = new HttpClient())
			{
				var resp = client.GetAsync(url).Result;
				var stream = resp.Content.ReadAsStreamAsync().Result;
				return BitmapFactory.DecodeStream(stream);
			}
		}

		private void GetCaptcha()
		{
			ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

			string html;
			using (var client = new HttpClient())
			{
				var resp = client.GetAsync(Url).Result;
				if (resp.IsSuccessStatusCode)
				{
					html = resp.Content.ReadAsStringAsync().Result;
					var doc = new HtmlParser().Parse(html);

					CaptchaSid = doc.QuerySelector("input[name='captcha_sid']").GetAttribute("value");

					CaptchaToken = doc.QuerySelector("input[name='captcha_token']").GetAttribute("value");

					var tmp = doc.QuerySelector("img[title='Imagine CAPTCHA']").GetAttribute("src");
					CaptchaURL = $"http://moldcell.md{tmp}";

					FormBuildID = doc.QuerySelector("input[name='form_build_id']").GetAttribute("value");
				}
				else
					StartActivity(new Intent(this, typeof(ActivitySecond)));
			}
		}

	}
}