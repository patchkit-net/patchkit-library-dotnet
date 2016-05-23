using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PatchKit.API;
using PatchKit.API.Data;
using PatchKit.Patcher.Data.Local;
using PatchKit.Patcher.Downloader;
using PatchKit.Patcher.Utilities;

namespace PatchKit.Patcher
{
    public delegate void PatcherProgressHandler(PatchKitPatcherProgress progress);

    /// <summary>
    /// Main class for patcher.
    /// </summary>
    public sealed class PatchKitPatcher
    {
        /// <summary>
        /// Patcher status.
        /// </summary>
        public bool IsPatching { get; private set; }

        public readonly PatchKitAPI PatchKitAPI;

        private readonly LocalApplication _localApplication;

        private readonly string _secretKey;

        internal IDownloader Downloader;

        /// <summary>
        /// Temporary directory for patches.
        /// </summary>
        public string TemporaryPatchesPath;

        /// <summary>
        /// Temporary directory for downloads.
        /// </summary>
        public string TemporaryDownloadsPath;

        /// <summary>
        /// Notifies about patching patcher progress.
        /// </summary>
        public event PatcherProgressHandler PatcherProgress;

        /// <summary>
        /// Notifies about patching downloader progress.
        /// </summary>
        public event DownloaderProgressHandler DownloaderProgress;

        /// <summary>
        /// Initializes the patcher.
        /// </summary>
        /// <param name="localPath">Path where local data is stored.</param>
        /// <param name="secretKey">Application secret key.</param>
        /// <param name="patchKitAPI">PatchKit API.</param>
        public PatchKitPatcher(string localPath, string secretKey, PatchKitAPI patchKitAPI)
            : this(localPath, secretKey, patchKitAPI, new Downloader.Downloader())
        {
        }

        /// <summary>
        /// Initializes the patcher.
        /// </summary>
        /// <param name="localPath">Path where local data is stored.</param>
        /// <param name="secretKey">Application secret key.</param>
        /// <param name="patchKitAPISettings">PatchKit API settings used to create API object.</param>
        public PatchKitPatcher(string localPath, string secretKey, PatchKitAPISettings patchKitAPISettings)
            : this(localPath, secretKey, new PatchKitAPI(patchKitAPISettings), new Downloader.Downloader())
        {
        }

        internal PatchKitPatcher(string localPath, string secretKey, PatchKitAPI patchKitAPI, IDownloader downloader)
        {
            IsPatching = false;
            Downloader = downloader;
            _secretKey = secretKey;
            _localApplication = new LocalApplication(localPath);
            PatchKitAPI = patchKitAPI;
        }

        public async Task PatchAsync(CancellationToken cancellationToken)
        {
            if (IsPatching)
            {
                throw new InvalidOperationException("You cannot start patcher if it's already patching.");
            }

            IsPatching = true;

            CreateTempDirectories();

            try
            {
                OnPatcherProgress(new PatchKitPatcherProgress()
                {
                    Progress = 0.0f,
                    Type = PatchKitPatcherProgressType.Started
                });

                var currentlyAvailableVersion = await PatchKitAPI.GetAppLatestVersionIdAsync(_secretKey, cancellationToken);

                if (!await ApplicationFilesConsistent(cancellationToken))
                {
                    await TryPatchWithContent(currentlyAvailableVersion.Id, cancellationToken);
                }
                else
                {
                    await TryPatchWithDiff(currentlyAvailableVersion.Id, cancellationToken);
                }

                IsPatching = false;

                OnPatcherProgress(new PatchKitPatcherProgress
                {
                    Progress = 1.0f,
                    Type = PatchKitPatcherProgressType.Succeed
                });
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.All(exception => exception is OperationCanceledException))
                {
                    OnPatcherProgress(new PatchKitPatcherProgress
                    {
                        Progress = 1.0f,
                        Type = PatchKitPatcherProgressType.Canceled
                    });
                }
                else
                {
                    OnPatcherProgress(new PatchKitPatcherProgress
                    {
                        Progress = 1.0f,
                        Type = PatchKitPatcherProgressType.Failed,
                        Exception = ex
                    });
                }
                throw;
            }
            catch (OperationCanceledException)
            {
                OnPatcherProgress(new PatchKitPatcherProgress()
                {
                    Progress = 1.0f,
                    Type = PatchKitPatcherProgressType.Canceled
                });
                throw;
            }
            catch (Exception ex)
            {
                OnPatcherProgress(new PatchKitPatcherProgress
                {
                    Progress = 1.0f,
                    Type = PatchKitPatcherProgressType.Failed,
                    Exception = ex
                });
                throw;
            }
            finally
            {
                IsPatching = false;
                RemoveTempDirectories();
            }
        }

