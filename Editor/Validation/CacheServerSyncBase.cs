using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using UnityEditor;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Editor.Validation {
    [TestMustExpectAllLogs(false)]
    public abstract class CacheServerSyncBase<T> where T : IAssetSource, new() {

        [UnityTest]
        public IEnumerator UploadAssetsToCacheServer() {
            if (!AssetDatabase.IsConnectedToCacheServer()) {
                Assert.Pass("Not connected to a cache server, skipping upload.");
                yield break;
            }

            yield return RequestAndWait_Co(UploadArtifactsToCacheServer);
        }

        [UnityTest]
        public IEnumerator UploadShadersToCacheServer() {
            if (!AssetDatabase.IsConnectedToCacheServer()) {
                Assert.Pass("Not connected to a cache server, skipping upload.");
                yield break;
            }

            if (AssetUtils.hasUploadedShadersRecently) {
                Assert.Ignore("Uploaded shaders recently, skipping upload.");
                yield break;
            }

            AssetUtils.hasUploadedShadersRecently = true;

            yield return RequestAndWait_Co(CacheServer.UploadShaderCache);
        }

        static IEnumerator RequestAndWait_Co(Action request) {
            while (Progress.GetCount() > 0) {
                yield return null;
            }

            request();

            while (Progress.GetCount() > 0) {
                yield return null;
            }
        }

        static void UploadArtifactsToCacheServer() {
            var assets = new T()
                .GetAssetPaths()
                .Select(AssetDatabase.AssetPathToGUID)
                .Select(guid => new GUID(guid))
                .ToArray();

            if (assets.Length > 0) {
                CacheServer.UploadArtifacts(assets, uploadAllRevisions: true);
            }
        }
    }
}
