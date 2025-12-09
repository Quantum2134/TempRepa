using EngineCore.Assets.Assets;
using EngineCore.ECS.Components;
using EngineCore.Graphics;
using EngineCore.Logging;
using EngineCore.Platform.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Scripts
{
    public class ScriptSystem
    {
        private ScriptAssemblyLoadContext? _context;
        private Assembly? _assembly;

        private Dictionary<string, Type> scripts;

        public List<ScriptComponent> Components;

        public Dictionary<string, Type> Scripts => scripts;

        public ScriptSystem()
        {
            scripts = new Dictionary<string, Type>();
            Components = new List<ScriptComponent>();
        }

        public void AddScript(ScriptComponent scriptComponent)
        {
            Components.Add(scriptComponent);
        }

        public void LoadScriptAssembly(string dllPath)
        {
            // 1. Выгрузить предыдущую версию (если есть)
            Unload();

            // 2. Создать новый контекст
            _context = new ScriptAssemblyLoadContext(dllPath);

            // 3. Загрузить сборку
            _assembly = _context.LoadFromAssemblyPath(Path.GetFullPath(dllPath));

            // 4. Найти все типы ScriptComponent
            var scriptTypes = new List<Type>();
            foreach (var type in _assembly.GetTypes())
            {
                Logger.Log(type.BaseType.Name);



                if (type.IsAssignableTo(typeof(ScriptComponent)))
                {
                    scriptTypes.Add(type);
                }
            }

            // 5. Зарегистрировать в реестре (например, по имени)
            foreach (var type in scriptTypes)
            {
                //ScriptRegistry.RegisterDynamic(type.Name, () => (ScriptComponent)Activator.CreateInstance(type)!);

                //ScriptComponent script = (ScriptComponent)Activator.CreateInstance(type);
                //script.name = type.Name;
                scripts.Add(type.Name, type);
                //Components.Add((ScriptComponent)Activator.CreateInstance(type));
            }

        }
        public void Unload()
        {
            if (_context != null)
            {
                _context.Unload(); // ← инициирует выгрузку
                _context = null;
                _assembly = null;

                // Запускаем GC, чтобы ускорить выгрузку (не обязательно, но помогает)
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        public Type GetScript(string name)
        {
            return scripts[name];
        }
    }
}
