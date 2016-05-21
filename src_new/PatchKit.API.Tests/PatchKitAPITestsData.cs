using PatchKit.API.Data;

namespace PatchKit.API.Tests
{
    public class PatchKitAPITestsData
    {
        public PatchKitAPISettings Settings;

        public AppVersion[] AppVersions;

        public AppContentSummary[] AppContentSummaries;

        public AppContentTorrentUrl[] AppContentTorrentUrls;

        public AppContentUrl[][] AppContentUrls;

        public AppDiffSummary[] AppDiffSummaries;

        public AppDiffTorrentUrl[] AppDiffTorrentUrls;

        public AppDiffUrl[][] AppDiffUrls;

        public AppVersion AppLatestVersion
        {
            get { return AppVersions[AppVersions.Length - 1]; }
        }

        public AppLatestVersionID AppLatestVersionID
        {
            get
            {
                return new AppLatestVersionID
                {
                    ID = AppLatestVersion.ID
                };
            }
        }

        public static readonly PatchKitAPITestsData Default = new PatchKitAPITestsData
        {
            Settings = new PatchKitAPISettings("874e2b343034cdc6e1aa6c02340bd469", "http://test.patchkit.net:8080", new[]
            {
                "http://test.patchkit.net:43231"
            }),
            AppVersions = new[]
            {
                new AppVersion
                {
                    Changelog = "Changelog1",
                    ContentGuid = "c319cfa3-2f0d-4c6a-8ca0-30fc48550bcf",
                    DiffGuid = null,
                    ID = 1,
                    Label = "1.0",
                    PublishTime = 1463258360
                },
                new AppVersion
                {
                    Changelog = "Changelog2",
                    ContentGuid = "4e7750e5-73a6-4a68-9e83-9c105f4143a7",
                    DiffGuid = "32f46463-cb77-4322-b142-d830192ad79a",
                    ID = 2,
                    Label = "1.23f",
                    PublishTime = -1
                }
            },
            AppContentSummaries = new[]
            {
                new AppContentSummary
                {
                    CompressionMethod = "zip",
                    EncryptionMethod = "none",
                    Files = new[]
                    {
                        new AppContentFile
                        {
                            Hash = "6d2a109a",
                            Path = "game_Data/mainData"
                        },
                        new AppContentFile
                        {
                            Hash = "533a7e86",
                            Path = "game_Data/Managed/mscorlib.dll"
                        },
                        new AppContentFile
                        {
                            Hash = "b8f26a20",
                            Path = "game_Data/Managed/UnityEngine.dll"
                        },
                        new AppContentFile
                        {
                            Hash = "a708031e",
                            Path = "game_Data/Mono/etc/mono/1.0/DefaultWsdlHelpGenerator.aspx"
                        },
                        new AppContentFile
                        {
                            Hash = "9c7400fd",
                            Path = "game_Data/Mono/etc/mono/1.0/machine.config"
                        },
                        new AppContentFile
                        {
                            Hash = "860053fd",
                            Path = "game_Data/Mono/etc/mono/2.0/Browsers/Compat.browser"
                        },
                        new AppContentFile
                        {
                            Hash = "32f85654",
                            Path = "game_Data/Mono/etc/mono/2.0/DefaultWsdlHelpGenerator.aspx"
                        },
                        new AppContentFile
                        {
                            Hash = "ba624663",
                            Path = "game_Data/Mono/etc/mono/2.0/machine.config"
                        },
                        new AppContentFile
                        {
                            Hash = "f9c5f704",
                            Path = "game_Data/Mono/etc/mono/2.0/settings.map"
                        },
                        new AppContentFile
                        {
                            Hash = "57bd3370",
                            Path = "game_Data/Mono/etc/mono/2.0/web.config"
                        },
                        new AppContentFile
                        {
                            Hash = "17a9443c",
                            Path = "game_Data/Mono/etc/mono/browscap.ini"
                        },
                        new AppContentFile
                        {
                            Hash = "92844ff6",
                            Path = "game_Data/Mono/etc/mono/config"
                        },
                        new AppContentFile
                        {
                            Hash = "f9bbcca1",
                            Path = "game_Data/Mono/etc/mono/mconfig/config.xml"
                        },
                        new AppContentFile
                        {
                            Hash = "a667a7cb",
                            Path = "game_Data/Mono/mono.dll"
                        },
                        new AppContentFile
                        {
                            Hash = "b8d250b5",
                            Path = "game_Data/PlayerConnectionConfigFile"
                        },
                        new AppContentFile
                        {
                            Hash = "ebe23b63",
                            Path = "game_Data/Resources/unity default resources"
                        },
                        new AppContentFile
                        {
                            Hash = "e6f5133b",
                            Path = "game_Data/Resources/unity_builtin_extra"
                        },
                        new AppContentFile
                        {
                            Hash = "54e96fc",
                            Path = "game_Data/sharedassets0.assets"
                        },
                        new AppContentFile
                        {
                            Hash = "a7f78260",
                            Path = "game.exe"
                        }
                    },
                    Size = 8994434
                },
                new AppContentSummary
                {
                    CompressionMethod = "zip",
                    EncryptionMethod = "none",
                    Files = new[]
                    {
                        new AppContentFile
                        {
                            Hash = "c2577801",
                            Path = "game_Data/mainData"
                        },
                        new AppContentFile
                        {
                            Hash = "533a7e86",
                            Path = "game_Data/Managed/mscorlib.dll"
                        },
                        new AppContentFile
                        {
                            Hash = "b8f26a20",
                            Path = "game_Data/Managed/UnityEngine.dll"
                        },
                        new AppContentFile
                        {
                            Hash = "a708031e",
                            Path = "game_Data/Mono/etc/mono/1.0/DefaultWsdlHelpGenerator.aspx"
                        },
                        new AppContentFile
                        {
                            Hash = "9c7400fd",
                            Path = "game_Data/Mono/etc/mono/1.0/machine.config"
                        },
                        new AppContentFile
                        {
                            Hash = "860053fd",
                            Path = "game_Data/Mono/etc/mono/2.0/Browsers/Compat.browser"
                        },
                        new AppContentFile
                        {
                            Hash = "32f85654",
                            Path = "game_Data/Mono/etc/mono/2.0/DefaultWsdlHelpGenerator.aspx"
                        },
                        new AppContentFile
                        {
                            Hash = "ba624663",
                            Path = "game_Data/Mono/etc/mono/2.0/machine.config"
                        },
                        new AppContentFile
                        {
                            Hash = "f9c5f704",
                            Path = "game_Data/Mono/etc/mono/2.0/settings.map"
                        },
                        new AppContentFile
                        {
                            Hash = "57bd3370",
                            Path = "game_Data/Mono/etc/mono/2.0/web.config"
                        },
                        new AppContentFile
                        {
                            Hash = "17a9443c",
                            Path = "game_Data/Mono/etc/mono/browscap.ini"
                        },
                        new AppContentFile
                        {
                            Hash = "92844ff6",
                            Path = "game_Data/Mono/etc/mono/config"
                        },
                        new AppContentFile
                        {
                            Hash = "f9bbcca1",
                            Path = "game_Data/Mono/etc/mono/mconfig/config.xml"
                        },
                        new AppContentFile
                        {
                            Hash = "a667a7cb",
                            Path = "game_Data/Mono/mono.dll"
                        },
                        new AppContentFile
                        {
                            Hash = "b8d250b5",
                            Path = "game_Data/PlayerConnectionConfigFile"
                        },
                        new AppContentFile
                        {
                            Hash = "ebe23b63",
                            Path = "game_Data/Resources/unity default resources"
                        },
                        new AppContentFile
                        {
                            Hash = "e6f5133b",
                            Path = "game_Data/Resources/unity_builtin_extra"
                        },
                        new AppContentFile
                        {
                            Hash = "68d893db",
                            Path = "game_Data/sharedassets0.assets"
                        },
                        new AppContentFile
                        {
                            Hash = "a7f78260",
                            Path = "game.exe"
                        }
                    },
                    Size = 8709867
                }
            },
            AppContentTorrentUrls = new[]
            {
                new AppContentTorrentUrl
                {
                    Url = @"http://patchkit.net:9292/fake_bucket/download?path=/patchkit-client-data-dev/users/mietek/apps/874e2b343034cdc6e1aa6c02340bd469/versions/1/content.torrent"
                },
                new AppContentTorrentUrl
                {
                    Url = @"http://patchkit.net:9292/fake_bucket/download?path=/patchkit-client-data-dev/users/mietek/apps/874e2b343034cdc6e1aa6c02340bd469/versions/2/content.torrent"
                }
            },
            AppContentUrls = new[]
            {
                new []
                {
                    new AppContentUrl
                    {
                        Url = @"http://t-p2p-node-2.patchkit.net:43210/download/c319cfa3-2f0d-4c6a-8ca0-30fc48550bcf"
                    }
                },
                new AppContentUrl[0]
            },
            // TODO: Fill data with diff summary from server.
            AppDiffSummaries = new []
            {
                new AppDiffSummary
                {
                    Size = 347866, 
                    EncryptionMethod = "none",
                    CompressionMethod = "zip",
                    AddedFiles = new []
                    {
                        "game_Data/"
                    }
                }
            },
            AppDiffTorrentUrls = new []
            {
                new AppDiffTorrentUrl
                {
                    Url = @"http://patchkit.net:9292/fake_bucket/download?path=/patchkit-client-data-dev/users/mietek/apps/874e2b343034cdc6e1aa6c02340bd469/versions/2/diff.torrent"
                }
            },
            AppDiffUrls = new []
            {
                new[]
                {
                    new AppDiffUrl
                    {
                        Url = @"http://t-p2p-node-2.patchkit.net:43210/download/32f46463-cb77-4322-b142-d830192ad79a"
                    }
                }
            }
        };
    }
}
