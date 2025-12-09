using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.EditorConfiguration
{
    internal class EditorConfig
    {
        public string? LastOpenedProject { get; set; }
        public List<string> RecentProjects { get; set; }

        public EditorConfig()
        {
            RecentProjects = new List<string>();
        }
    }
}
