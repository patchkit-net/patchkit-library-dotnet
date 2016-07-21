namespace PatchKit.Api
{
	/// <summary>
	/// <see cref="ApiConnection"/> settings.
	/// </summary>
	[System.Serializable]
	public struct ApiConnectionSettings
	{
	    public ApiConnectionSettings(int timeout, params string[] urls) : this()
	    {
	        Urls = urls;
	        Timeout = timeout;
	    }

	    /// <summary>
	    /// Urls for API servers. Priority of servers is based on the array order. Must contain at least one url.
	    /// </summary>
	    public string[] Urls;

	    /// <summary>
	    /// Timeout of request to API server. Note that total timeout is calculated by multiplying this value by the amount of <see cref="Urls"/>.
	    /// </summary>
	    public int Timeout;
	}
}