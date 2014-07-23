namespace wServer.realm.entities
{
    internal class Placeholder : StaticObject
    {
        public Placeholder(int life)
            : base(0x070f, life, true, true, false)
        {
            Size = 0;
        }
    }
}