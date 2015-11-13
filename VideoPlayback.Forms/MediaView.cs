using System;

using Xamarin.Forms;

namespace VideoPlayback.Forms
{
	/// <summary>
	/// View to render media content. This view is backed by native renderers and uses AVPlayerViewController on iOS and VideoPlayerView on Android.
	/// </summary>
	public sealed class MediaView : ContentView
	{
		public MediaView ()
		{
		}

		public static BindableProperty UrlProperty = BindableProperty.Create<MediaView, string>(p => p.Url, default(string), BindingMode.OneWay);
		public static BindableProperty AutoPlayProperty = BindableProperty.Create<MediaView, bool>(p => p.AutoPlay, false, BindingMode.OneWay);
		public static BindableProperty MessageProperty = BindableProperty.Create<MediaView, string>(p => p.Message, string.Empty, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the URL of the media content.
		/// </summary>
		public string Url
		{
			get
			{
				return (string)base.GetValue(UrlProperty);
			}
			set
			{
				base.SetValue(UrlProperty, value);
			}
		}

		/// <summary>
		/// Controls if the playback starts automatically or has to be started by the user. Default is FALSE.
		/// </summary>
		public bool AutoPlay
		{
			get
			{
				return (bool)base.GetValue(AutoPlayProperty);
			}
			set
			{
				base.SetValue(AutoPlayProperty, value);
			}
		}

		/// <summary>
		/// Defines the message to show while new media is loaded.
		/// </summary>
		public string Message
		{
			get
			{
				return (string)base.GetValue(MessageProperty);
			}
			set
			{
				base.SetValue(MessageProperty, value);
			}
		}


	}
}


