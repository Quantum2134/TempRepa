using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.EditorConfiguration
{
    internal class EditorPaths
    {
        public static string EditorRoot = AppContext.BaseDirectory;

        public static string EditorResourcesPath = Path.Combine(EditorRoot, "EditorResources");

        public static string EditorConfigPath = Path.Combine(EditorRoot, "EditorResources", "Config", "editorconfig.json");

        public static string ProjectTemplatesPath = Path.Combine(EditorRoot, "ProjectTemplates");
    }
}
