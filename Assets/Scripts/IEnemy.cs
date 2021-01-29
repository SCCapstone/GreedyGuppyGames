public interface IEnemy
{
    void Die();
    void SetWaypoint(int index);
    void Start();
    void TakeDamage(int amount);
}