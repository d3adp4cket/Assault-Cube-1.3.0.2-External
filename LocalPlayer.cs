using System;

namespace AssaultCubeExternal
{
    public class LocalPlayer
    {
        private readonly Memory _m;
        private readonly IntPtr _base;
        private IntPtr _ptr;

        public LocalPlayer(Memory m, IntPtr moduleBase) { _m = m; _base = moduleBase; Refresh(); }

        public void Refresh()
        {
            // Use IntPtr.Add to avoid any arithmetic issues
            IntPtr ptrAddr = IntPtr.Add(_base, Offsets.LocalPlayer);
            int raw = _m.ReadInt(ptrAddr);
            _ptr = new IntPtr(raw);
        }

        public bool   IsValid => _ptr != IntPtr.Zero;
        public string PtrHex  => _ptr.ToString("X8");

        // Diagnostic: returns a string summarising what we see at the pointer address
        public (int raw, bool ok) DiagRead()
        {
            IntPtr ptrAddr = IntPtr.Add(_base, Offsets.LocalPlayer);
            int raw = _m.ReadInt(ptrAddr);
            return (raw, _m.LastReadSuccess);
        }

        public float  PosX        => _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.PosX));
        public float  PosY        => _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.PosY));
        public float  PosZ        => _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.PosZ));
        public float  CamX        => _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.CamX));
        public float  CamY        => _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.CamY));
        public int    Health      => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.Health));
        public int    Armor       => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.Armor));
        public string Name        => _m.ReadString(IntPtr.Add(_ptr, Offsets.Player.Name));
        public int    AmmoAR      => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.AmmoAR));
        public int    AmmoSMG     => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.AmmoSMG));
        public int    AmmoSniper  => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.AmmoSniper));
        public int    AmmoShotgun => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.AmmoShotgun));
        public int    AmmoPistol  => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.AmmoPistol));
        public int    AmmoGrenade => _m.ReadInt  (IntPtr.Add(_ptr, Offsets.Player.AmmoGrenade));

        // Eye pos = feet pos + head offset
        public Vec3 EyePos => new Vec3
        {
            X = _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.HeadX)),
            Y = _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.HeadY)),
            Z = _m.ReadFloat(IntPtr.Add(_ptr, Offsets.Player.HeadZ)),
        };

        // Write camera angles directly for aimbot
        public void SetCamX(float v) => _m.WriteFloat(IntPtr.Add(_ptr, Offsets.Player.CamX), v);
        public void SetCamY(float v) => _m.WriteFloat(IntPtr.Add(_ptr, Offsets.Player.CamY), v);
        public void SetHealth(int v)          => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.Health),         v);
        public void SetArmor(int v)           => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.Armor),          v);
        public void SetAmmoAR(int v)          => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.AmmoAR),         v);
        public void SetAmmoSMG(int v)         => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.AmmoSMG),        v);
        public void SetAmmoSniper(int v)      => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.AmmoSniper),     v);
        public void SetAmmoShotgun(int v)     => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.AmmoShotgun),    v);
        public void SetAmmoPistol(int v)      => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.AmmoPistol),     v);
        public void SetAmmoGrenade(int v)     => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.AmmoGrenade),    v);
        public void SetFastFireAR(int v)      => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.FastFireAR),     v);
        public void SetFastFireSniper(int v)  => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.FastFireSniper), v);
        public void SetFastFireShotgun(int v) => _m.WriteInt(IntPtr.Add(_ptr, Offsets.Player.FastFireShotgun),v);

        public void GodMode()     { SetHealth(999); SetArmor(999); }
        public void InfiniteAmmo(){ SetAmmoAR(999); SetAmmoSMG(999); SetAmmoSniper(999); SetAmmoShotgun(999); SetAmmoPistol(999); SetAmmoGrenade(999); }
        public void FastFire()    { SetFastFireAR(0); SetFastFireSniper(0); SetFastFireShotgun(0); }
    }
}
