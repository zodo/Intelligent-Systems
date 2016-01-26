namespace NumberRecognition
{
	public class Synapse
	{
		public Neuron InputNeuron { get; }
		public Neuron OutputNeuron { get; }
		public double Weight { get; set; }
		public double WeightDelta { get; set; }

		public Synapse(Neuron inputNeuron, Neuron outputNeuron)
		{
			InputNeuron = inputNeuron;
			OutputNeuron = outputNeuron;
			Weight = Network.GetRandom();
		}
	}
}