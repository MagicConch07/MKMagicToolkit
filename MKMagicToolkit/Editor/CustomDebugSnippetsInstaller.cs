using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MKMagicToolkit.Editors
{
    public enum EIdeType
    {
        VisualStudio,
        VSCode,
        Rider
    }

    public enum EVisualStudioVersion
    {
        VS2019,
        VS2022,
        VS2026
    }

    public class MKMagicToolkitVscUserSnippetInstaller : EditorWindow
    {
        private const string SnippetsRelDir = "Editor/Snippets";
        private const string VsSourceRelPath = SnippetsRelDir + "/CustomDebugSnippet_VS.snippet";
        private const string VscSourceRelPath = SnippetsRelDir + "/CustomDebugSnippets_VSC.code-snippets";

        private string _userSnippetsDir;
        private EIdeType _selectedIde = EIdeType.VisualStudio;
        private EVisualStudioVersion _selectedVsVersion = EVisualStudioVersion.VS2026;

        private Vector2 _scroll;

        [MenuItem("MKMagicToolkit/Snippet Installer")]
        public static void Open()
        {
            GetWindow<MKMagicToolkitVscUserSnippetInstaller>("Custom Debug Snippets");
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_userSnippetsDir))
            {
                _userSnippetsDir = (_selectedIde == EIdeType.VisualStudio)
                    ? GetVisualStudioUserSnippetDir(_selectedVsVersion)
                    : GetVscodeUserSnippetsDir();
            }
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.LabelField("Snippets Installer", EditorStyles.boldLabel);

            if (_selectedIde == EIdeType.VisualStudio)
            {
                EditorGUILayout.HelpBox(
                    "Visual Studio 사용자(My Code Snippets) 폴더에 설치합니다.\n" +
                    "예: %USERPROFILE%\\Documents\\Visual Studio (your version)\\Code Snippets\\Visual C#\\My Code Snippets",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "VS Code 전역(User) snippets 폴더에 설치합니다.\n" +
                    "예: %APPDATA%\\Code\\User\\snippets",
                    MessageType.Info);
            }

            EditorGUI.BeginChangeCheck();
            _selectedIde = (EIdeType)EditorGUILayout.EnumPopup("IDE", _selectedIde);
            if (EditorGUI.EndChangeCheck())
            {
                _userSnippetsDir = (_selectedIde == EIdeType.VisualStudio)
                    ? GetVisualStudioUserSnippetDir(_selectedVsVersion)
                    : GetVscodeUserSnippetsDir();
            }

            EditorGUILayout.Space(8);

            if (_selectedIde == EIdeType.VisualStudio)
            {
                EditorGUI.BeginChangeCheck();
                _selectedVsVersion = (EVisualStudioVersion)EditorGUILayout.EnumPopup("VS Version", _selectedVsVersion);
                if (EditorGUI.EndChangeCheck())
                {
                    _userSnippetsDir = GetVisualStudioUserSnippetDir(_selectedVsVersion);
                }

                EditorGUILayout.Space(8);
            }

            if (_selectedIde == EIdeType.Rider)
            {
                DrawRiderOnly();
                EditorGUILayout.EndScrollView();
                return;
            }

            // Source
            var sourcePath = ResolveSourcePath(_selectedIde);
            EditorGUILayout.LabelField("Source (in package):", EditorStyles.miniBoldLabel);
            EditorGUILayout.SelectableLabel(sourcePath ?? "(not resolved)", GUILayout.Height(18));

            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                EditorGUILayout.HelpBox("Source snippet file not found. SourceRelPath를 확인하세요.", MessageType.Error);
                EditorGUILayout.EndScrollView();
                return;
            }

            EditorGUILayout.Space(8);

            // Destination folder
            EditorGUILayout.LabelField("Destination (User snippets folder):", EditorStyles.miniBoldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                _userSnippetsDir = EditorGUILayout.TextField(_userSnippetsDir);

                if (GUILayout.Button("Browse", GUILayout.Width(80)))
                {
                    var picked = EditorUtility.OpenFolderPanel("Select User snippets folder", _userSnippetsDir, "");
                    if (!string.IsNullOrEmpty(picked))
                        _userSnippetsDir = picked;
                }
            }

            EditorGUILayout.Space(8);

            string installFileName = (_selectedIde == EIdeType.VisualStudio)
                ? Path.GetFileName(VsSourceRelPath)
                : Path.GetFileName(VscSourceRelPath);

            var destFile = string.IsNullOrEmpty(_userSnippetsDir) ? null : Path.Combine(_userSnippetsDir, installFileName);

            EditorGUILayout.LabelField("Install target file:", EditorStyles.miniBoldLabel);
            EditorGUILayout.SelectableLabel(destFile ?? "(no folder)", GUILayout.Height(18));

            EditorGUILayout.Space(8);

            // Actions
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Install", GUILayout.Height(28)))
                {
                    Install();
                }

                if (GUILayout.Button("Remove", GUILayout.Height(28)))
                {
                    Remove();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void Install()
        {
            if (string.IsNullOrEmpty(_userSnippetsDir))
            {
                EditorUtility.DisplayDialog("MKMagicToolkit", "Destination folder is empty.\n경로를 지정하세요.", "OK");
                return;
            }

            if (!Directory.Exists(_userSnippetsDir))
            {
                EditorUtility.DisplayDialog("MKMagicToolkit", "Destination folder does not exist.\n폴더를 먼저 생성하거나 올바른 경로를 지정하세요.\n\n" + _userSnippetsDir, "OK");
                return;
            }

            var sourcePath = ResolveSourcePath(_selectedIde);
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                EditorUtility.DisplayDialog("MKMagicToolkit", "Source snippet file not found:\n" + (sourcePath ?? "(null)"), "OK");
                return;
            }

            string installFileName = (_selectedIde == EIdeType.VisualStudio)
                ? Path.GetFileName(VsSourceRelPath)
                : Path.GetFileName(VscSourceRelPath);

            var destFile = Path.Combine(_userSnippetsDir, installFileName);

            File.Copy(sourcePath, destFile, true);

            EditorUtility.DisplayDialog("MKMagicToolkit", "Installed:\n" + destFile, "OK");
        }

        private void Remove()
        {
            if (string.IsNullOrEmpty(_userSnippetsDir))
            {
                EditorUtility.DisplayDialog("MKMagicToolkit", "Destination folder is empty.\n경로를 지정하세요.", "OK");
                return;
            }

            string installFileName = (_selectedIde == EIdeType.VisualStudio)
                ? Path.GetFileName(VsSourceRelPath)
                : Path.GetFileName(VscSourceRelPath);

            var destFile = Path.Combine(_userSnippetsDir, installFileName);

            if (!File.Exists(destFile))
            {
                EditorUtility.DisplayDialog("MKMagicToolkit", "No file to remove:\n" + destFile, "OK");
                return;
            }

            File.Delete(destFile);
            EditorUtility.DisplayDialog("MKMagicToolkit", "Removed:\n" + destFile, "OK");
        }


        private string ResolveSourcePath(EIdeType ide)
        {
            string rel = ide switch
            {
                EIdeType.VisualStudio => VsSourceRelPath,
                EIdeType.VSCode => VscSourceRelPath,
                _ => null
            };

            var pkg = ResolveSourcePathInPackage(rel);
            if (!string.IsNullOrEmpty(pkg) && File.Exists(pkg))
            {
                return pkg;
            }

            return null;
        }

        private string ResolveSourcePathInPackage(string rel)
        {
            if (string.IsNullOrEmpty(rel))
            {
                Debug.LogError("ResolveSourcePathInPackage: rel is null or empty");
                return null;
            }

            var script = MonoScript.FromScriptableObject(this);
            var scriptPath = AssetDatabase.GetAssetPath(script);

            var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scriptPath);
            if (info == null || string.IsNullOrEmpty(info.resolvedPath))
            {
                Debug.LogError("ResolveSourcePathInPackage: PackageInfo not found for scriptPath = " + scriptPath);
                return null;
            }

            return Path.Combine(info.resolvedPath, rel);
        }

        private string GetVscodeUserSnippetsDir()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(appData) || !Directory.Exists(appData))
            {
                return string.Empty;
            }

            return Path.Combine(appData, "Code", "User", "snippets");
        }

        private string GetVisualStudioUserSnippetDir(EVisualStudioVersion versions)
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrEmpty(docs) || !Directory.Exists(docs))
            {
                Debug.LogError("GetVisualStudioUserSnippetDir: MyDocuments folder not found.");
                return string.Empty;
            }

            string vsVersionName = versions switch
            {
                EVisualStudioVersion.VS2019 => "Visual Studio 2019",
                EVisualStudioVersion.VS2022 => "Visual Studio 2022",
                EVisualStudioVersion.VS2026 => "Visual Studio 18",
                _ => "None"
            };

            return Path.Combine(docs, vsVersionName, "Code Snippets", "Visual C#", "My Code Snippets");
        }

        private void DrawRiderOnly()
        {
            EditorGUILayout.HelpBox(
                "Rider는 자동 설치하지 않습니다.\n" +
                "아래 버튼을 눌러 패키지 내 Rider 스니펫 폴더로 이동한 뒤, 사용자가 Rider에서 수동 설치하세요.",
                MessageType.Info);

            var snippetsAbsDir = ResolveSourcePathInPackage(SnippetsRelDir);

            EditorGUILayout.LabelField("Package Rider Snippets Folder", EditorStyles.miniBoldLabel);
            EditorGUILayout.SelectableLabel(snippetsAbsDir ?? "(not resolved)", GUILayout.Height(18));

            if (GUILayout.Button("Open package Rider snippets folder", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(snippetsAbsDir) || !Directory.Exists(snippetsAbsDir))
                {
                    EditorUtility.DisplayDialog("MKMagicToolkit", "폴더를 찾지 못했습니다:\n\n" + snippetsAbsDir, "OK");
                    return;
                }

                EditorUtility.OpenWithDefaultApp(snippetsAbsDir);
            }
        }
    }
}