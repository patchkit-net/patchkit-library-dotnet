using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatchKit.Patcher.Downloader
{
    interface IDownloader
    {
        string TempDirectory { get; set; }

        Task<string> DownloadTorrent(Uri url, DownloaderProgressHandler onDownloadProgress,
            CancellationToken cancelToken);

        Task<string> DownloadString(Uri url, DownloaderProgressHandler onDownloadProgress,
            CancellationToken cancelToken);

        Task<string> DownloadFile(Uri url, long totalDownloadBytesCount,
            DownloaderProgressHandler onDownloadProgress, CancellationToken cancelToken);
    }
}
