using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PatchKit.Downloader
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
