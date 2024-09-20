using UnityEngine;
using YooAsset;
using Cysharp.Threading.Tasks;

namespace Framework.Core
{
    public class ResourceModule : IGameModule
    {
        public string Name => "Resource";

        private ResourcePackage package;

        public void OnInit()
        {
            // 初始化 YooAsset
            YooAssets.Initialize();
            package = YooAssets.CreatePackage("DefaultPackage");
            YooAssets.SetDefaultPackage(package);
        }

        public async UniTask InitializeAsync(string packageVersion)
        {
            // // 设置资源包的版本信息
            // var operation = package.UpdatePackageVersionAsync(packageVersion);
            // await operation;

            // // 更新资源清单
            // var manifestOperation = package.UpdateManifestAsync(packageVersion);
            // await manifestOperation;

            // // 设置下载器
            // var downloader = YooAssets.CreateResourceDownloader(10, 3);
            // await downloader.DownloadAsync().ToUniTask();
        }

        public void OnStart() { }

        public void OnUpdate() { }

        public void OnDestroy()
        {
            // 卸载所有资源
            // package.UnloadUnusedAssets();
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            var handle = package.LoadAssetAsync<T>(assetName);
            await handle.ToUniTask();
            return handle.AssetObject as T;
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            // package.UnloadAsset(asset);
        }

        // 可以根据需要添加更多方法，如加载场景、预加载等
    }
}