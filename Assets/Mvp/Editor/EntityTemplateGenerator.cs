using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Maze.MVP.Editor
{
    public class EntityTemplateGenerator : EditorWindow
    {
        private string _entityName = "NewEntity";

        [MenuItem("Tools/Entity Template Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<EntityTemplateGenerator>("Entity Generator");
            window.Show();
        }

        private void OnGUI()
        {
            string rootPath = PlayerPrefs.HasKey("EntityRootPath")
                ? PlayerPrefs.GetString("EntityRootPath")
                : "Assets/Scripts/Entities";
        
            GUILayout.Label("Create MVP Entity Templates", EditorStyles.boldLabel);
            _entityName = EditorGUILayout.TextField("Entity Name", _entityName);
        
            // Folder selection UI
            EditorGUILayout.BeginHorizontal();
            rootPath = EditorGUILayout.TextField("Root Path", rootPath);
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Root Folder", rootPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert to relative path if it's under the Assets folder
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        rootPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        rootPath = selectedPath;
                    }
                    PlayerPrefs.SetString("EntityRootPath", rootPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        
            string @namespace = EditorSettings.projectGenerationRootNamespace;
            GUILayout.Space(10);

            if (GUILayout.Button("Generate Entity Files", GUILayout.Height(30)))
            {
                if (string.IsNullOrWhiteSpace(_entityName))
                {
                    EditorUtility.DisplayDialog("Error", "Entity name cannot be empty.", "OK");
                    return;
                }
            
                if (string.IsNullOrWhiteSpace(rootPath))
                {
                    EditorUtility.DisplayDialog("Error", "Path name cannot be empty.", "OK");
                    return;
                }
            
                string folderPath = Path.Combine(rootPath, _entityName);
                GenerateEntityFiles(_entityName, @namespace, folderPath);
            }
        }

        private static void GenerateEntityFiles(string entityName, string namespaceName, string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            string entityVar = char.ToLowerInvariant(entityName[0]) + entityName.Substring(1);

            WriteFile(folderPath, $"{entityName}View.cs", GetViewTemplate(entityName, namespaceName));
            WriteFile(folderPath, $"I{entityName}Model.cs", GetIModelTemplate(entityName, namespaceName));
            WriteFile(folderPath, $"{entityName}Model.cs", GetModelTemplate(entityName, namespaceName));
            WriteFile(folderPath, $"I{entityName}Presenter.cs", GetIPresenterTemplate(entityName, namespaceName));
            WriteFile(folderPath, $"{entityName}Presenter.cs", GetPresenterTemplate(entityName, namespaceName));
            WriteFile(folderPath, $"{entityName}Data.cs", GetDataTemplate(entityName, namespaceName));
            WriteFile(folderPath, $"{entityName}LifetimeScope.cs",
                GetLifetimeScopeTemplate(entityName, namespaceName, entityVar));

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", $"Entity '{entityName}' files generated successfully.", "OK");
        }

        private static void WriteFile(string path, string fileName, string content)
        {
            string fullPath = Path.Combine(path, fileName);
            File.WriteAllText(fullPath, content, Encoding.UTF8);
        }

        #region Templates

        private static string GetViewTemplate(string name, string ns) => $@"
using UnityEngine;

namespace {ns}
{{
    public class {name}View : View<I{name}Model, I{name}Presenter>
    {{
        
    }}
}}";

        private static string GetIModelTemplate(string name, string ns) => $@"
namespace {ns}
{{
    public interface I{name}Model : IModelObserver
    {{
        
    }}
}}";

        private static string GetModelTemplate(string name, string ns) => $@"
namespace {ns}
{{
    public class {name}Model : Model, I{name}Model
    {{
        
    }}
}}";

        private static string GetIPresenterTemplate(string name, string ns) => $@"
namespace {ns}
{{
    public interface I{name}Presenter : IPresenter
    {{
        
    }}
}}";

        private static string GetPresenterTemplate(string name, string ns) => $@"
namespace {ns}
{{
    public class {name}Presenter : Presenter<{name}Model, {name}View>, I{name}Presenter
    {{
        public {name}Presenter({name}Model model, {name}View view) : base(model, view)
        {{
            
        }}
    }}
}}";

        private static string GetDataTemplate(string name, string ns) => $@"
using UnityEngine;

namespace {ns}
{{
    [CreateAssetMenu(fileName = ""{name}Data"", menuName = ""Data/{name}Data"")]
    public class {name}Data : Data
    {{

    }}
}}";

        private static string GetLifetimeScopeTemplate(string name, string ns, string varName) => $@"
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace {ns}
{{
    public class {name}LifetimeScope : LifetimeScope
    {{
        [SerializeField] private {name}View {varName}View;
        
        protected override void Configure(IContainerBuilder builder)
        {{
            builder.RegisterScriptableObject<{name}Data>();
            builder.Register<{name}Model>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent({varName}View).AsSelf().AsImplementedInterfaces();
            builder.Register<{name}Presenter>(Lifetime.Scoped);

            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
            {{
                entryPoints.Add<{name}Presenter>();
            }});
        }}
    }}
}}";

        #endregion
    }
}