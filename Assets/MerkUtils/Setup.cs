using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor.PackageManager;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;

namespace Merkab
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("_Project", "_Scripts", "Animation", "Art", "Materials", "Prefabs", "Resources", "ScriptableObjects", "Settings");
            Refresh();
        }

        //[MenuItem("Tools/Setup/Import My Favorite Assets")]
        public static void ImportMyFavoriteAssets()
        {
            Assets.ImportAssets("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
        }

        //[MenuItem("Tools/Setup/Import Basics")]
        public static void ImportBasics()
        {
            // You know what to do
        }

        //[MenuItem("Tools/Setup/Install Unity AI Navigation")]
        public static void InstallUnityAINavigation()
        {
            Packages.InstallPackages(new[] {
                "com.unity.ai.navigation"
            });
        }

        //[MenuItem("Tools/Setup/Install My Favorite Open Source")]
        public static void InstallOpenSource()
        {
            Packages.InstallPackages(new[] {
                "git+https://github.com/KyleBanks/scene-ref-attribute",
                "git+https://github.com/starikcetin/Eflatun.SceneReference.git#3.1.1"
            });
        }

        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullpath = Combine(Application.dataPath, root);
                if (!Exists(fullpath))
                {
                    CreateDirectory(fullpath);
                }
                foreach (var folder in folders)
                {
                    CreateSubFolders(fullpath, folder);
                }
            }

            private static void CreateSubFolders(string rootPath, string folderHierarchy)
            {
                var folders = folderHierarchy.Split('/');
                var currentPath = rootPath;
                foreach (var folder in folders)
                {
                    currentPath = Combine(currentPath, folder);
                    if (!Exists(currentPath))
                    {
                        CreateDirectory(currentPath);
                    }
                }
            }
        }

        static class Packages
        {
            static AddRequest Request;
            static Queue<string> PackagesToInstall = new();

            public static void InstallPackages(string[] packages)
            {
                foreach (var package in packages)
                {
                    PackagesToInstall.Enqueue(package);
                }

                // Start the installation of the first package
                if (PackagesToInstall.Count > 0)
                {
                    Request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }

            static async void Progress()
            {
                if (Request.IsCompleted)
                {
                    if (Request.Status == StatusCode.Success)
                        Debug.Log("Installed: " + Request.Result.packageId);
                    else if (Request.Status >= StatusCode.Failure)
                        Debug.Log(Request.Error.message);

                    EditorApplication.update -= Progress;

                    // If there are more packages to install, start the next one
                    if (PackagesToInstall.Count > 0)
                    {
                        // Add delay before next package install
                        await Task.Delay(1000);
                        Request = Client.Add(PackagesToInstall.Dequeue());
                        EditorApplication.update += Progress;
                    }
                }
            }
        }
    }

    public static class Assets
    {
        public static void ImportAssets(string asset, string subfolder, string folder = "C:/Users/M3rka/AppData/Roaming/Unity/Asset Store-5.x")
        {
            ImportPackage(Combine(folder, subfolder, asset), false);
        }
    }
}