namespace NumberRecognition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Network
	{
        private double LearnRate { get; }

        private double Momentum { get; }

        private List<Neuron> InputLayer { get; }

        private List<Neuron> HiddenLayer { get; }

        private List<Neuron> OutputLayer { get; }

		private static readonly Random Random = new Random();


		public Network(int inputSize, int hiddenSize, int outputSize, double? learnRate = null, double? momentum = null)
		{
			LearnRate = learnRate ?? .4;
			Momentum = momentum ?? .9;
			InputLayer = new List<Neuron>();
			HiddenLayer = new List<Neuron>();
			OutputLayer = new List<Neuron>();

			for (var i = 0; i < inputSize; i++)
				InputLayer.Add(new Neuron());

			for (var i = 0; i < hiddenSize; i++)
				HiddenLayer.Add(new Neuron(InputLayer));

			for (var i = 0; i < outputSize; i++)
				OutputLayer.Add(new Neuron(HiddenLayer));
		}

		
		public int Train(List<DataSet> dataSets, double minimumError, IProgress<int> progress = null, CancellationToken ctk = default(CancellationToken))
		{
			var error = 1.0;
			var numEpochs = 0;
		    double? startError = null;

			while (error > minimumError && numEpochs < int.MaxValue)
			{
				var errors = new List<double>();
				foreach (var dataSet in dataSets)
				{
					ForwardPropagate(dataSet.Values);
					BackPropagate(dataSet.Targets);
					errors.Add(CalculateError(dataSet.Targets));
				}
				error = errors.Average();
                numEpochs++;

                if (!startError.HasValue)
                {
                    startError = error;
                }
                var res = GetProgressPercent(minimumError, startError.Value, error);
			    progress?.Report((int)res);
                ctk.ThrowIfCancellationRequested();

			}
		    return numEpochs;
		}

        private static double GetProgressPercent(double minimumError, double startError, double error)
        {
            var prog = Math.Abs((startError - error) * 100 / (startError - minimumError));
            var t = prog / 100;
            var c = t;
            var res = c * t * t * t * t * t * t * 100;
            return res;
        }

        public double[] Compute(double[] inputs)
        {
            ForwardPropagate(inputs);
            return OutputLayer.Select(a => a.Value).ToArray();
        }


        private void ForwardPropagate(double[] inputs)
		{
			var i = 0;
            InputLayer.ForEach(a => a.Value = inputs[i++]);

            Parallel.ForEach(HiddenLayer, n => n.CalculateValue());
            Parallel.ForEach(OutputLayer, n => n.CalculateValue());
		}

        private void BackPropagate(double[] targets)
		{
			var i = 0;

			OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));

            Parallel.ForEach(HiddenLayer, n => n.CalculateGradient());
            Parallel.ForEach(HiddenLayer, n => n.UpdateWeights(LearnRate, Momentum));
            Parallel.ForEach(OutputLayer, n => n.UpdateWeights(LearnRate, Momentum));
		}
        
        private double CalculateError(double[] targets)
		{
			var i = 0;
			return OutputLayer.Sum(a => Math.Abs(a.CalculateError(targets[i++])));
		}

        public static Func<int> GetRandomFunc { get; set; } 

        public static double GetRandom()
		{
		    if (GetRandomFunc != null)
		    {
		        return GetRandomFunc();
		    }
			return 2 * Random.NextDouble() - 1;
		}
	}
}