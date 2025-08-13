using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using Slothsoft.TestRunner.Editor.Validation.Validators;
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
        public void AssertFail(string message);

        /// <summary>
        /// Same as <see cref="Assert.IsTrue(bool, string)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="assertion"></param>
        /// <param name="message"></param>
        public void AssertTrue(bool assertion, string message);

        /// <summary>
        /// Same as <see cref="Assert.IsFalse(bool, string)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="assertion"></param>
        /// <param name="message"></param>
        public void AssertFalse(bool assertion, string message);

        /// <summary>
        /// Same as <see cref="Assert.That{TActual}(TActual, IResolveConstraint)"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="constraint"></param>
        public void AssertThat(object actual, IResolveConstraint constraint);

        /// <summary>
        /// Same as <see cref="Assert.That{TActual}(TActual, IResolveConstraint, string, object[])"/>, but appends the message to the list of errors instead of aborting immediately.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="constraint"></param>
        public void AssertThat(object actual, IResolveConstraint constraint, string message);

        /// <summary>
        /// Attempt to retrieve a component of type <typeparamref name="T"/> from <paramref name="obj"/>, and fail on success.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void AssertDoesNotHaveComponent<T>(GameObject obj) where T : class;

        /// <summary>
        /// Attempt to retrieve a component of type <typeparamref name="T"/> from <paramref name="obj"/>, and fail on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>True if the component was found.</returns>
        public bool AssertHasComponent<T>(GameObject obj) where T : class;

        /// <summary>
        /// Attempt to retrieve a component of type <typeparamref name="T"/> from <paramref name="obj"/>, and fail on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>True if the component was found and written to <paramref name="component"/>.</returns>
        public bool AssertHasComponent<T>(GameObject obj, out T component) where T : class;

        /// <summary>
        /// Verify that <paramref name="assetPath"/> is part of the dependency hierarchy of the current package. Print <paramref name="message"/> if not.
        /// </summary>
        /// <param name="asset"></param>
        public void AssertAssetPath(string assetPath, string message);

        /// <summary>
        /// Perform a full validation of <paramref name="asset"/>, using all applicable <see cref="ValidateAttribute"/>.
        /// </summary>
        /// <param name="asset"></param>
        public void ValidateAsset(UnityObject asset);

        /// <summary>
        /// Perform a full validation of the asset at <paramref name="assetPath"/>, using all applicable <see cref="ValidateAttribute"/>.
        /// </summary>
        /// <param name="asset"></param>
        public void ValidateAsset(string assetPath);

        /// <summary>
        /// The asset currently processed by <see cref="ValidateAsset(UnityObject)"/>.
        /// </summary>
        public UnityObject CurrentAsset { get; }

        /// <summary>
        /// The asset path of the asset currently processed by <see cref="ValidateAsset(UnityObject)"/>, or null if the current asset has not been saved to disk.
        /// </summary>
        public string CurrentAssetPath { get; }

        /// <summary>
        /// Whether or not the asset currently processed by <see cref="ValidateAsset(UnityObject)"/> resides in a "Tests" folder.
        /// </summary>
        public bool CurrentAssetIsTestAsset { get; }

        /// <summary>
        /// Create a human-readable name for <paramref name="target"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public string GetName(UnityObject target);

        /// <summary>
        /// The latest scene loaded via <see cref="OpenScene(string)"/>.
        /// </summary>
        public Scene CurrentScene { get; }

        /// <summary>
        /// Determine whether or not <see cref="OpenScene(string)"/> will succeed.
        /// </summary>
        /// <param name="scenePath"></param>
        public bool CanOpenScene(string scenePath);

        /// <summary>
        /// Load the scene at <paramref name="scenePath"/> and set <see cref="CurrentScene"/>.
        /// </summary>
        /// <param name="scenePath"></param>
        public void OpenScene(string scenePath);

        /// <summary>
        /// Close <see cref="CurrentScene"/>.
        /// </summary>
        public void CloseScene();
    }
}