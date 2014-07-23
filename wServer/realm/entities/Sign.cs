namespace wServer.realm.entities
{
    public class Sign : StaticObject
    {
        public Sign(short objType) : base(objType, null, true, false, false)
        {
        }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            return false;
        }
    }
}