using System;
using System.Threading;

namespace PatchKit.Patcher.Downloader
{
    interface IDownloader
    {
        string TempDirectory { get; set; }

        string DownloadTorrent(Uri url, DownloaderProgressHandler onDownloadProgress,
            CancellationToken cancelToken);

        string DownloadString(Uri url, DownloaderProgressHandler onDownloadProgress,
            CancellationToken cancelToken);

        string DownloadFile(Uri url, long totalDownloadBytesCount,
            DownloaderProgressHandler onDownloadProgress, CancellationToken cancelToken);
    }
}
