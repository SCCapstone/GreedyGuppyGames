// <copyright file="DamageCollider.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// Damage collider - a collider based implementation of DamageZone
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DamageCollider : DamageZone
    {
        /// <summary>
        /// On collision enter, see if the colliding object has a Damager and then make the damageableBehaviour take damage
        /// </summary>
        /// <param name="c">The collider</param>
        protected void OnCollisionEnter(Collision c)
        {
            var damager = c.gameObject.GetComponent<Damager>();
            if (damager == null)
            {
                return;
            }

            this.LazyLoad();

            float scaledDamage = this.ScaleDamage(damager.damage);
            Vector3 collisionPosition = this.ConvertContactsToPosition(c.contacts);
            this.damageableBehaviour.TakeDamage(scaledDamage, collisionPosition, damager.alignmentProvider);

            damager.HasDamaged(collisionPosition, this.damageableBehaviour.configuration.alignmentProvider);
        }

        /// <summary>
        /// Averages the contacts to get the position.
        /// </summary>
        /// <returns>The average position.</returns>
        /// <param name="contacts">Contacts.</param>
        protected Vector3 ConvertContactsToPosition(ContactPoint[] contacts)
        {
            Vector3 output = Vector3.zero;
            int length = contacts.Length;

            if (length == 0)
            {
                return output;
            }

            for (int i = 0; i < length; i++)
            {
                output += contacts[i].point;
            }

            output = output / length;
            return output;
        }
    }
}