using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CgminerMonitorClient.Utils
{
    /// <summary>Represents a 2-tuple, or pair. </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <filterpriority>2</filterpriority>
    [Serializable]
    public class Tuple<T1, T2> : IComparable, ITuple
    {
        private readonly T1 _mItem1;
        private readonly T2 _mItem2;

        /// <summary>Initializes a new instance of the <see cref="T:CgminerMonitorClient.Utils.Tuple`2" /> class.</summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        public Tuple(T1 item1, T2 item2)
        {
            _mItem1 = item1;
            _mItem2 = item2;
        }

        /// <summary>Gets the value of the current <see cref="T:CgminerMonitorClient.Utils.Tuple`2" /> object's first component.</summary>
        /// <returns>The value of the current <see cref="T:CgminerMonitorClient.Utils.Tuple`2" /> object's first component.</returns>
        public T1 Item1
        {
            get { return _mItem1; }
        }

        /// <summary>Gets the value of the current <see cref="T:CgminerMonitorClient.Utils.Tuple`2" /> object's second component.</summary>
        /// <returns>The value of the current <see cref="T:CgminerMonitorClient.Utils.Tuple`2" /> object's second component.</returns>
        public T2 Item2
        {
            get { return _mItem2; }
        }

        int ITuple.Size
        {
            get { return 2; }
        }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_mItem1);
            sb.Append(", ");
            sb.Append(_mItem2);
            sb.Append(")");
            return sb.ToString();
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCode();
        }

        /// <summary>
        ///     Returns a string that represents the value of this <see cref="T:CgminerMonitorClient.Utils.Tuple`2" />
        ///     instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="T:CgminerMonitorClient.Utils.Tuple`2" /> object.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            return ((ITuple) this).ToString(stringBuilder);
        }

        protected bool Equals(Tuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(_mItem1, other._mItem1) && EqualityComparer<T2>.Default.Equals(_mItem2, other._mItem2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tuple<T1, T2>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T1>.Default.GetHashCode(_mItem1)*397) ^ EqualityComparer<T2>.Default.GetHashCode(_mItem2);
            }
        }

        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }
            var tuple = other as Tuple<T1, T2>;
            if (tuple == null)
            {
                throw new ArgumentException();
            }

            return Equals(other) ? 0 : 1;
        }

        public static bool operator ==(Tuple<T1, T2> left, Tuple<T1, T2> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tuple<T1, T2> left, Tuple<T1, T2> right)
        {
            return !Equals(left, right);
        }
    }
}