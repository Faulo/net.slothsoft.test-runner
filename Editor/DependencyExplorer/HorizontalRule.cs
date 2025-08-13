using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using UnityEngine.UIElements;

namespace Slothsoft.TestRunner.Editor.DependencyExplorer {
    class HorizontalRule : VisualElement {
        internal HorizontalRule() {
            AddToClassList("hr");
        }
    }
}
