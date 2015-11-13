using System;

using Xamarin.Forms;

namespace VideoPlayback.Forms
{
	public class App : Application
	{
		Switch switchLocal;
		MediaView mediaView;

		public App ()
		{
			this.mediaView = new MediaView {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Message = "Loading!"
			};

			this.switchLocal = new Switch {
				IsToggled = false
			};

			this.MainPage = new ContentPage {
				Content = new StackLayout {
					Padding = new Thickness (20),
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Children = {
						new StackLayout {
							Orientation = StackOrientation.Horizontal,
							Children = {
								new Label {
									Text = "Use remote data?"
								},
								switchLocal
							}
						},

						this.CreatePlayButton ("Play MP3", "(no online version available!)", Device.OnPlatform ("song.mp3", "android.resource://net.csharx.videoplayback/raw/song", null)),
						this.CreatePlayButton ("Play simple MP4", "http://techslides.com/demos/sample-videos/small.mp4", Device.OnPlatform ("simple.mp4", "android.resource://net.csharx.videoplayback/raw/simple", null)),

						this.mediaView
					}
				}

			};
		}

		View CreatePlayButton (string title, string remoteUrl, string localUrl)
		{
			var btn = new Button {
				Text = title
			};
			btn.Clicked += (sender, e) => {
				if (this.switchLocal.IsToggled)
				{
					mediaView.Url = remoteUrl;
				}
				else
				{
					mediaView.Url = localUrl;
				}

			};

			return btn;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

