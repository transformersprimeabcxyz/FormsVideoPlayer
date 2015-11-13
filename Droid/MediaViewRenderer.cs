using System;
using Xamarin.Forms;
using VideoPlayback.Forms;
using VideoPlayback.Forms.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using System.ComponentModel;
using SD = System.Diagnostics;
using Android.Content.Res;
using Android.App;
using Java.IO;

[assembly:ExportRenderer (typeof(MediaView), typeof(MediaViewRenderer))]

namespace VideoPlayback.Forms.Droid
{
	public class MediaViewRenderer : ViewRenderer<MediaView, VideoView>
	{
		public MediaViewRenderer ()
		{
		}

		MediaController mediaController;
		VideoView videoView;
		TextView loadingView;

		// Prevents double-disposing.
		bool isDisposed = false;

		/// <summary>
		/// Gets called if a Xamarin.Forms view wants to use this renderer.
		/// </summary>
		/// <param name="e">information about the view that just got connected</param>
		protected override void OnElementChanged (ElementChangedEventArgs<MediaView> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement == null)
			{
				this.videoView = new VideoView (this.Context);
				this.videoView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
				this.SetNativeControl (this.videoView);

				this.mediaController = new MediaController (this.Context);
				this.videoView.SetMediaController (this.mediaController);

				this.loadingView = new TextView (this.Context) {
					Text = this.Element.Message,
					TextAlignment = Android.Views.TextAlignment.Center,
					Visibility = Android.Views.ViewStates.Visible
				};
				//this.AddView (this.loadingView);
			}

			if (e.OldElement != null)
			{
				// Unsubscribe any events and perform other teardown operation from the Forms view if neccessary. It is possible that the same renderer gets reused for a new view.
				e.OldElement.SizeChanged -= this.OnElementSizeChanged;
			}

			if (e.NewElement != null)
			{
				// The new Forms view that will be using this renderer. Subscribe events or whatever has to be done.
				e.NewElement.SizeChanged += this.OnElementSizeChanged;
			}
		}

		/// <summary>
		/// Gets called by Forms if the size of the view changes. Adjusts the native view's frame.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnElementSizeChanged (object sender, EventArgs e)
		{
			this.videoView.Invalidate();
		}

		/// <summary>
		/// Gets called if a (bindable) property in the MediaView changes.
		/// </summary>
		/// <param name="sender">the MediaView instance</param>
		/// <param name="e">event arguments</param>
		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			// Always update the message - does not hurt.
			this.loadingView.Text = this.Element.Message;

			// If the content URL changes, recreate the player.
			if (e.PropertyName == "Url")
			{
				try
				{
					// We must tell the player if it is supposed to play a local file or stream from the web.
					// Attention: when streaming from the web, the URL must contain an extension or the response header must have a propert content type set, otherwise it won't play.
					var contentUrl = this.Element.Url.Trim ();
					SD.Debug.WriteLine ("Playing file/url: " + contentUrl);
					Android.Net.Uri uri = null;
					if (contentUrl.ToLower ().StartsWith ("http://", StringComparison.Ordinal) || contentUrl.ToLower ().StartsWith ("https://", StringComparison.Ordinal))
					{
						uri = Android.Net.Uri.Parse (contentUrl);
					}
					else
					{
						//uri = Android.Net.Uri.FromFile (new File (contentUrl));
						uri = Android.Net.Uri.Parse (contentUrl);
					}

					SD.Debug.WriteLine ("Parsed URI: " + uri);
					this.videoView.SetVideoURI(uri);

					//this.loadingView.Visibility = Android.Views.ViewStates.Visible;
					//this.videoView.SetZOrderOnTop(false);

					this.mediaController.Show();

					if (this.Element.AutoPlay)
					{
						this.videoView.Start ();
					}
				}
				catch (Exception ex)
				{
					SD.Debug.WriteLine ("Failed to set new media URL to '{0}'. Exception: {1}", this.Element.Url, ex);
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					if (this.videoView != null)
					{
						this.videoView.SetMediaController (null);
						this.videoView.Dispose ();
						this.videoView = null;
					}

					if (this.mediaController != null)
					{
						this.mediaController.Dispose ();
						this.mediaController = null;
					}

					if (this.loadingView != null)
					{
						this.loadingView.RemoveFromParent ();
						this.loadingView.Dispose ();
						this.loadingView = null;
					}
				}
			}
			this.isDisposed = true;
			base.Dispose (disposing);
		}
	}
}

