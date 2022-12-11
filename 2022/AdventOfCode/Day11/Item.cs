using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Day11
{
    public enum Operator
    {
        add,
        multiply,
        multiplySelf
    }

    public record struct Operand(Operator Operator, long Value);


    public class Item
    {
        private readonly List<Operand> _values = new();
        private readonly DumbItemCache _cache = new();

        public Item(long val)
        {
            this.Add(val);
        }

        public long Mod(int mod)
        {
            if (_values.Count == 0)
                return 0;

            var cache = _cache.Get(new DumbItemCache.Key(mod));
            long partialMod;
            int i;
            if (cache == null)
            {
                partialMod = _values.First().Value % mod;
                i = 1;
            }
            else
            {
                partialMod = cache.Value.ModValue;
                i = cache.Value.Index;
            }

            for(; i < _values.Count; i++)
            {
                partialMod = _values[i].Operator switch
                {
                    Operator.add => (partialMod + _values[i].Value % mod) % mod,
                    Operator.multiply => (partialMod * _values[i].Value % mod) % mod,
                    Operator.multiplySelf => (partialMod * partialMod) % mod,
                    _ => throw new UnreachableException()
                };
            }
            _cache.Cache(new DumbItemCache.Key(mod), new DumbItemCache.Value(i, partialMod));
            return partialMod;
        }

        public Item Add(long value)
        {
            _values.Add(new Operand(Operator.add, value));
            return this;
        }

        public Item Multiply(long value)
        {
            _values.Add(new Operand(Operator.multiply, value));
            return this;
        }
        // Pow(2) with the 'MOD' value at that point.
        public Item MultiplySelf()
        {
            _values.Add(new Operand(Operator.multiplySelf, 0));
            return this;
        }

        public static implicit operator Item(long val) => new(val);
        public static Item operator +(Item left, long right) => left.Add(right);
        public static Item operator *(Item left, long right) => left.Multiply(right);

    }
}
