using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;

namespace PatchKit.Downloader
{
    public delegate void DownloaderProgressHandler(DownloaderProgress progress);


    internal class Downloader : IDownloader
    {
        internal const int DownloaderBufferSize = 1024;

        public string TempDirectory { get; set; }

        public string DownloadTorrent(Uri url,
            DownloaderProgressHandler onDownloadProgress, CancellationToken cancelToken)
        {
            string downloadDir = Utilities.TempDirectory.Create(TempDirectory, "torrent_");
            string torrentFilePath = DownloadFile(url, 0, progress => { }, cancelToken);

            var settings = new EngineSettings
            {
                AllowedEncryption = EncryptionTypes.All,
                PreferEncryption = true,
                SavePath = downloadDir
            };

            using (var engine = new ClientEngine(settings))
            {
                using (var torrentManager = new TorrentManager(
                    Torrent.Load(torrentFilePath), downloadDir, new TorrentSettings()))
                {
                    engine.Register(torrentManager);

                    engine.StartAll();

                    while (!torrentManager.Complete)
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            torrentManager.Stop();
                            cancelToken.ThrowIfCancellationRequested();
                        }

                        if (torrentManager.Error != null)
                        {
                            torrentManager.Stop();

                            throw new WebException(torrentManager.Error.Reason.ToString(),
                                torrentManager.Error.Exception);
                        }

                        if (onDownloadProgress != null)
                        {
                            onDownloadProgress(new DownloaderProgress()
                            {
                                Type = DownloaderProgressType.Downloading,
                                Progress = (float)torrentManager.Progress / 100.0f,
                                DownloadedBytesCount = (long)Math.Round(torrentManager.Progress / 100.0 * torrentManager.Torrent.Size),
                                TotalBytesCount = torrentManager.Torrent.Size
                            });
                        }

                        Thread.Sleep(5);
                    }

                    TorrentFile[] files = torrentManager.Torrent.Files;
                    if (files.Length >= 1)
                    {
                        TorrentFile torrentFile = files[0];
                        string fullPath = torrentFile.FullPath;
                        return fullPath;
                    }
                }
            }

            return null;
        }

        public string DownloadString(Uri url, DownloaderProgressHandler onDownloadProgress,
            CancellationToken cancelToken)
        {
            using (var destinationStream = new MemoryStream())
            {
                DownloadStream(url, destinationStream, 0, onDownloadProgress, cancelToken);

                return Encoding.UTF8.GetString(destinationStream.ToArray());
            }
        }

        public string DownloadFile(Uri url, long totalDownloadBytesCount,
            DownloaderProgressHandler onDownloadProgress, CancellationToken cancelToken)
        {
            string destinationFilePath = Path.Combine(TempDirectory, Path.GetRandomFileName());

            using (var destinationStream = new FileStream(destinationFilePath, FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
            {
                DownloadStream(url, destinationStream, totalDownloadBytesCount, onDownloadProgress, cancelToken);
            }

            return destinationFilePath;
        }

        private void DownloadStream(Uri url, Stream destinationStream,
            long totalDownloadBytesCount, DownloaderProgressHandler onDownloadProgress,
            CancellationToken cancelToken)
        {
            try
            {
                onDownloadProgress(new DownloaderProgress
                {
                    Progress = 0.0f,
                    Type = DownloaderProgressType.Started
                });

                ServicePointManager.DefaultConnectionLimit = 65535;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;

                using (var response = request.GetResponse())
                {
                    cancelToken.ThrowIfCancellationRequested();
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            throw new WebException("Empty response stream.");
                        }

                        byte[] buffer = new byte[DownloaderBufferSize];
                        int readBytesCount;
                        long totalReadBytesCount = 0;
                        while ((readBytesCount = responseStream.ReadAsync(buffer, 0, DownloaderBufferSize, cancelToken).Result) > 0)
                        {
                            totalReadBytesCount += readBytesCount;

                            onDownloadProgress(new DownloaderProgress
                            {
                                Progress = totalDownloadBytesCount == 0 ? 0.0f : (float)totalReadBytesCount / totalDownloadBytesCount,
                                DownloadedBytesCount = totalReadBytesCount,
                                TotalBytesCount = totalDownloadBytesCount,
                                Type = DownloaderProgressType.Downloading
                            });

                            cancelToken.ThrowIfCancellationRequested();
                            destinationStream.WriteAsync(buffer, 0, readBytesCount, cancelToken).Wait(cancelToken);
                        }
                    }
                }

                onDownloadProgress(new DownloaderProgress
                {
                    Progress = 1.0f,
                    Type = DownloaderProgressType.Succeed
                });
            }
            catch (OperationCanceledException)
            {
                onDownloadProgress(new DownloaderProgress
                {
                    Progress = 1.0f,
                    Type = DownloaderProgressType.Canceled
                });
                throw;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.All(exception => exception is OperationCanceledException))
                {
                    onDownloadProgress(new DownloaderProgress
                    {
                        Progress = 1.0f,
                        Type = DownloaderProgressType.Canceled
                    });
                }
                else
                {
                    onDownloadProgress(new DownloaderProgress
                    {
                        Progress = 1.0f,
                        Type = DownloaderProgressType.Failed,
                        Exception = ex
                    });
                }
                throw;
            }
            catch (Exception ex)
            {
                onDownloadProgress(new DownloaderProgress
                {
                    Progress = 1.0f,
                    Type = DownloaderProgressType.Failed,
                    Exception = ex
                });
                throw;
            }
        }
    }
}
