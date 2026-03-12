namespace AssaultCubeExternal
{
    public class Entity
    {
        public bool   IsValid    { get; set; }
        public int    Index      { get; set; }
        public int    Health     { get; set; }
        public int    Armor      { get; set; }
        public string Name       { get; set; } = "";
        public Vec3   Pos        { get; set; }   // feet
        public Vec3   HeadPos    { get; set; }   // head

        // Screen-space (filled by EspOverlay each frame)
        public float  ScrFeetX, ScrFeetY;
        public float  ScrHeadX, ScrHeadY;
        public bool   OnScreen;
        public float  Distance;

        // Derived box (filled by EspOverlay)
        public float BoxX, BoxY, BoxW, BoxH;
    }
}
