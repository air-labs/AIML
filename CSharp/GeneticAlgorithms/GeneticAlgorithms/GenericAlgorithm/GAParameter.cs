using System;

namespace AIRLab.GeneticAlgorithms
{
    public class GaParameter
    {
        public GaParameter(string name, string group, double min, double max, double initialValue)
            :this(name,group,min,max)
        {
            Value = initialValue;
        }

        public GaParameter(string name, string group, double min, double max)
        {
            Name = name;
            Group = group;
            Min = min;
            Max = max;
            Value = (min + max)/2;
        }

        public string Name { get; private set; }
        public string Group { get; private set; }

        private double _value;
        public double Value
        {
            get { return _value; }
            set 
            { 
                _value = value;
                if (ValueChanged != null) ValueChanged(_value);
            }
        }

        public double Min { get; private set; }
        public double Max { get; private set; }

        public event Action<double> ValueChanged;
    }
}