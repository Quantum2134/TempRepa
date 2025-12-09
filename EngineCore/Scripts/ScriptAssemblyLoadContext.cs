using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Scripts
{
    public class ScriptAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string _assemblyPath;

        public ScriptAssemblyLoadContext(string assemblyPath)
            : base(isCollectible: true) 
        {
            _assemblyPath = assemblyPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
           
            //if (assemblyName.Name == "EngineCore") // ← замените на реальное имя вашей основной сборки
            //{
            //    // Делегируем загрузку в основной контекст
            //    return Assembly.Load(assemblyName);
            //}

            //// Иначе — попытка загрузить локальную зависимость
            //string? candidate = Path.Combine(Path.GetDirectoryName(_assemblyPath)!, assemblyName.Name + ".dll");
            //if (File.Exists(candidate))
            //    return LoadFromAssemblyPath(candidate);

            //// Иначе — делегировать базовому (default) контексту
            //return null!;


            // Список сборок, которые уже загружены в основном контексте
            // и чьи типы используются в публичном API EngineCore
            var sharedAssemblies = new[]
            {
        "EngineCore",
        "OpenTK.Mathematics",
        "OpenTK",
        "OpenTK.Windowing.GraphicsLibraryFramework"
        // добавьте сюда другие, если используете (например, StbImageWrite, ImGui.NET и т.д.)
    };

            if (sharedAssemblies.Contains(assemblyName.Name))
            {
                // Загружаем из основного контекста — НЕ создаём новую копию!
                return Assembly.Load(assemblyName);
            }

            // Для всего остального — локальные зависимости скрипта (если есть)
            string? candidate = Path.Combine(Path.GetDirectoryName(_assemblyPath)!, assemblyName.Name + ".dll");
            if (File.Exists(candidate))
                return LoadFromAssemblyPath(candidate);

            return null; // пусть CLR сама попробует разрешить (обычно не нужно)
        }
    }
}
