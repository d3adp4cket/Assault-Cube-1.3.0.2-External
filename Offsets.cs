namespace AssaultCubeExternal
{
    public static class Offsets
    {
        public static readonly int LocalPlayer = 0x0017E0A8;
        public static readonly int EntityList  = 0x18AC04;
        public static readonly int FOV         = 0x18A7CC;
        public static readonly int PlayerCount = 0x18AC0C;
        public static readonly int ViewMatrix  = 0x501D78; // 4x4 float matrix

        public static class Player
        {
            public const int PosX = 0x2C, PosY = 0x30, PosZ = 0x28;
            public const int HeadX = 0x04, HeadY = 0x0C, HeadZ = 0x08;
            public const int CamX = 0x34, CamY = 0x38;
            public const int AmmoPistol = 0x12C, AmmoShotgun = 0x134, AmmoSMG = 0x138;
            public const int AmmoSniper = 0x13C, AmmoAR = 0x140, AmmoGrenade = 0x144;
            public const int FastFireShotgun = 0x158, FastFireSniper = 0x160, FastFireAR = 0x164;
            public const int AutoShoot = 0x204;
            public const int Health = 0xEC, Armor = 0xF0;
            public const int Name = 0x205;
        }
    }
}
