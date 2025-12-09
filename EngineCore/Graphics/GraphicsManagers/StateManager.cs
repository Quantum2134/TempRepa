using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using EngineCore.Graphics.GraphicsManagers.States;
using OpenTK.Mathematics;


namespace EngineCore.Graphics.GraphicsManagers
{
    public class StateManager
    {
        private BlendState currentBlend;
        private DepthState currentDepth;
        private ScissorState currentScissor;
        public Color4 ClearColor { get; private set; }

        private static readonly Dictionary<BlendMode, (BlendingFactor Src, BlendingFactor Dst)> BlendModes = new()
        {
            [BlendMode.None] = (BlendingFactor.One, BlendingFactor.Zero),
            [BlendMode.Alpha] = (BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha),
            [BlendMode.Additive] = (BlendingFactor.SrcAlpha, BlendingFactor.One),
            [BlendMode.Multiply] = (BlendingFactor.DstColor, BlendingFactor.Zero),
            [BlendMode.Screen] = (BlendingFactor.One, BlendingFactor.OneMinusSrcColor),
            [BlendMode.Substract] = (BlendingFactor.One, BlendingFactor.One) 
        };

        private static readonly Dictionary<DepthMode, DepthFunction> DepthModes = new()
        {
            [DepthMode.Less] = DepthFunction.Less,
            [DepthMode.LessOrEqual] = DepthFunction.Lequal,
            [DepthMode.Equal] = DepthFunction.Equal,
            [DepthMode.Greater] = DepthFunction.Greater,
            [DepthMode.GreaterOrEqual] = DepthFunction.Gequal,
            [DepthMode.Always] = DepthFunction.Always,
            [DepthMode.Never] = DepthFunction.Never
        };

        public StateManager()
        {
            currentBlend = new BlendState()
            {
                Enabled = true,
                SrcFactor = BlendingFactor.SrcAlpha,
                DstFactor = BlendingFactor.OneMinusSrcAlpha
            };
            currentDepth = new DepthState()
            {
                Enabled = true,
                Func = DepthFunction.Lequal
            };
            currentScissor = new ScissorState()
            {
                Enabled = false,
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0
            };

            ApplyBlendState(currentBlend);
            ApplyDepthState(currentDepth);
            ApplyScissorState(currentScissor);

            ClearColor = Color4.Gray;
            GL.ClearColor(ClearColor);
        }

        public void SetClearColor(Color4 color)
        {
            GL.ClearColor(color);
            ClearColor = color;
        }
        public void SetBlendMode(BlendMode mode)
        {
            (BlendingFactor src, BlendingFactor dst) = BlendModes[mode];
            var newState = new BlendState
            {
                Enabled = mode != BlendMode.None,
                SrcFactor = src,
                DstFactor = dst
            };

            if (!currentBlend.Equals(newState))
            {
                currentBlend = newState;
                ApplyBlendState(newState);
            }
        }

        public void SetDepthMode(DepthMode mode, bool enabled = true)
        {
            DepthFunction func = DepthModes[mode];
            var newState = new DepthState
            {
                Enabled = enabled,
                Func = func
            };

            if (!currentDepth.Equals(newState))
            {
                currentDepth = newState;
                ApplyDepthState(newState);
            }
        }

        public void SetScissorState(ScissorState state)
        {
            if (!currentScissor.Equals(state))
            {
                currentScissor = state;
                ApplyScissorState(state);
            }
        }



        private void ApplyBlendState(BlendState state)
        {
            if (state.Enabled)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(state.SrcFactor, state.DstFactor);
            }
            else
            {
                GL.Disable(EnableCap.Blend);
            }
        }

        private void ApplyDepthState(DepthState state)
        {
            if (state.Enabled)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(state.Func);
            }
            else
            {
                GL.Disable(EnableCap.DepthTest);
            }
        }

        private void ApplyScissorState(ScissorState state)
        {
            if (state.Enabled)
            {
                GL.Enable(EnableCap.ScissorTest);
                GL.Scissor(state.X, state.Y, state.Width, state.Height);
            }
            else
            {
                GL.Disable(EnableCap.ScissorTest);
            }
        }
    }
}
