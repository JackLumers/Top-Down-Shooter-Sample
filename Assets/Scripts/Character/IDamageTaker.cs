namespace Character
{
    public interface IDamageTaker
    {
        public void TakeDamage(int damage);
        
        public Faction Faction { get; }
    }
}