using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using MediaPlayer;
using Xamarin.Forms;
using VideoPlayback.Forms;
using VideoPlayback.Forms.iOS;
using System.ComponentModel;
using Foundation;
using System.Diagnostics;
using CoreGraphics;
using AVKit;
using AVFoundation;

[assembly:ExportRenderer (typeof(MediaView), typeof(MediaViewRenderer))]

namespace VideoPlayback.Forms.iOS
{
	/// <summary>
	/// iOS renderer for the MediaView. Uses AVPlayerViewController.
	/// </summary>
	public class MediaViewRenderer : ViewRenderer<MediaView, UIView>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VideoPlayback.Forms.iOS.MediaViewRenderer"/> class.
		/// </summary>
		public MediaViewRenderer ()
		{
		}

		// Used to observe the current player status.
		IDisposable playerLoadingStateObserver;

		// The movie player instance.
		AVPlayerViewController moviePlayer;

		// A label that shows the loading message.
		UIView loadingView;

		// Prevents double-disposing.
		bool isDisposed = false;

		/// <summary>
		/// Gets called if a Xamarin.Forms view wants to use this renderer.
		/// </summary>
		/// <param name="e">information about the view that just got connected</param>
		protected override void OnElementChanged (ElementChangedEventArgs<MediaView> e)
		{
			base.OnElementChanged (e);

			this.RecreateMoviePlayer(null);

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
			var mediaView = sender as MediaView;
			if(sender != null)
			{
				base.Frame = new CGRect(this.Frame.Location, new CGSize(Convert.ToSingle(mediaView.Width), Convert.ToSingle(mediaView.Height)));
				this.moviePlayer.View.Frame = this.Bounds;
				this.loadingView.Frame = this.Bounds;
			}
		}

		/// <summary>
		/// Recreates the movie player instance. 
		/// </summary>
		void RecreateMoviePlayer(NSUrl contentUrl)
		{
			if(this.moviePlayer == null)
			{
				this.moviePlayer = new AVPlayerViewController
				{
					ShowsPlaybackControls = true
				};
			}

			if(this.moviePlayer.Player != null)
			{
				if(this.playerLoadingStateObserver != null)
				{
					this.playerLoadingStateObserver.Dispose();
				}
				this.moviePlayer.Player.Dispose();
				this.moviePlayer.Player = null;
			}

			if(contentUrl != null)
			{
				this.moviePlayer.Player = new AVPlayer(contentUrl);
				this.playerLoadingStateObserver = this.moviePlayer.Player.AddObserver("status", NSKeyValueObservingOptions.New, change => {
					Debug.WriteLine("Player state: " + this.moviePlayer.Player.Status);
					if(this.moviePlayer.Player.Status == AVPlayerStatus.ReadyToPlay)
					{
						this.SetNativeControl(this.moviePlayer.View);
					}
				});
			}

			// Add the loading view to the movie player's view.
			this.loadingView = new UILabel(this.moviePlayer.View.Bounds)
			{
				AutoresizingMask = UIViewAutoresizing.All,
				Text = this.Element.Message,
				Font = UIFont.SystemFontOfSize(40),
				TextColor = UIColor.Black,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
				Hidden = true
			};

			this.SetNativeControl (this.loadingView);

			// Adjust the size of the native view.
			this.OnElementSizeChanged(this.Element, null);
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
			((UILabel)this.loadingView).Text = this.Element.Message;

			// If the content URL changes, recreate the player.
			if (e.PropertyName == "Url")
			{
				try
				{
					// We must tell the player if it is supposed to play a local file or stream from the web.
					// Attention: when streaming from the web, the URL must contain an extension or the response header must have a propert content type set, otherwise it won't play.
					var contentUrl = this.Element.Url.Trim();
					Debug.WriteLine("Playing file/url: " + contentUrl);
					if(contentUrl.ToLower ().StartsWith ("http://", StringComparison.Ordinal) || contentUrl.ToLower ().StartsWith ("https://", StringComparison.Ordinal))
					{
						this.RecreateMoviePlayer( NSUrl.FromString (contentUrl));
					}
					else
					{
						this.RecreateMoviePlayer(NSUrl.FromFilename (contentUrl));
					}

					this.loadingView.Hidden = false;

					if(this.Element.AutoPlay)
					{
						this.moviePlayer.Player.Play();
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine ("Failed to set new media URL to '{0}'. Exception: {1}", this.Element.Url, ex);
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					if(this.playerLoadingStateObserver != null)
					{
						this.playerLoadingStateObserver.Dispose();
					}

					if (this.moviePlayer != null)
					{
						if(this.moviePlayer.Player != null)
						{
							this.moviePlayer.Player.Pause ();
							this.moviePlayer.Player.Dispose();
						}
						this.moviePlayer.Dispose ();
						this.moviePlayer = null;
					}

					if(this.loadingView != null)
					{
						this.loadingView.RemoveFromSuperview();
						this.loadingView.Dispose();
						this.loadingView = null;
					}
				}
			}
			this.isDisposed = true;
			base.Dispose (disposing);
		}
	}
}

