using System;
using System.Collections.Generic;

namespace AssaultCubeExternal
{
    public class GameWorld
    {
        private readonly Memory _m;
        private readonly IntPtr _base;

        public GameWorld(Memory m, IntPtr moduleBase) { _m = m; _base = moduleBase; }

        public int PlayerCount()
            => Math.Clamp(_m.ReadInt(IntPtr.Add(_base, Offsets.PlayerCount)), 0, 64);

        public ViewMatrix GetViewMatrix()
            => _m.ReadMatrix(IntPtr.Add(_base, Offsets.ViewMatrix));

        public List<Entity> GetEnemies()
        {
            var list  = new List<Entity>();
            int count = PlayerCount();
            if (count <= 1) return list;

            // EntityList offset holds a POINTER to the player pointer array
            // Dereference it first to get the actual array base address
            int  arrayPtrRaw = _m.ReadInt(IntPtr.Add(_base, Offsets.EntityList));
            if (arrayPtrRaw == 0) return list;
            IntPtr arrayBase = new IntPtr(arrayPtrRaw);

            for (int i = 1; i < count; i++)  // 0 = local player, skip
            {
                // Each slot is 4 bytes (pointer to player struct)
                IntPtr slotAddr = IntPtr.Add(arrayBase, i * 4);
                int    ptrRaw   = _m.ReadInt(slotAddr);
                if (ptrRaw == 0) continue;
                IntPtr ptr = new IntPtr(ptrRaw);

                int hp = _m.ReadInt(IntPtr.Add(ptr, Offsets.Player.Health));

                list.Add(new Entity
                {
                    Index   = i,
                    Health  = hp,
                    Armor   = _m.ReadInt   (IntPtr.Add(ptr, Offsets.Player.Armor)),
                    Name    = _m.ReadString(IntPtr.Add(ptr, Offsets.Player.Name), 16),
                    IsValid = true,
                    Pos = new Vec3
                    {
                        X = _m.ReadFloat(IntPtr.Add(ptr, Offsets.Player.PosX)),
                        Y = _m.ReadFloat(IntPtr.Add(ptr, Offsets.Player.PosY)),
                        Z = _m.ReadFloat(IntPtr.Add(ptr, Offsets.Player.PosZ)),
                    },
                    HeadPos = new Vec3
                    {
                        X = _m.ReadFloat(IntPtr.Add(ptr, Offsets.Player.HeadX)),
                        Y = _m.ReadFloat(IntPtr.Add(ptr, Offsets.Player.HeadY)),
                        Z = _m.ReadFloat(IntPtr.Add(ptr, Offsets.Player.HeadZ)),
                    },
                });
            }
            return list;
        }
    }
}
