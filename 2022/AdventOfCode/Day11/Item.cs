using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly LinkedList<Operand> values = new();

        public Item(long val)
        {
            this.Add(val);
        }

        public long Mod(int mod)
        {
            if (values.Count == 0)
                return 0;

            var node = values.First;
            if (node == null)
                return 0;

            var _mod = node.Value.Value % mod;
            while (node.Next != null)
            {
                node = node.Next;
                _mod = node.Value.Operator switch
                {
                    Operator.add => (_mod + node.Value.Value % mod) % mod,
                    Operator.multiply => (_mod * node.Value.Value % mod) % mod,
                    Operator.multiplySelf => (_mod * _mod) % mod,
                    _ => throw new UnreachableException()
                };
            }
            return _mod;
        }

        public Item Add(long value)
        {
            values.AddLast(new Operand(Operator.add, value));
            return this;
        }

        public Item Multiply(long value)
        {
            values.AddLast(new Operand(Operator.multiply, value));
            return this;
        }
        // Pow(2) with the 'MOD' value at that point.
        public Item MultiplySelf()
        {
            values.AddLast(new Operand(Operator.multiplySelf, 0));
            return this;
        }

        public static implicit operator Item(long val) => new(val);
        public static Item operator +(Item left, long right) => left.Add(right);
        public static Item operator *(Item left, long right) => left.Multiply(right);

    }
}
