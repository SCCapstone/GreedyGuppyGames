// <copyright file="MinAttribute.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class MinAttribute : PropertyAttribute
    {
        public readonly float min;

        public MinAttribute(float min)
        {
            this.min = min;
        }
    }
}
