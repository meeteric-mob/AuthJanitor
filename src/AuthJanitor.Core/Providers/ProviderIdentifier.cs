using System;
using System.Collections.Generic;
using System.Text;

namespace AuthJanitor.Providers
{
    public class ProviderIdentifier : IComparable<ProviderIdentifier>, IEquatable<ProviderIdentifier>
    {
        public string Value { get; }

        public ProviderIdentifier(string value)
        {
            Value = value;
        }

        override public string ToString()
        {
            return Value;
        }

        public static ProviderIdentifier FromString(string value)
        {
            return new ProviderIdentifier(value);
        }

        public bool Equals(ProviderIdentifier other) => this.Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
        
        public int CompareTo(ProviderIdentifier other)
        {
            if (other == null) return -1;
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ProviderIdentifier other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();

        #region Operator
        public static bool operator ==(ProviderIdentifier a, ProviderIdentifier b)
        {
            if(ReferenceEquals(null, a) || ReferenceEquals(null, b)) return false;
            return a.CompareTo(b) == 0;
        }
        public static bool operator !=(ProviderIdentifier a, ProviderIdentifier b)
        {
            if (ReferenceEquals(null, a) || ReferenceEquals(null, b)) return false;
            return !(a == b);
        }
        #endregion
    }
}
