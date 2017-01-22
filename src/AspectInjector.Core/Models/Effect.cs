﻿using Mono.Cecil;
using System;

namespace AspectInjector.Core.Models
{
    public abstract class Effect : IEquatable<Effect>
    {
        public uint Priority { get; protected set; }

        public bool Equals(Effect other)
        {
            if (other == null)
                return false;

            return IsEqualTo(other);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        protected virtual bool IsEqualTo(Effect other)
        {
            return this == other;
        }

        public abstract bool IsApplicableFor(ICustomAttributeProvider target);
    }
}