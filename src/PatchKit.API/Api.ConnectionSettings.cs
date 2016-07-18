using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PatchKit
{
	public partial class Api
	{
		/// <summary>
		/// <see cref="Api"/> settings.
		/// </summary>
		public class ConnectionSettings
		{
			private string[] _urls;

			private long _timeout;

			public ConnectionSettings(long timeout = 500, params string[] urls)
			{
				Url = url;
				MirrorUrls = mirrorUrls;
				Timeout = timeout;
			}

			/// <summary>
			/// Urls for API servers. Priority of servers is based on the array order.
			/// </summary>
			public IEnumerable<string> Urls
			{
				get
				{
					return _urls;
				}
				set
				{
					if (value == null)
					{
						throw new ArgumentNullException("value");
					}
					if (value.Length == 0)
					{
						throw new ArgumentException("value");
					}
					_urls = value;
				}
			}

			/// <summary>
			/// The urls for mirror API servers.
			/// </summary>
			[CanBeNull]
			public string[] MirrorUrls { get; set; }

			/// <summary>
			/// Request timeout.
			/// </summary>
			public long Timeout
			{
				get
				{
					return _timeout;
				}
				set
				{
					if (value <= 0)
					{
						throw new ArgumentOutOfRangeException("value");
					}
					_timeout = value;
				}
			}
		}
	}
}