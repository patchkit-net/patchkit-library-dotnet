using System;
using System.IO;
using System.Linq;
using System.Threading;
using PatchKit.Data.Local;
using PatchKit.Data.Remote;
using PatchKit.Downloader;
using PatchKit.Utilities;

namespace PatchKit
{
    public delegate void PatcherProgressHandler(PatcherProgress progress);

    /// <summary>
    /// Main class for patcher.
    /// </summary>
    public sealed class Patcher
    {
        /// <summary>
        /// Patcher status.
        /// </summary>
        public bool IsPatching { get; private set; }

        private readonly LocalApplication _localApplication;

        private readonly RemoteData _remoteDatabase;

        internal IDownloader _downloader;

        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Temporary directory for patches.
        /// </summary>
        public string _temporaryPatchesPath;

        /// <summary>
        /// Temporary directory for downloads.
        /// </summary>
        public string _temporaryDownloadsPath;

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
        /// <param name="patcherSettings">Patcher settings.</param>
        public Patcher(string localPath, PatcherSettings patcherSettings)
            : this(localPath, patcherSettings, new Downloader.Downloader())
        {

        }

        internal Patcher(string localPath, PatcherSettings patcherSettings, IDownloader downloader)
        {
            IsPatching = false;
            _localApplication = new LocalApplication(localPath, HashUtilities.ComputeStringHash(patcherSettings.SecretKey));
            _downloader = downloader;
            _remoteDatabase = new RemoteData(patcherSettings, _downloader);

        }

        public void CancelAsync()
        {
            if (!IsPatching)
            {
                throw new InvalidOperationException("You cannot cancel patcher if it's already working.");
            }

            _cancellationTokenSource.Cancel();
        }

        public void PatchAsync()
        {
            new Thread(Patch) { IsBackground = true }.Start();
        }

        public void Patch()
        {
            if (IsPatching)
            {
                throw new InvalidOperationException("You cannot start patcher if it's already patching.");
            }

            _cancellationTokenSource = new CancellationTokenSource();

            IsPatching = true;
            CreateTempDirectories();

            try
            {
                OnPatcherProgress(new PatcherProgress()
                {
                    Progress = 0.0f,
                    Type = PatcherProgressType.Started
                });

                /*_downloader.DownloadTorrent(new Uri(@"http://cdimage.debian.org/debian-cd/8.3.0/i386/bt-cd/debian-8.3.0-i386-netinst.iso.torrent"),
                    _localApplication.TemporaryDownloadsPath, OnDownloaderProgress, _cancellationTokenSource.Token);*/

                int currentlyAvailableVersion = _remoteDatabase.GetCurrentVersion(_cancellationTokenSource.Token);
                //For testing (TODO: place it under DEBUG or DEVELOPMENT #if)
                //_localDatabase.Cache.GetCommonVersion().HasValue ? _remoteDatabase.GetCurrentVersion(_cancellationTokenSource.Token) : 1;

                if (!ApplicationFilesConsistent())
                {
                    TryPatchWithContent(currentlyAvailableVersion);
                }
                else
                {
                    TryPatchWithDiff(currentlyAvailableVersion);
                }

                IsPatching = false;

                OnPatcherProgress(new PatcherProgress
                {
                    Progress = 1.0f,
                    Type = PatcherProgressType.Succeed
                });

            }
            catch (AggregateException ex)
            {
                IsPatching = false;

                if (ex.InnerExceptions.All(exception => exception is OperationCanceledException))
                {
                    OnPatcherProgress(new PatcherProgress
                    {
                        Progress = 1.0f,
                        Type = PatcherProgressType.Canceled
                    });
                }
                else
                {
                    OnPatcherProgress(new PatcherProgress
                    {
                        Progress = 1.0f,
                        Type = PatcherProgressType.Failed,
                        Exception = ex
                    });
                }
                //TODO: Czy napewno chcemy tutaj rethrowa? Crashuje on ca³¹ aplikacjê.
                //throw;
            }
            catch (OperationCanceledException)
            {
                IsPatching = false;

                OnPatcherProgress(new PatcherProgress()
                {
                    Progress = 1.0f,
                    Type = PatcherProgressType.Canceled
                });
                //TODO: Czy napewno chcemy tutaj rethrowa? Crashuje on ca³¹ aplikacjê.
                //throw;
            }
            catch (Exception ex)
            {
                IsPatching = false;

                OnPatcherProgress(new PatcherProgress
                {
                    Progress = 1.0f,
                    Type = PatcherProgressType.Failed,
                    Exception = ex
                });
                //TODO: Czy napewno chcemy tutaj rethrowa? Crashuje on ca³¹ aplikacjê.
                //throw;
            }
            finally
            {
                IsPatching = false;
                RemoveTempDirectories();
            }
        }

        private void CreateTempDirectories()
        {
            _temporaryPatchesPath = TempDirectory.Create("patches_");
            _temporaryDownloadsPath = TempDirectory.Create("downloads_");

            if (_downloader.TempDirectory == null)
            {
                _downloader.TempDirectory = _temporaryDownloadsPath;
            }
        }

