using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using PrimalEditor.Components;
using PrimalEditor.EngineAPIStructs;

namespace PrimalEditor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
    }
}

namespace PrimalEditor.DllWrapper
{
    static class EngineAPI
    {
        private const string _dllName = "EngineDLL.dll";

        [DllImport(_dllName)]
        private static extern int CreateGameEntity(GameEntityDescriptor desc);

        public static int CreateGameEntity(GameEntity entity)
        {
            GameEntityDescriptor desc = new GameEntityDescriptor();

            //Transform Component
            {
                var c = entity.GetComponent<Transform>();
                desc.Transform.Position = c.Position;
                desc.Transform.Rotation = c.Rotation;
                desc.Transform.Scale = c.Scale;
            }

            return CreateGameEntity(desc);
        }

        [DllImport(_dllName)]
        private static extern void RemoveGameEntity(int id);
        public static void RemoveGameEntity(GameEntity entity)
        {
            RemoveGameEntity(entity.EntityId);
        }
    }
}
