namespace DecisionTree.Model
{
    using System.Collections.Generic;

    public class Attribute
    {
        public Attribute(string name, string attr)
        {
            Values = new List<string>();
            Name = name;
            IsQuantity = attr == "q";
            IsGoal = attr == "g";
        }

        public bool IsQuantity { get; set; }

        public string Name { get; set; }

        public List<string> Values { get; set; }

        public double InfoGain { get; set; }

        public bool IsGoal { get; set; }    
    }
}
