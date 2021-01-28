using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class EnemyTests {
        [Test]
        public void enemy_takes_damage() {
            // ARRANGE
            IEnemy enemy = Substitute.For<IEnemy>();
            enemy.SetHealth(1);

            // ACT
            enemy.TakeDamage(1);

            // ASSERT
            Assert.AreEqual(expected: 0, enemy.GetHealth());
        }
    }
}
