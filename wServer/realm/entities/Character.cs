namespace wServer.realm.entities
{
    public abstract class Character : Entity
    {
        protected Character(short objType, wRandom rand)
            : base(objType)
        {
            Random = rand;

            if (ObjectDesc == null) return;
            //Name = ObjectDesc.DisplayId ?? "";
            if (ObjectDesc.SizeStep != 0)
            {
                var step = Random.Next(0, (ObjectDesc.MaxSize - ObjectDesc.MinSize)/ObjectDesc.SizeStep + 1)*
                           ObjectDesc.SizeStep;
                Size = ObjectDesc.MinSize + step;
            }
            else
                Size = ObjectDesc.MinSize;

            HP = ObjectDesc.MaxHp;
        }

        public wRandom Random { get; private set; }

        public int HP { get; set; }
    }
}