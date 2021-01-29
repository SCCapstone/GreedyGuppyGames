public interface IEnemy {
    void Die();
    void SetWaypoint(int index);
    void Start();
    void TakeDamage(int amount);
    float GetSpeed();
    int GetHealth();
    int GetValue();
    void SetSpeed(float _speed);
    void SetHealth(int _health);
    void SetValue(int _value);
}