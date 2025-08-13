using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    static class AssetUtils {
        const string ASSET_IS_WIP = "WIP";
        const string ASSET_IS_DEPRECATED = "DEPRECATED";

        internal static bool hasUploadedShadersRecently = false;

        /// <summary>
        /// Determines whether an asset is a "work in progress".
        /// Deprecated assets SHOULD not be included in the game build.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsWIPAsset(string assetPath) {
            return assetPath.Contains(ASSET_IS_WIP, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether an asset is a test asset.
        /// Test assets SHOULD not be included in the game build.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsTestAsset(string assetPath) {
            return assetPath.Contains("/Tests/", StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether an asset is deprecated.
        /// Deprecated assets SHOULD not be referenced by other assets.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsDeprecatedAsset(string assetPath) {
            return assetPath.Contains(ASSET_IS_DEPRECATED, StringComparison.Ordinal);
        }
        internal static ContainsConstraint IsNotDeprecatedAssetConstraint => Does.Not.Contain(ASSET_IS_DEPRECATED);

        static bool ShouldIncludeAssetInDirectory(string assetPath) {
            if (assetPath.Contains("/.")) {
                // Unity ignores files and folders starting with '.', so we should, too.
                return false;
            }

            if (assetPath.Contains("~/")) {
                // Unity ignores folders ending with '~', so we should, too.
                return false;
            }

            if (assetPath.Contains(".bundle/")) {
                // Unity treats folders ending with '.bundle' as MacOS assets, so we should ignore them.
                return false;
            }

            if (assetPath.Contains("InitTestScene")) {
                // temp scene created by Unity's Test Runner
                return false;
            }

            return true;
        }

        static bool TryGetCleanAssetPath(ref string assetPath) {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) {
                return false;
            }

            assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return !string.IsNullOrEmpty(assetPath);
        }

        static readonly Regex isLibraryPath = new("Library/PackageCache/([^/]+)@[^/]+/");

        /// <summary>
        /// filePath => assetPath[]
        /// </summary>
        static readonly Dictionary<string, List<string>> assetsInDirectoryCache = new();

        public static IEnumerable<string> AssetsInDirectory(this string directoryPath) {
            if (!assetsInDirectoryCache.TryGetValue(directoryPath, out var assets)) {
                assetsInDirectoryCache[directoryPath] = assets = new();

                DirectoryInfo directory = new(directoryPath);
                string projectRoot = directory.FullName;

                SortedSet<string> allAssetPaths = new(StringComparer.InvariantCultureIgnoreCase);
                foreach (var file in directory.EnumerateFiles("*.meta", SearchOption.AllDirectories)) {
                    string path = file.FullName;

                    path = directoryPath + path[projectRoot.Length..^".meta".Length];

                    path = path.Replace('\\', '/');

                    if (isLibraryPath.Match(path) is { Success: true, Groups: GroupCollection groups, Value: string match }) {
                        path = path.Replace(match, $"Packages/{groups[1].Value}/");
                    }

                    if (ShouldIncludeAssetInDirectory(path) && TryGetCleanAssetPath(ref path)) {
                        assets.Add(path);
                    }
                }
            }

            return assets;
        }

        public static IEnumerable<string> ExcludeTestAssets(this IEnumerable<string> assetPaths) {
            return assetPaths.Where(assetPath => !IsTestAsset(assetPath));
        }

        public static IEnumerable<string> ExcludeWIPAssets(this IEnumerable<string> assetPaths) {
            return assetPaths.Where(assetPath => !IsWIPAsset(assetPath));
        }

        public static IEnumerable<string> ExcludeDeprecatedAssets(this IEnumerable<string> assetPaths) {
            return assetPaths.Where(assetPath => !IsDeprecatedAsset(assetPath));
        }

        public static IEnumerable<string> OnlyWIPAssets(this IEnumerable<string> assetPaths) {
            return assetPaths.Where(IsWIPAsset);
        }

        /// <summary>
        /// assetPath => assetPath[]
        /// </summary>
        static readonly Dictionary<string, string[]> dependentAssetsCache = new();

        /// <summary>
        /// assetPath => assetPath[]
        /// </summary>
        static readonly Dictionary<string, string[]> dependentAssetsRecursiveCache = new();

        public static string[] DependentAssets(string assetPath, bool recursive) {
            var cache = recursive
                ? dependentAssetsRecursiveCache
                : dependentAssetsCache;

            if (!cache.TryGetValue(assetPath, out string[] assets)) {
                cache[assetPath] = assets = AssetDatabase.GetDependencies(assetPath, recursive);
            }

            return assets;
        }

        public static IEnumerable<string> DependentAssets(this IEnumerable<string> assetPaths, bool recursive = true, bool excludeSource = false) {
            var sourcePaths = assetPaths.ToList();
            var dependentPaths = sourcePaths.SelectMany(assetPath => DependentAssets(assetPath, recursive));

            return excludeSource
                ? dependentPaths.Where(assetPath => !sourcePaths.Contains(assetPath))
                : dependentPaths;
        }

        public static IEnumerable<string> DependingAssets(string assetPath) {
            foreach (string path in AssetDatabase.GetAllAssetPaths()) {
                if (DependentAssets(path, false).Contains(assetPath)) {
                    yield return path;
                }
            }
        }

        public static IEnumerable<string> DependingAssets(this IEnumerable<string> assetPaths) {
            return assetPaths.SelectMany(DependingAssets);
        }

        internal static void ClearCache(IEnumerable<string> assetPaths) {
            assetsInDirectoryCache.Clear();
            searchedAssetsCache.Clear();
            loadedAssetsCache.Clear();
            loadedPrefabsCache.Clear();

            foreach (string assetPath in assetPaths) {
                RemoveWhereKeyOrContains(dependentAssetsCache, assetPath);
                RemoveWhereKeyOrContains(dependentAssetsRecursiveCache, assetPath);
            }
        }

        static void RemoveWhereKeyOrContains(IDictionary<string, string[]> pathCache, string assetPath) {
            pathCache.Remove(assetPath);

            foreach (string path in pathCache.Where(entry => entry.Value.Contains(assetPath)).Select(entry => entry.Key).ToList()) {
                pathCache.Remove(path);
            }
        }

        /// <summary>
        /// Type extending UnityObject => FindAssets search query
        /// </summary>
        static readonly Dictionary<Type, string> searchedAssetsCache = new();

        internal static string GetSearchCache<T>() where T : class {
            return searchedAssetsCache.TryGetValue(typeof(T), out string search)
                ? search
                : searchedAssetsCache[typeof(T)] = string.Join(',', TypeCache
                    .GetTypesDerivedFrom<T>()
                    .Append(typeof(T))
                    .Where(typeof(UnityObject).IsAssignableFrom)
                    .FindCommonAncestors()
                    .Select(t => "t:" + t.Name)
                );
        }

        static IEnumerable<Type> FindCommonAncestors(this IEnumerable<Type> types) {
            List<Type> result = new(types);
            return result.Where(t1 => !result.Any(t2 => t1 != t2 && t2.IsAssignableFrom(t1)));
        }

        /// <summary>
        /// Find all assets in the project that are of type <typeparamref name="T"/> and returns their GUIDs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] FindAssetsOfType<T>() where T : class {
            string search = GetSearchCache<T>();

            return string.IsNullOrEmpty(search)
                ? Array.Empty<string>()
                : AssetDatabase.FindAssets(search);
        }

        /// <summary>
        /// Type extending UnityObject => UnityObject[]
        /// </summary>
        static readonly Dictionary<Type, ICollection> loadedAssetsCache = new();

        /// <summary>
        /// Find all assets in the project that are of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IReadOnlyList<T> FindAndLoadAssetsOfType<T>() where T : class {
            return (
                loadedAssetsCache.TryGetValue(typeof(T), out var result)
                    ? result
                    : loadedAssetsCache[typeof(T)] = FindAssetsOfType<T>()
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                        .OfType<T>()
                        .ToList()
            ) as IReadOnlyList<T>;
        }

        /// <summary>
        /// Find all prefabs in the project that have a component of type <typeparamref name="T"/>.
        /// Only the root <see cref="GameObject"/> is considered for the component, childs are not.
        /// If a prefab contains multiple components of type <typeparamref name="T"/>, it will be returned multiple times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IReadOnlyList<(GameObject prefab, T component)> FindAndLoadPrefabsWithComponent<T>() where T : class {
            if (!loadedPrefabsCache.TryGetValue(typeof(T), out var result)) {
                result = LoadPrefabsWithComponent<T>();
            }

            return result as IReadOnlyList<(GameObject prefab, T component)>;
        }

        /// <summary>
        /// Type extending Component => (Prefab, Component)[]
        /// </summary>
        static readonly Dictionary<Type, ICollection> loadedPrefabsCache = new();

        static ICollection LoadPrefabsWithComponent<T>() where T : class {
#pragma warning disable UNT0014 // Invalid type for call to GetComponent
            return loadedPrefabsCache[typeof(T)] = AssetDatabase
                .FindAssets("t:GameObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                .SelectMany(prefab => prefab.GetComponents<T>().Select(component => (prefab, component)))
                .ToList();
#pragma warning restore UNT0014 // Invalid type for call to GetComponent
        }
    }
}