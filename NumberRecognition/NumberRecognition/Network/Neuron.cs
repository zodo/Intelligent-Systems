namespace NumberRecognition
{

    using System;
    using System.Collections.Generic;

    public class Neuron
    {
        public double Output { get; set; }
        public double Delta { get; set; }
        public int Inputs { get; set; }
        public List<double> Weights { get; set; }
        public List<double> Deltas { get; set; }

        public Neuron(int inputs)
        {
            Inputs = inputs;
            Weights = new List<double>(inputs);
            Deltas = new List<double>(inputs);
            var rand = new Random();

            for (int i = 0; i < inputs; i++)
            {
                var d = rand.NextDouble() / 2 - 0.25;
                Weights.Add(d);
                Deltas.Add(0);
            }
        }
    }
}