        private void CreateTempDirectories()
        {
            TemporaryPatchesPath = TempDirectory.Create("patches_");
            TemporaryDownloadsPath = TempDirectory.Create("downloads_");

            if (Downloader.TempDirectory == null)
            {
                Downloader.TempDirectory = TemporaryDownloadsPath;
            }
        }

        private void RemoveTempDirectories()
        {
            Directory.Delete(TemporaryPatchesPath, true);
            Directory.Delete(TemporaryDownloadsPath, true);
        }

        private async Task TryPatchWithContent(int version, CancellationToken cancellationToken)
        {
            OnPatcherProgress(new PatchKitPatcherProgress
            {
                Type = PatchKitPatcherProgressType.Checking,
                Progress = 0.0f
            });

            if (await CheckVersion(version, true, cancellationToken) == false)
            {
                _localApplication.Clear();

                OnPatcherProgress(new PatchKitPatcherProgress
                {
                    Type = PatchKitPatcherProgressType.Downloading,
                    Progress = 0.0f
                });

                string contentTorrentUrl = (await PatchKitAPI.GetAppContentTorrentUrlAsync(_secretKey, version, cancellationToken)).Url;

                string contentPackage = await Downloader.DownloadTorrent(new Uri(contentTorrentUrl), OnDownloaderProgress, cancellationToken);

                ZipUtilities.FileUnzipStartHandler onContentFileUnzipStart = (filePath, fileName, progress) =>
                {
                    _localApplication.Cache.SetFileVersion(fileName, -1);
                };

                ZipUtilities.FileUnzipEndHandler onContentFileUnzipEnd = (filePath, fileName, progress) =>
                {
                    _localApplication.Cache.SetFileVersion(fileName, version);

                    OnPatcherProgress(new PatchKitPatcherProgress
                    {
                        Type = PatchKitPatcherProgressType.Unpacking,
                        FileName = fileName,
                        FilePath = filePath,
                        Progress = progress
                    });
                };

                OnPatcherProgress(new PatchKitPatcherProgress
                {
                    Type = PatchKitPatcherProgressType.Unpacking,
                    Progress = 0.0f
                });

                ZipUtilities.Unzip(contentPackage, _localApplication.Path,
                    onContentFileUnzipStart,
                    onContentFileUnzipEnd, cancellationToken);
            }
        }

