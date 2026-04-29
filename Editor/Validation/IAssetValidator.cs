using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IAssetValidator {
        /// <summary>
        /// Same as <see cref="Assert.Fail(string)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="assertion"></param>
        /// <param name="message"></param>
        void AssertFail(string message);

        /// <summary>
        /// Same as <see cref="Assert.IsTrue(bool, string)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="assertion"></param>
        /// <param name="message"></param>
        void AssertTrue(bool assertion, string message);

        /// <summary>
        /// Same as <see cref="Assert.IsFalse(bool, string)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="assertion"></param>
        /// <param name="message"></param>
        void AssertFalse(bool assertion, string message);

        /// <summary>
        /// Same as <see cref="Assert.That{TActual}(TActual, IResolveConstraint)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="constraint"></param>
        void AssertThat(object actual, IResolveConstraint constraint);

        /// <summary>
        /// Same as <see cref="Assert.That{TActual}(TActual, IResolveConstraint, string, object[])"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="constraint"></param>
        void AssertThat(object actual, IResolveConstraint constraint, string message);

        /// <summary>
        /// Attempt to retrieve a component of type <typeparamref name="T"/> from <paramref name="obj"/>, and fail on success.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        void AssertDoesNotHaveComponent<T>(GameObject obj) where T : class;

        /// <summary>
        /// Attempt to retrieve a component of type <typeparamref name="T"/> from <paramref name="obj"/>, and fail on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>True if the component was found.</returns>
        bool AssertHasComponent<T>(GameObject obj) where T : class;

        /// <summary>
        /// Attempt to retrieve a component of type <typeparamref name="T"/> from <paramref name="obj"/>, and fail on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>True if the component was found and written to <paramref name="component"/>.</returns>
        bool AssertHasComponent<T>(GameObject obj, out T component) where T : class;

        /// <summary>
        /// Verify that <paramref name="assetPath"/> is part of the dependency hierarchy of the current package. Print <paramref name="message"/> if not.
        /// </summary>
        /// <param name="asset"></param>
        void AssertAssetPath(string assetPath, string message);

        /// <summary>
        /// Perform a full validation of <paramref name="asset"/>, using all applicable <see cref="ValidateAttribute"/>.
        /// </summary>
        /// <param name="asset"></param>
        void ValidateAsset(UnityObject asset);

        /// <summary>
        /// Perform a full validation of the asset at <paramref name="assetPath"/>, using all applicable <see cref="ValidateAttribute"/>.
        /// </summary>
        /// <param name="asset"></param>
        void ValidateAsset(string assetPath);

        /// <summary>
        /// The asset currently processed by <see cref="ValidateAsset(UnityObject)"/>.
        /// </summary>
        UnityObject currentAsset { get; }

        /// <summary>
        /// The asset path of the asset currently processed by <see cref="ValidateAsset(UnityObject)"/>, or null if the current asset has not been saved to disk.
        /// </summary>
        string currentAssetPath { get; }

        /// <summary>
        /// The latest scene loaded via <see cref="OpenScene(string)"/>.
        /// </summary>
        Scene currentScene { get; }

        /// <summary>
        /// Create a human-readable name for <paramref name="target"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        string GetName(UnityObject target);

        /// <summary>
        /// Determine whether or not <see cref="OpenScene(string)"/> will succeed.
        /// </summary>
        /// <param name="scenePath"></param>
        bool CanOpenScene(string scenePath);

        /// <summary>
        /// Load the scene at <paramref name="scenePath"/> and set <see cref="currentScene"/>.
        /// </summary>
        /// <param name="scenePath"></param>
        void OpenScene(string scenePath);

        /// <summary>
        /// Close <see cref="currentScene"/>.
        /// </summary>
        void CloseScene();
    }
}