        private void RemoveTempDirectories()
        {
            Directory.Delete(_temporaryPatchesPath, true);
            Directory.Delete(_temporaryDownloadsPath, true);
        }

        private void TryPatchWithContent(int version)
        {
            OnPatcherProgress(new PatcherProgress
            {
                Type = PatcherProgressType.Checking,
                Progress = 0.0f
            });

            if (CheckVersion(version, true) == false)
            {
                _localApplication.Clear(_cancellationTokenSource.Token);

                OnPatcherProgress(new PatcherProgress
                {
                    Type = PatcherProgressType.Downloading,
                    Progress = 0.0f
                });

                string contentPackage = _remoteDatabase.DownloadContent(version, OnDownloaderProgress,
                    _cancellationTokenSource.Token);

                ZipUtilities.FileUnzipStartHandler onContentFileUnzipStart = (filePath, fileName, progress) =>
                {
                    _localApplication.Cache.SetFileVersion(fileName, -1);
                };

                ZipUtilities.FileUnzipEndHandler onContentFileUnzipEnd = (filePath, fileName, progress) =>
                {
                    _localApplication.Cache.SetFileVersion(fileName, version);

                    OnPatcherProgress(new PatcherProgress
                    {
                        Type = PatcherProgressType.Unpacking,
                        FileName = fileName,
                        FilePath = filePath,
                        Progress = progress
                    });
                };

                OnPatcherProgress(new PatcherProgress
                {
                    Type = PatcherProgressType.Unpacking,
                    Progress = 0.0f
                });

                ZipUtilities.Unzip(contentPackage, _localApplication.Path,
                    onContentFileUnzipStart,
                    onContentFileUnzipEnd, _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// Tries to patch current application using diff files.
        /// </summary>
        /// <param name="targetVersion">The version that should be downloaded.</param>
        /// <returns></returns>
        private void TryPatchWithDiff(int targetVersion)
        {
            OnPatcherProgress(new PatcherProgress
            {
                Type = PatcherProgressType.Checking,
                Progress = 0.0f
            });

            float totalProgress = 0.0f;

            int localVersion = _localApplication.Cache.GetCommonVersion().Value;
            float progressPerVersion = 1.0f / (targetVersion - localVersion);


            while (!_localApplication.IsUpToDate(targetVersion))
            {
                int nextVersion = localVersion + 1;

                RemoteDiffSummary diffSummary =
                    _remoteDatabase.GetDiffSummary(nextVersion, _cancellationTokenSource.Token);

                OnPatcherProgress(new PatcherProgress
                {
                    Type = PatcherProgressType.Patching,
                    Progress = 0.0f,
                });

                OnPatcherProgress(new PatcherProgress
                {
                    Type = PatcherProgressType.Downloading,
                    Progress = totalProgress
                });

                string diffPackage = _remoteDatabase.DownloadDiff(nextVersion, OnDownloaderProgress,
                    _cancellationTokenSource.Token);

                while (Directory.Exists(_temporaryPatchesPath))
                {
                    Directory.Delete(_temporaryPatchesPath, true);

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

                        IOUtilities.CopyFile(filePath, localFilePath, true, _cancellationTokenSource.Token);

                        _localApplication.Cache.SetFileVersion(fileName, nextVersion);
                    }
                    else if (isModified)
                    {
                        PatchUtilities.Patch(_localApplication.GetFilePath(fileName), filePath,
                            _cancellationTokenSource.Token);

                        _localApplication.Cache.SetFileVersion(fileName, nextVersion);
                    }
                    else
                    {
                        throw new InvalidDataException("File of invaild type located in patch package: " + fileName);
                    }

                    OnPatcherProgress(new PatcherProgress
                    {
                        Type = PatcherProgressType.Patching,
                        FileName = fileName,
                        FilePath = filePath,
                        // ReSharper disable once AccessToModifiedClosure
                        Progress = totalProgress + progress * progressPerVersion
                    });
                };

                ZipUtilities.Unzip(diffPackage, _temporaryPatchesPath, onDiffFileUnzipStart, onDiffFileUnzipEnd,
                    _cancellationTokenSource.Token);

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

        private bool ApplicationFilesConsistent()
        {
            int? commonVersion = _localApplication.Cache.GetCommonVersion();
            if (!commonVersion.HasValue)
            {
                return false;
            }

            RemoteContentSummary contentSummary =
                _remoteDatabase.GetContentSummary(commonVersion.Value, _cancellationTokenSource.Token);
            if (!_localApplication.CheckFilesConsistency(contentSummary, _cancellationTokenSource.Token))
            {
                return false;
            }

            return true;
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
            }
            return false;
        }

        private bool CheckVersion(int version, bool checkHash)
        {
            if (version <= _remoteDatabase.GetCurrentVersion(_cancellationTokenSource.Token) && _localApplication.Cache.GetCommonVersion() == version)
            {
                var contentSummary = _remoteDatabase.GetContentSummary(version, _cancellationTokenSource.Token);

                foreach (var file in contentSummary.Files)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (checkHash ? !CheckFileWithHash(file.Path, version, file.Hash) : !CheckFile(file.Path, version))
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        private void OnPatcherProgress(PatcherProgress progress)
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
