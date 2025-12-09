using EngineCore.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EngineEditor.GameBuild
{
    public class GameBuilder
    {
        private readonly string _runtimeProjectPath; // путь до RuntimeGame.csproj
        private readonly string _resourcesSourcePath; // путь до папки resources в редакторе

        public GameBuilder(string runtimeProjectPath, string resourcesSourcePath)
        {
            _runtimeProjectPath = Path.GetFullPath(runtimeProjectPath);
            _resourcesSourcePath = Path.GetFullPath(resourcesSourcePath);
        }

        public bool Build(string finalPath, Type[] types)
        {
            try
            {
                //finalPath = Path.GetFullPath(finalPath);
                //Directory.CreateDirectory(finalPath); // создаст, если не существует

                //// 1. Копируем ресурсы
                //Console.WriteLine($"Copying resources from {_resourcesSourcePath} to {Path.Combine(finalPath, "resources")}");
                //CopyDirectory(_resourcesSourcePath, Path.Combine(finalPath, "resources"));



                ////2 scripts





                //// 3. Собираем и публикуем runtime в finalPath
                //Console.WriteLine($"Building runtime to {finalPath}");
                //var psi = new ProcessStartInfo
                //{
                //    FileName = "dotnet",
                //    Arguments = $"publish \"{_runtimeProjectPath}\" -c Release -r win-x64 --self-contained false -o \"{finalPath}\"",
                //    UseShellExecute = false,
                //    RedirectStandardOutput = true,
                //    RedirectStandardError = true,
                //    CreateNoWindow = true
                //};

                //using var process = Process.Start(psi);
                //process.WaitForExit();

                //if (process.ExitCode != 0)
                //{
                //    var error = process.StandardError.ReadToEnd();
                //    Console.WriteLine($"Build failed:\n{error}");
                //    return false;
                //}

                //Console.WriteLine("Build succeeded!");
                //return true;

                string tempDir = Path.Combine(Path.GetTempPath(), "QE_Build_" + Guid.NewGuid());
                Directory.CreateDirectory(tempDir);
                Logger.Log(tempDir);
                // Копируем шаблон
                CopyDirectory(Path.GetDirectoryName(@"D:\Projects\GameEngine\EngineRuntime\"), tempDir);

                // 1. Генерируем и вставляем скрипты в Program.cs
                string scriptCode = GenerateScriptRegistrationCode(types);
                string programPath = Path.Combine(tempDir, "RuntimeGame.cs");
                string code = File.ReadAllText(programPath).Replace("// [AUTOGEN_SCRIPTS]", scriptCode);
                File.WriteAllText(programPath, code);

                //// 2. Добавляем ссылку на scripts.dll в .csproj
                string csprojPath = Path.Combine(tempDir, "EngineRuntime.csproj");
                //string relativeDllPath = MakeRelativePath(csprojPath, scriptsDllPath);
                //AddReferenceToCsproj(csprojPath, relativeDllPath);

                // 3. Копируем саму DLL в выходную папку (иначе runtime не найдёт)
                //string finalOutput = Path.GetFullPath(settings.OutputDirectory);
                //Directory.CreateDirectory(finalOutput);
                //File.Copy(scriptsDllPath, Path.Combine(finalOutput, "scripts.dll"), overwrite: true);

                // 4. Собираем из tempDir
                //string args = $"publish \"{csprojPath}\" -c Release -r {settings.TargetRuntime} " +
                //              (settings.SelfContained ? "--self-contained true " : "--self-contained false ") +
                //              $"-o \"{finalOutput}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish \"{_runtimeProjectPath}\" -c Release -r win-x64 --self-contained false -o \"{finalPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    Console.WriteLine($"Build failed:\n{error}");
                    return false;
                }

                Console.WriteLine("Build succeeded!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Build error: {ex}");
                return false;
            }
        }

        public static string GenerateScriptRegistrationCode(Type[] scriptTypes)
        {
            var code = new StringBuilder();
            for (int i = 0; i < scriptTypes.Length; i++)
            {
                var type = scriptTypes[i];
                string typeName = type.FullName; // важно: полное имя с неймспейсом!
                string varName = $"script{i}";

                code.AppendLine($"var {varName} = new {typeName}();");
                code.AppendLine($"ScriptSystem.AddScript({varName});");
            }
            return code.ToString();
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                var targetFile = Path.Combine(targetDir, file.Substring(sourceDir.Length + 1));
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
                File.Copy(file, targetFile, overwrite: true);
            }
        }
    }
}