// <copyright file="IEnemy.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

// interface used in edit mode test
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