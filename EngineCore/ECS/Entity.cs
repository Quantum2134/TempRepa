using EngineCore.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.ECS
{
    public class Entity
    {
        public string Name { get; set; }

        private List<Component> components;

        public Transform transform {  get; set; }

        public List<Component> Components { get { return components; } }

        public Entity(string name)
        {
            Name = name;
            components = new List<Component>();
            transform = new Transform();
        }

        public void AddComponent(Component component)
        {
            component.Entity = this;
            components.Add(component);
        }

        public T GetComponent<T>() where T : Component
        {
            T result = null;

            foreach (var component in components)
            {
                if(component.GetType() == typeof(T) || component.GetType().IsAssignableFrom(typeof(T)))
                {
                    result = (T)component;
                }
            }

            return result;       
        }

        public T[] GetComponents<T>() where T : Component
        {
            //return (T[])(components.FindAll(x => x.GetType().IsAssignableFrom(typeof(T))) as IEnumerable<T>);
            return components.OfType<T>().ToArray();
        }

        public bool RemoveComponent<T>() where T : Component
        {
            return components.Remove(GetComponent<T>());
        }       
    }
}
