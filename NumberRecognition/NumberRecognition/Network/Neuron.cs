namespace NumberRecognition
{
    using System.Collections.Generic;
    using System.Linq;

    public class Neuron
	{
	    private List<Synapse> InputSynapses { get; set; }

	    private List<Synapse> OutputSynapses { get; set; }

	    private double Bias { get; set; }

	    private double BiasDelta { get; set; }

	    private double Gradient { get; set; }
		public double Value { get; set; }

		public Neuron()
		{
			InputSynapses = new List<Synapse>();
			OutputSynapses = new List<Synapse>();
			Bias = Network.GetRandom();
		}

		public Neuron(IEnumerable<Neuron> inputNeurons) : this()
		{
			foreach (var inputNeuron in inputNeurons)
			{
				var synapse = new Synapse(inputNeuron, this);
				inputNeuron.OutputSynapses.Add(synapse);
				InputSynapses.Add(synapse);
			}
		}

		public void CalculateValue()
		{
			Value = Sigmoid.Output(InputSynapses.Sum(a => a.Weight * a.InputNeuron.Value) + Bias);
		}

		public double CalculateError(double target)
		{
			return target - Value;
		}

		public void CalculateGradient(double? target = null)
		{
		    if (target == null)
		    {
		        Gradient = OutputSynapses.Sum(a => a.OutputNeuron.Gradient * a.Weight) * Sigmoid.Derivative(Value);
		    }
		    else
		    {
		        Gradient = CalculateError(target.Value) * Sigmoid.Derivative(Value);
		    }
		}

		public void UpdateWeights(double learnRate, double momentum)
		{
			var prevDelta = BiasDelta;
			BiasDelta = learnRate * Gradient;
			Bias += BiasDelta + momentum * prevDelta;

			foreach (var synapse in InputSynapses)
			{
				prevDelta = synapse.WeightDelta;
				synapse.WeightDelta = learnRate * Gradient * synapse.InputNeuron.Value;
				synapse.Weight += synapse.WeightDelta + momentum * prevDelta;
			}
		}
	}
}