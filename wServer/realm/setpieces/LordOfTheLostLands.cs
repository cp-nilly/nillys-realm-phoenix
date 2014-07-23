namespace wServer.realm.setpieces
{
    internal class LordOfTheLostLands : ISetPiece
    {
        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var loll = Entity.Resolve(0x0d50);
            loll.Move(pos.X + 2.5f, pos.Y + 2.5f);
            world.EnterWorld(loll);
        }
    }
}