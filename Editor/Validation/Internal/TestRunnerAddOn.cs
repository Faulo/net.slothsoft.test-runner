using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TestTools.TestRunner;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
#if UNITY_INCLUDE_TESTS
    static class TestRunnerAddOn {
        class AddOn {
            const string GUI_CLASS = "TestListGUI";
            static readonly Type t_TestListGUI = typeof(TestRunnerWindow)
                .Assembly
                .GetTypes()
                .First(t => t.Name == GUI_CLASS);

            const string TREE_CLASS = "TreeViewController";
            static readonly Type t_TreeViewController = typeof(TreeView)
                .Assembly
                .GetTypes()
                .First(t => t.Name == TREE_CLASS);

            readonly TestRunnerWindow window;

            static readonly FieldInfo i_SelectedTestTypes = typeof(TestRunnerWindow)
                .GetField(nameof(m_SelectedTestTypes), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new EntryPointNotFoundException(nameof(m_SelectedTestTypes));
            object m_SelectedTestTypes;

            static readonly FieldInfo i_TestListTree = t_TestListGUI
                .GetField(nameof(m_TestListTree), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new EntryPointNotFoundException(nameof(m_TestListTree));
            object m_TestListTree;

            static readonly PropertyInfo i_selectionChangedCallback = t_TreeViewController
                .GetProperty(nameof(selectionChangedCallback), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new EntryPointNotFoundException(nameof(selectionChangedCallback));
            Action<int[]> selectionChangedCallback;

            static readonly PropertyInfo i_itemSingleClickedCallback = t_TreeViewController
                .GetProperty(nameof(itemSingleClickedCallback), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new EntryPointNotFoundException(nameof(itemSingleClickedCallback));
            Action<int> itemSingleClickedCallback;
            static readonly MethodInfo i_FindItem = t_TreeViewController
                .GetMethod(nameof(FindItem), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new EntryPointNotFoundException(nameof(itemSingleClickedCallback));

            TreeViewItem FindItem(int id) {
                return i_FindItem.Invoke(m_TestListTree, new object[] { id }) as TreeViewItem;
            }

            static UnityObject FindAsset(TreeViewItem item) {
                var match = Regex.Match(item.displayName, "\\(\"(.+)\"\\)");

                if (match.Success) {
                    string path = match.Groups[1].Value;
                    if (AssetDatabase.LoadMainAssetAtPath(path) is UnityObject obj) {
                        return obj;
                    }

                    if (AssetDatabase.LoadMainAssetAtPath($"Packages/{path}/package.json") is TextAsset manifest) {
                        return manifest;
                    }
                }

                return null;
            }

            void Select(params int[] ids) {
                var assets = ids
                    .Select(FindItem)
                    .Select(FindAsset)
                    .Where(obj => obj)
                    .ToArray();

                if (assets.Length != 0) {
                    Selection.objects = assets;
                    EditorGUIUtility.PingObject(assets[0]);
                }
            }

            void Select(int id) {
                Select(new int[] { id });
            }

            bool isSetUp = false;

            readonly bool useSelection = true;
            readonly bool useClick = false;

            internal AddOn(TestRunnerWindow window) {
                this.window = window;
            }

            internal void Update() {
                if (isSetUp) {
                    return;
                }

                if (window) {
                    m_SelectedTestTypes = i_SelectedTestTypes.GetValue(window);

                    if (m_SelectedTestTypes is not null) {
                        m_TestListTree = i_TestListTree.GetValue(m_SelectedTestTypes);

                        if (m_TestListTree is not null) {
                            if (useSelection) {
                                selectionChangedCallback = i_selectionChangedCallback.GetValue(m_TestListTree) as Action<int[]>;
                                selectionChangedCallback += Select;
                                i_selectionChangedCallback.SetValue(m_TestListTree, selectionChangedCallback);
                            }

                            if (useClick) {
                                itemSingleClickedCallback = i_itemSingleClickedCallback.GetValue(m_TestListTree) as Action<int>;
                                itemSingleClickedCallback += Select;
                                i_itemSingleClickedCallback.SetValue(m_TestListTree, itemSingleClickedCallback);
                            }

                            isSetUp = true;
                        }
                    }
                }
            }
        }

        [InitializeOnLoadMethod]
        static void OnInitialize() {
            EditorApplication.update += Update;
        }

        static readonly Dictionary<TestRunnerWindow, AddOn> windows = new();

        static void Update() {
            if (WindowInstance is TestRunnerWindow window) {
                if (!windows.ContainsKey(window)) {
                    ClearWindows();
                    windows[window] = new AddOn(window);
                }

                windows[window].Update();
            }
        }

        static void ClearWindows() {
            windows.Clear();
            AssetUtils.ClearCache(Enumerable.Empty<string>());
        }

        const string WINDOW_CLASS = nameof(TestRunnerWindow);

        const string WINDOW_FIELD = "s_Instance";

        static readonly FieldInfo WindowInstance_Info = typeof(TestRunnerWindow)
            .GetField(WINDOW_FIELD, BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new EntryPointNotFoundException(WINDOW_FIELD);

        static object WindowInstance => WindowInstance_Info.GetValue(null);

    }
#endif
}