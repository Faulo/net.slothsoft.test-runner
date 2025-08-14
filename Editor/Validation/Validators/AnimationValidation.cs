using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class AnimationValidation {
        [Validate]
        public static void ValidateFBX(GameObject _, IAssetValidator validator) {
            if (string.IsNullOrEmpty(validator.CurrentAssetPath)) {
                return;
            }

            if (validator.CurrentAssetPath.EndsWith(".fbx", StringComparison.InvariantCultureIgnoreCase)) {
                foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(validator.CurrentAssetPath)) {
                    if (subAsset is AnimationClip or Avatar) {
                        validator.ValidateAsset(subAsset);
                    }
                }
            }
        }

        [Validate]
        public static void ValidateAnimatorController(AnimatorController animator, IAssetValidator validator) {
            foreach (var layer in animator.layers) {
                validator.ValidateAsset(layer.stateMachine);
            }
        }

        [Validate]
        public static void ValidateAnimatorStateMachine(AnimatorStateMachine stateMachine, IAssetValidator validator) {
            foreach (var state in stateMachine.states) {
                validator.ValidateAsset(state.state);
            }

            foreach (AnimatorTransitionBase transition in stateMachine.entryTransitions) {
                validator.ValidateAsset(transition);
            }

            foreach (AnimatorTransitionBase transition in stateMachine.anyStateTransitions) {
                validator.ValidateAsset(transition);
            }
        }

        [Validate]
        public static void ValidateAnimatorState(AnimatorState state, IAssetValidator validator) {
            foreach (AnimatorTransitionBase transition in state.transitions) {
                validator.ValidateAsset(transition);
            }
        }
    }
}