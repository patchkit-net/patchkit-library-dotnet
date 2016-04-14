using System;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof (Patcher));

        /// <summary>
        /// Patcher status.
        /// </summary>
        public bool IsPatching { get; private set; }

        private readonly LocalApplication _localApplication;

        private readonly RemoteData _remoteDatabase;

        internal IDownloader Downloader;

        private CancellationTokenSource _cancellationTokenSource;

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
        /// <param name="patcherSettings">Patcher settings.</param>
        public Patcher(string localPath, PatcherSettings patcherSettings)
            : this(localPath, patcherSettings, new Downloader.Downloader())
        {
        }

        internal Patcher(string localPath, PatcherSettings patcherSettings, IDownloader downloader)
        {
            Log.Debug("[*new*]");
            Log.Debug(string.Format("Local path: {0}", localPath));
            Log.Debug(string.Format("Patcher settings:" +
                                    "- Service URL: {0}" +
                                    "- Secret Key: {1}", patcherSettings.ServiceURL, patcherSettings.SecretKey));
            Log.Debug(string.Format("Downloader: {0}", downloader.GetType()));

            IsPatching = false;
            Downloader = downloader;
            _localApplication = new LocalApplication(localPath, HashUtilities.ComputeStringHash(patcherSettings.SecretKey));
            _remoteDatabase = new RemoteData(patcherSettings, Downloader);

            Log.Debug("[-new-]");
        }

        public int GetCurrentVersion()
        {
            Log.Debug("[-*GetCurrentVersion*-]");
            return _remoteDatabase.GetCurrentVersion(new CancellationTokenSource().Token);
        }

        public void GetCurrentVersionAsync(Action<int, bool> callback)
        {
            Log.Debug("[*GetCurrentVersionAsync*]");
            Log.Debug(string.Format("Callback: {0}", callback));

            new Thread(() =>
            {
                Log.Debug("[*GetCurrentVersionAsync:thread*]");
                try
                {
                    var version = GetCurrentVersion();
                    callback(version, true);
                }
                catch (Exception exception)
                {
                    Log.Error("[GetCurrentVersionAsync:thread] : exception", exception);
                    callback(0, false);
                }
                Log.Debug("[-GetCurrentVersionAsync:thread-]");
            })
            { IsBackground = true }.Start();

            Log.Debug("[-GetCurrentVersionAsync-]");
        }

        public RemoteVersionInfo GetVersionInfo(int version)
        {
            Log.Debug("[-*GetVersionInfo*-]");
            Log.Debug(string.Format("Version: {0}", version));

            return _remoteDatabase.GetVersionInfo(version, new CancellationTokenSource().Token);
        }

        public void GetVersionInfoAsync(int version, Action<RemoteVersionInfo, bool> callback)
        {
            Log.Debug("[*GetVersionInfoAsync*]");
            Log.Debug(string.Format("Version: {0}", version));
            Log.Debug(string.Format("Callback: {0}", callback));

            new Thread(() =>
            {
                Log.Debug("[*GetVersionInfoAsync:thread*]");
                try
                {
                    var info = GetVersionInfo(version);
                    callback(info, true);
                }
                catch(Exception exception)
                {
                    Log.Error("[GetVersionInfoAsync:thread] : exception", exception);
                    callback(default(RemoteVersionInfo), false);
                }
                Log.Debug("[-GetVersionInfoAsync:thread-]");
            }) { IsBackground = true }.Start();

            Log.Debug("[-GetVersionInfoAsync-]");
        }

        public RemoteVersionInfo[] GetAllVersionsInfo()
        {
            Log.Debug("[-*GetAllVersionsInfo*-]");

            return _remoteDatabase.GetAllVersionsInfo(new CancellationTokenSource().Token).Versions;
        }

        public void GetAllVersionsInfoAsync(Action<RemoteVersionInfo[], bool> callback)
        {
            Log.Debug("[*GetAllVersionsInfoAsync*]");

            Log.Debug(string.Format("Callback: {0}", callback));

            new Thread(() =>
            {
                Log.Debug("[*GetAllVersionsInfo:thread*]");

                try
                {
                    var info = GetAllVersionsInfo();
                    callback(info, true);
                }
                catch (Exception exception)
                {
                    Log.Error("[GetAllVersionsInfo:thread] : exception", exception);
                    callback(new RemoteVersionInfo[] {}, false);
                }
                Log.Debug("[-GetAllVersionsInfo:thread-]");
            })
            { IsBackground = true }.Start();

            Log.Debug("[-GetAllVersionsInfoAsync-]");
        }

        public void CancelPatchAsync()
        {
            Log.Debug("[*CancelPatchAsync*]");

            if (!IsPatching)
            {
                Log.Error("[-CancelPatchAsync-] : error - called while patcher was working");
                throw new InvalidOperationException("You cannot cancel patcher if it's already working.");
            }

            _cancellationTokenSource.Cancel();

            Log.Debug("[-CancelPatchAsync-]");
        }

        public void PatchAsync()
        {
            Log.Debug("[*PatchAsync*]");
            new Thread(Patch) { IsBackground = true }.Start();
            Log.Debug("[-PatchAsync-]");
        }

        public void Patch()
        {
            Log.Debug("[*Patch*]");

            if (IsPatching)
            {
                Log.Error("[-Patch-] : error - called while patcher was working");
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

                Log.Debug("[Patch] : getting current version");

                int currentlyAvailableVersion = _remoteDatabase.GetCurrentVersion(_cancellationTokenSource.Token);
                //For testing (TODO: place it under DEBUG or DEVELOPMENT #if)
                //_localDatabase.Cache.GetCommonVersion().HasValue ? _remoteDatabase.GetCurrentVersion(_cancellationTokenSource.Token) : 1;

                Log.Debug("[Patch] : choosing strategy of patching");

                if (!ApplicationFilesConsistent())
                {
                    Log.Debug("[Patch] : trying to patch with content");

                    TryPatchWithContent(currentlyAvailableVersion);
                }
                else
                {
                    Log.Debug("[Patch] : trying to patch with diff");

                    TryPatchWithDiff(currentlyAvailableVersion);
                }

                Log.Debug("[Patch] : finished patching");

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
                    Log.Debug("[Patch] : cancelled");

                    OnPatcherProgress(new PatcherProgress
                    {
                        Progress = 1.0f,
                        Type = PatcherProgressType.Canceled
                    });
                }
                else
                {
                    Log.Error("[Patch] : exception", ex);

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
                Log.Debug("[Patch] : cancelled");

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
                Log.Error("[Patch] : exception", ex);

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

            Log.Debug("[-Patch-]");
        }

        private void CreateTempDirectories()
        {
            Log.Debug("[*CreateTempDirectories*]");

            TemporaryPatchesPath = TempDirectory.Create("patches_");
            TemporaryDownloadsPath = TempDirectory.Create("downloads_");

            Log.Debug(string.Format("[CreateTempDirectories] : TemporaryPatchesPath - {0}", TemporaryPatchesPath));
            Log.Debug(string.Format("[CreateTempDirectories] : TemporaryDownloadsPath - {0}", TemporaryDownloadsPath));

            if (Downloader.TempDirectory == null)
            {
                Log.Debug("[CreateTempDirectories] : downloader temp directory empty - setting it to TemporaryDownloadsPath");

                Downloader.TempDirectory = TemporaryDownloadsPath;
            }

            Log.Debug("[-CreateTempDirectories-]");
        }

        private void RemoveTempDirectories()
        {
            Log.Debug("[*RemoveTempDirectories*]");

            Log.Debug(string.Format("[RemoveTempDirectories] : TemporaryPatchesPath - {0}", TemporaryPatchesPath));
            Log.Debug(string.Format("[RemoveTempDirectories] : TemporaryDownloadsPath - {0}", TemporaryDownloadsPath));

            Directory.Delete(TemporaryPatchesPath, true);
            Directory.Delete(TemporaryDownloadsPath, true);

            Log.Debug("[-RemoveTempDirectories-]");
        }

        private void TryPatchWithContent(int version)
        {
            Log.Debug("[*TryPatchWithContent*]");
            Log.Debug(string.Format("Version: {0}", version));

            OnPatcherProgress(new PatcherProgress
            {
                Type = PatcherProgressType.Checking,
                Progress = 0.0f
            });

            Log.Debug("[TryPatchWithContent] : checking whether version is currently patched");

            if (CheckVersion(version, true) == false)
            {
                Log.Debug("[TryPatchWithContent] : patching with content");

                _localApplication.Clear(_cancellationTokenSource.Token);

                OnPatcherProgress(new PatcherProgress
                {
                    Type = PatcherProgressType.Downloading,
                    Progress = 0.0f
                });

                Log.Debug("[TryPatchWithContent] : downloading content");

                string contentPackage = _remoteDatabase.DownloadContent(version, OnDownloaderProgress,
                    _cancellationTokenSource.Token);

                Log.Debug(string.Format("[TryPatchWithContent] : content downloaded to {0}", contentPackage));

                Log.Debug("[TryPatchWithContent] : unzipping content");

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

                Log.Debug("[TryPatchWithContent] : finished");
            }

            Log.Debug("[-TryPatchWithContent-]");
        }

        /// <summary>
        /// Tries to patch current application using diff files.
        /// </summary>
        /// <param name="targetVersion">The version that should be downloaded.</param>
        /// <returns></returns>
        private void TryPatchWithDiff(int targetVersion)
        {
            Log.Debug("[*TryPatchWithDiff*]");
            Log.Debug(string.Format("Target version: {0}", targetVersion));


            OnPatcherProgress(new PatcherProgress
            {
                Type = PatcherProgressType.Checking,
                Progress = 0.0f
            });

            float totalProgress = 0.0f;

            int localVersion = _localApplication.Cache.GetCommonVersion().Value;
            float progressPerVersion = 1.0f / (targetVersion - localVersion);

            Log.Debug(string.Format("[TryPatchWithDiff] : Local version - {0}", localVersion));

            while (!_localApplication.IsUpToDate(targetVersion))
            {
                int nextVersion = localVersion + 1;

                Log.Debug(string.Format("[TryPatchWithContent] : patching from {0} to {1}", localVersion, localVersion));

                Log.Debug("[TryPatchWithContent] : getting diff summary");

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

                Log.Debug("[TryPatchWithContent] : downloading diff package");

                string diffPackage = _remoteDatabase.DownloadDiff(nextVersion, OnDownloaderProgress,
                    _cancellationTokenSource.Token);

                Log.Debug(string.Format("[TryPatchWithContent] : downloaded diff package to {0}", diffPackage));

                Log.Debug("[TryPatchWithContent] : deleting temporary patches location");

                while (Directory.Exists(TemporaryPatchesPath))
                {
                    Directory.Delete(TemporaryPatchesPath, true);

                    Thread.Sleep(1000);
                }

                Log.Debug("[TryPatchWithContent] : unzipping diff package");

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

                ZipUtilities.Unzip(diffPackage, TemporaryPatchesPath, onDiffFileUnzipStart, onDiffFileUnzipEnd,
                    _cancellationTokenSource.Token);

                Log.Debug("[TryPatchWithContent] : patching with diff files");

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

                Log.Debug("[TryPatchWithContent] : finished patching");
            }

            Log.Debug("[-TryPatchWithDiff-]");
        }

        private bool ApplicationFilesConsistent()
        {
            Log.Debug("[*ApplicationFilesConsistent*]");

            Log.Debug("[ApplicationFilesConsistent] : getting common version of files");

            int? commonVersion = _localApplication.Cache.GetCommonVersion();
            if (!commonVersion.HasValue)
            {
                Log.Debug("[-ApplicationFilesConsistent-] : false - there's no common version");

                return false;
            }

            Log.Debug("[ApplicationFilesConsistent] : getting content summary");

            RemoteContentSummary contentSummary =
                _remoteDatabase.GetContentSummary(commonVersion.Value, _cancellationTokenSource.Token);

            if (!_localApplication.CheckFilesConsistency(contentSummary, _cancellationTokenSource.Token))
            {
                Log.Debug("[-ApplicationFilesConsistent-] : false - files aren't consisent");

                return false;
            }

            Log.Debug("[-ApplicationFilesConsistent-] : true");

            return true;
        }

        private bool CheckFile(string file, int version)
        {
            Log.Debug("[*CheckFile*]");
            Log.Debug(string.Format("File: {0}", file));
            Log.Debug(string.Format("Version: {0}", version));

            var localVersion = _localApplication.Cache.GetFileVersion(file);

            if (localVersion != null && localVersion == version)
            {
                if (File.Exists(_localApplication.GetFilePath(file)))
                {
                    Log.Debug("[-CheckFile-] : true");

                    return true;
                }

                Log.Debug("[-CheckFile-] : false - file doesn't exist");

                return false;
            }

            Log.Debug("[-CheckFile-] : false - file doesn't exist or is in different version");

            return false;
        }

        private bool CheckFileWithHash(string file, int version, string hash)
        {
            Log.Debug("[*CheckFileWithHash*]");
            Log.Debug(string.Format("File: {0}", file));
            Log.Debug(string.Format("Version: {0}", version));
            Log.Debug(string.Format("Hash: {0}", hash));

            if (CheckFile(file, version))
            {
                string fileHash = HashUtilities.ComputeFileHash(_localApplication.GetFilePath(file));

                if (hash == fileHash)
                {
                    Log.Debug("[-CheckFileWithHash-] : true");

                    return true;
                }

                Log.Debug("[-CheckFileWithHash-] : false - different hashes");

                return false;
            }

            Log.Debug("[-CheckFileWithHash-] : false - basic check failed");

            return false;
        }

        private bool CheckVersion(int version, bool checkHash)
        {
            Log.Debug("[*CheckVersion*]");
            Log.Debug(string.Format("Version: {0}", version));
            Log.Debug(string.Format("Check hash: {0}", checkHash));

            if (version <= _remoteDatabase.GetCurrentVersion(_cancellationTokenSource.Token) && _localApplication.Cache.GetCommonVersion() == version)
            {
                Log.Debug("[CheckVersion] : getting content summary");

                var contentSummary = _remoteDatabase.GetContentSummary(version, _cancellationTokenSource.Token);

                Log.Debug("[CheckVersion] : checking files");

                foreach (var file in contentSummary.Files)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (checkHash ? !CheckFileWithHash(file.Path, version, file.Hash) : !CheckFile(file.Path, version))
                    {
                        Log.Debug(string.Format("[-CheckVersion-] : false - incorrect file of path {0}", file.Path));

                        return false;
                    }
                }

                Log.Debug("[-CheckVersion-] : true");

                return true;
            }

            Log.Debug("[-CheckVersion-] : false - incorrect version");

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
