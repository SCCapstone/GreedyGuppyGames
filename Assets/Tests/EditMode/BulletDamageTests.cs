using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BulletDamage
    {
        // A Test behaves as an ordinary method
        [Test]
        public void BulletDamageValue()
        {
            // ACT
            // GameObject testBulletGO = (GameObject)Instantiate(this.bulletPrefab, this.firePoint.position, this.firePoint.rotation);
            // Bullet testBullet = testBulletGO.GetComponent<Bullet>();
            Bullet testBullet = new Bullet(); // This is bad and technically shouldn't work, bullet is a monobehavior. Watch more Jason Weimann to fix
            int standardBulletDamage = testBullet.damage;

            // ASSERT
            Assert.AreEqual(expected: 50, actual: standardBulletDamage);
        }
    }
}
