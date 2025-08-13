using System;
using System.Linq;
using System.Reflection;
using Slothsoft.TestRunner.Editor;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ulisses.HeXXen1733.VFX.Editor {
    static class ShaderValidation {
        static readonly MethodInfo OpenCompiledShaderMethod = typeof(ShaderUtil).GetMethod("OpenCompiledShader", BindingFlags.NonPublic | BindingFlags.Static);

        static void OpenCompiledShader(Shader shader, int mode, int externPlatformsMask, bool includeAllVariants, bool preprocessOnly, bool stripLineDirectives) {
            OpenCompiledShaderMethod.Invoke(null, new object[] { shader, mode, externPlatformsMask, includeAllVariants, preprocessOnly, stripLineDirectives });
        }

        static readonly MethodInfo CompileShaderVariantMethod = typeof(ShaderUtil).GetMethod("CompileShaderVariant", BindingFlags.NonPublic | BindingFlags.Static);

        static ShaderData.VariantCompileInfo CompileShaderVariant(Shader shader, int subShaderIndex, int passId, ShaderType shaderType, BuiltinShaderDefine[] platformKeywords, string[] keywords, ShaderCompilerPlatform shaderCompilerPlatform, BuildTarget buildTarget, GraphicsTier tier, bool outputForExternalTool) {
            return (ShaderData.VariantCompileInfo)CompileShaderVariantMethod.Invoke(null, new object[] { shader, subShaderIndex, passId, shaderType, platformKeywords, keywords, shaderCompilerPlatform, buildTarget, tier, outputForExternalTool });
        }

        static readonly BuildTarget ActiveCompileTarget = EditorUserBuildSettings.activeBuildTarget;

        static readonly ShaderCompilerPlatform[] ActiveCompilePlatforms = PlayerSettings
            .GetGraphicsAPIs(ActiveCompileTarget)
            .Select(graphicsAPI => graphicsAPI switch {
                GraphicsDeviceType.Direct3D11 => ShaderCompilerPlatform.D3D,
                GraphicsDeviceType.Direct3D12 => ShaderCompilerPlatform.D3D,
                GraphicsDeviceType.Vulkan => ShaderCompilerPlatform.Vulkan,
                GraphicsDeviceType.Metal => ShaderCompilerPlatform.Metal,
                _ => ShaderCompilerPlatform.None,
            })
            .Where(platform => platform is not ShaderCompilerPlatform.None)
            .Distinct()
            .ToArray();

        static readonly int ActiveCompilePlatformsMask = ActiveCompilePlatforms
            .Aggregate(0, (mask, platform) => mask | (1 << (int)platform));

        const bool INCLUDE_ALL_VARIANTS = false;
        const bool PREPROCESS_ONLY = false;
        const bool STRIP_LINE_DIRECTIVES = false;

        const bool RECOMPILE_SHADERS = false;

        [Validate]
        public static void CompileShader(Shader shader, IAssetValidator validator) {
            if (RECOMPILE_SHADERS || Application.isBatchMode) {
                OpenCompiledShader(shader, 1, ActiveCompilePlatformsMask, INCLUDE_ALL_VARIANTS, PREPROCESS_ONLY, STRIP_LINE_DIRECTIVES);
            } else {
                ReportToValidator(ShaderUtil.GetShaderMessages(shader), validator);
            }
        }

        static void ReportToValidator(ShaderMessage[] messages, IAssetValidator validator) {
            foreach (var message in messages) {
                switch (message.severity) {
                    case ShaderCompilerMessageSeverity.Error:
                        validator.AssertFail($"{message.file}:{message.line}{Environment.NewLine}[{message.severity}] {message.message}{Environment.NewLine}{message.messageDetails}");
                        break;
                }
            }
        }

        // [Validate]
        // This looks like it should be a more precise version of CompileShader, but the keywords used don't line up with the ones that OpenCompiledShader uses.
        // Also, sometimes ShaderUtil.GetPassKeywords reports a PassIdentifier as invalid for no apparent reason.
        // Code is kept here for future research purposes.
        public static void CompileShaderVariants(Shader shader, IAssetValidator validator) {
            var data = ShaderUtil.GetShaderData(shader);

            foreach (var platform in ActiveCompilePlatforms) {
                var platformKeywords = ShaderUtil.GetShaderPlatformKeywordsForBuildTarget(platform, ActiveCompileTarget);

                for (int subShaderId = 0; subShaderId < data.SubshaderCount; subShaderId++) {
                    var subShader = data.GetSubshader(subShaderId);
                    for (int passId = 0; passId < subShader.PassCount; passId++) {
                        var pass = subShader.GetPass(passId);
                        PassIdentifier id = new((uint)subShaderId, (uint)passId);

                        for (int typeId = (int)ShaderType.Vertex; typeId < (int)ShaderType.Count; typeId++) {
                            var type = (ShaderType)typeId;
                            if (!pass.HasShaderStage(type)) {
                                continue;
                            }

                            string[] shaderKeywords = ShaderUtil.GetPassKeywords(shader, id, type, platform)
                                .Where(k => k is { type: ShaderKeywordType.UserDefined, isOverridable: true })
                                .Select(k => k.name)
                                .ToArray();

                            var info = pass.CompileVariant(type, shaderKeywords, platform, ActiveCompileTarget, platformKeywords, (GraphicsTier)(-1), false);

                            validator.AssertTrue(info.Success, $"Failed to compile {validator.GetName(shader)} variant: Subshader {subShaderId}, Pass {passId}, Type {type}, Platform {platform}, Target {ActiveCompileTarget}");

                            ReportToValidator(info.Messages, validator);
                        }
                    }
                }
            }
        }
    }
}