using System;

namespace AIRLab.GeneticAlgorithms
{
    public abstract class Chromosome : IEquatable<Chromosome>, IComparable<Chromosome>, ICloneable
    {
        public bool Evaluated { get; internal set; }
        public bool Inherited { get; internal set; }
        public int Id { get; internal set; }
        public int Generation { get; internal set; }
        public int Age { get; internal set; }
        public int Parent1 { get; internal set; }
        public int Parent2 { get; internal set; }
        public double Value { get; set; }
        public abstract bool Equals(Chromosome other);
        
        public override string ToString()
        {
            return Value.ToString();
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public int CompareTo(Chromosome other)
        {
            return -Value.CompareTo(other.Value);
        }
    }
}
