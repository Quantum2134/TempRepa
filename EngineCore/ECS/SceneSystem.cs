using EngineCore.Assets;
using EngineCore.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.ECS
{
    public class SceneSystem
    {
        private List<Scene> scenes;

        public List<Scene> Scenes => scenes;

        public Scene CurrentScene { get; private set; }

        public Application Application { get; private set; }
        public SceneSerializer SceneSerializer { get; private set; }

        public SceneSystem(Application application)
        {
            Application = application;
            SceneSerializer = new SceneSerializer(application);

            scenes = new List<Scene>();
        }

        public Scene NewScene(string name)
        {
            Scene scene = new Scene();
            scene.Name = name;
            scene.application = Application;
            CurrentScene = scene;

            scenes.Add(scene);

            return scene;
        }

        public void AddScene(Scene scene)
        {
            scenes.Add(scene);
        }

        public bool RemoveScene(Scene scene)
        {
            return scenes.Remove(scene);
        }

        public Scene GetScene(string name)
        {
            return scenes.FirstOrDefault(x => x.Name == name);
        }

        public void SetCurrentScene(Scene scene)
        {
            CurrentScene = scene;
        }
        public void SetCurrentScene(string name)
        {
            CurrentScene = scenes.FirstOrDefault(x => x.Name == name);
        }

        public void SaveScene(Scene scene)
        {
            SceneSerializer.SaveScene(Path.Combine(Application.AssetSystem.ResourcesPath, "Scenes"), scene);
        }

        public void SaveCurrentScene()
        {
            if(CurrentScene != null)
            {
                SceneSerializer.SaveScene(Path.Combine(Application.AssetSystem.ResourcesPath, "Scenes"), CurrentScene);
            }
            
        }

        public void SaveAllScenes()
        {
            foreach (Scene scene in scenes)
            {
                SceneSerializer.SaveScene(Path.Combine(Application.AssetSystem.ResourcesPath, "Scenes"), scene);
            }
        }

        public Scene LoadScene(string scenePath)
        {
            Scene scene = SceneSerializer.LoadScene(scenePath);
            scene.application = Application;
            CurrentScene = scene;

            return scene;

            return null;
        }
    }
}
