namespace wServer.realm.setpieces
{
    internal class CubeGod : ISetPiece
    {
        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var cube = Entity.Resolve(0x0d59);
            cube.Move(pos.X + 2.5f, pos.Y + 2.5f);
            world.EnterWorld(cube);
        }
    }
}