using EngineCore.Core;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Input
{
    public static class InputSystem
    {
        public static Application Application;

        public static KeyboardState GetState()
        {
            return Application.KeyboardState;
        }
    }
}
