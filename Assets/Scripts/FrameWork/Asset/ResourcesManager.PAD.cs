#if UNITY_ANDROID && ENABLE_ASSET_DELIVERY
//using Google.Play.AssetDelivery;
//using System.IO;

namespace DragonU3DSDK.Asset
{
    public partial class ResourcesManager
    {
        public const string INSTALL_TIME_ASSETPACK_NAME = "InstallTimeAssets";
        public const string ASSET_PACK_NAME = "assetpack";

        //PlayAssetPackRequest _installTimeAssetPackRequests;
        //PlayAssetPackRequest installTimeAssetPackRequests
        //{
        //    get
        //    {
        //        if (_installTimeAssetPackRequests == null)
        //        {
        //            _installTimeAssetPackRequests = PlayAssetDelivery.RetrieveAssetPackAsync(INSTALL_TIME_ASSETPACK_NAME);
        //        }
        //        return _installTimeAssetPackRequests;
        //    }
        //}

        //string GetAssetUrl(PlayAssetPackRequest assetPackRequest, string assetPath)
        //{
        //    AssetLocation location = assetPackRequest.GetAssetLocation(assetPath);
        //    return string.Format("{0}{1}{2}{3}", "jar:file://", location.Path, "!/assets/assetpack/", assetPath);
        //}

        //public string GetInstallTimeAssetUrl(string assetPath)
        //{
        //    return GetAssetUrl(installTimeAssetPackRequests, assetPath);
        //}

        //byte[] ReadAsset(PlayAssetPackRequest assetPackRequest, string assetPath)
        //{
        //    AssetLocation location = assetPackRequest.GetAssetLocation(assetPath);
        //    using (var fileStream = File.OpenRead(location.Path))
        //    {
        //        byte[] buffer = new byte[location.Size];
        //        fileStream.Seek((long)location.Offset, SeekOrigin.Begin);
        //        fileStream.Read(buffer, 0, buffer.Length);
        //        return buffer;
        //    }
        //}

        //public byte[] ReadInstallTimeAsset(string assetPath)
        //{
        //    return ReadAsset(installTimeAssetPackRequests, assetPath);
        //}
    }
}
#endif