        private async Task TryPatchWithDiff(int targetVersion, CancellationToken cancellationToken)
        {
            OnPatcherProgress(new PatchKitPatcherProgress
            {
                Type = PatchKitPatcherProgressType.Checking,
                Progress = 0.0f
            });

            float totalProgress = 0.0f;

            // ReSharper disable once PossibleInvalidOperationException
            int localVersion = _localApplication.Cache.GetCommonVersion().Value;
            float progressPerVersion = 1.0f / (targetVersion - localVersion);

            while (!_localApplication.IsUpToDate(targetVersion))
            {
                int nextVersion = localVersion + 1;

                AppDiffSummary diffSummary = await PatchKitAPI.GetAppDiffSummaryAsync(_secretKey, nextVersion, cancellationToken);

                OnPatcherProgress(new PatchKitPatcherProgress
                {
                    Type = PatchKitPatcherProgressType.Patching,
                    Progress = 0.0f,
                });

                OnPatcherProgress(new PatchKitPatcherProgress
                {
                    Type = PatchKitPatcherProgressType.Downloading,
                    Progress = totalProgress
                });

                var diffPackageUrl = await PatchKitAPI.GetAppDiffTorrentUrlAsync(_secretKey, nextVersion, cancellationToken);

                string diffPackage = await Downloader.DownloadTorrent(new Uri(diffPackageUrl.Url), OnDownloaderProgress, cancellationToken);

                while (Directory.Exists(TemporaryPatchesPath))
                {
                    Directory.Delete(TemporaryPatchesPath, true);

                    Thread.Sleep(1000);
                }

                ZipUtilities.FileUnzipStartHandler onDiffFileUnzipStart = (filePath, fileName, progress) =>
                {
                    _localApplication.Cache.SetFileVersion(fileName, -1);
                };

                ZipUtilities.FileUnzipEndHandler onDiffFileUnzipEnd = (filePath, fileName, progress) =>
                {
                    bool isAdded = diffSummary.AddedFiles.Contains(fileName);
                    bool isModified = diffSummary.ModifiedFiles.Contains(fileName);

                    if (isAdded)
                    {
                        string localFilePath = _localApplication.GetFilePath(fileName);

                        string localFileDirectoryPath = Path.GetDirectoryName(localFilePath);

                        if (localFileDirectoryPath != null)
                        {
                            Directory.CreateDirectory(localFileDirectoryPath);
                        }

                        IOUtilities.CopyFile(filePath, localFilePath, true, cancellationToken);

                        _localApplication.Cache.SetFileVersion(fileName, nextVersion);
                    }
                    else if (isModified)
                    {
                        PatchUtilities.Patch(_localApplication.GetFilePath(fileName), filePath, cancellationToken);

                        _localApplication.Cache.SetFileVersion(fileName, nextVersion);
                    }
                    else
                    {
                        throw new InvalidDataException("File of invaild type located in patch package: " + fileName);
                    }

                    OnPatcherProgress(new PatchKitPatcherProgress
                    {
                        Type = PatchKitPatcherProgressType.Patching,
                        FileName = fileName,
                        FilePath = filePath,
                        // ReSharper disable once AccessToModifiedClosure
                        Progress = totalProgress + progress * progressPerVersion
                    });
                };

                ZipUtilities.Unzip(diffPackage, TemporaryPatchesPath, onDiffFileUnzipStart, onDiffFileUnzipEnd, cancellationToken);

                foreach (var diffFile in diffSummary.RemovedFiles)
                {
                    _localApplication.ClearFile(diffFile);
                }

                foreach (var file in _localApplication.Cache.GetFileNames().ToArray())
                {
                    _localApplication.Cache.SetFileVersion(file, nextVersion);
                }

                localVersion = nextVersion;
                totalProgress += progressPerVersion;
            }
        }

        private async Task<bool> CheckVersion(int version, bool checkHash, CancellationToken cancellationToken)
        {
            if (version <= (await PatchKitAPI.GetAppLatestVersionIdAsync(_secretKey, cancellationToken)).Id && _localApplication.Cache.GetCommonVersion() == version)
            {
                var contentSummary = await PatchKitAPI.GetAppContentSummaryAsync(_secretKey, version, cancellationToken);

                foreach (var file in contentSummary.Files)
                {
                    if (checkHash ? !CheckFileWithHash(file.Path, version, file.Hash) : !CheckFile(file.Path, version))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private async Task<bool> ApplicationFilesConsistent(CancellationToken cancellationToken)
        {
            int? commonVersion = _localApplication.Cache.GetCommonVersion();
            if (!commonVersion.HasValue)
            {
                return false;
            }

            var contentSummary = await PatchKitAPI.GetAppContentSummaryAsync(_secretKey, commonVersion.Value, cancellationToken);

            return _localApplication.CheckFilesConsistency(commonVersion.Value, contentSummary);
        }

        private bool CheckFile(string file, int version)
        {
            var localVersion = _localApplication.Cache.GetFileVersion(file);

            if (localVersion != null && localVersion == version)
            {
                if (File.Exists(_localApplication.GetFilePath(file)))
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        private bool CheckFileWithHash(string file, int version, string hash)
        {
            if (CheckFile(file, version))
            {
                string fileHash = HashUtilities.ComputeFileHash(_localApplication.GetFilePath(file));

                if (hash == fileHash)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        private void OnPatcherProgress(PatchKitPatcherProgress progress)
        {
            var handler = PatcherProgress;
            if (handler != null)
            {
                handler.Invoke(progress);
            }
        }

        private void OnDownloaderProgress(DownloaderProgress progress)
        {
            var handler = DownloaderProgress;
            if (handler != null)
            {
                handler.Invoke(progress);
            }
        }
    }
}
