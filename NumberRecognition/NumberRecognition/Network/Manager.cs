namespace NumberRecognition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Manager
    {
        private readonly ImageOperations _imgOperations = new ImageOperations();

        private readonly Network _network = new Network(520, 30, 10);

        public async Task<int> Learn(IProgress<int> progressReporter, CancellationToken ctk)
        {
            var task = Task.Run(
                 () =>
                 {
                     var dataSets = GetDataSets();
                     return _network.Train(dataSets, 0.05, progressReporter, ctk);
                 }, ctk);
            return await task;
        }

        public async Task<double[]> Recognize(double[] img)
        {
            return await Task.Run(() => _network.Compute(img));
        }

        private double[] GetInputVector(int digit)
        {
            var vector = new double[10];
            for (int i = 0; i < 10; i++)
            {
                if(i == digit)
                {
                    vector[i] = 1;
                }
                else
                {
                    vector[i] = 0;
                }
            }
            return vector;
        }

        private List<DataSet> GetDataSets()
        {
            var datasets = new List<DataSet>();
            for (int digit = 0; digit < 10; digit++)
            {
                var outputs = _imgOperations.GetImagesBytesForDigit(digit).ToArray();
                var input = GetInputVector(digit);
                foreach (var output in outputs)
                {
                    datasets.Add(new DataSet(output, input));
                }
            }
            return datasets.OrderBy(x => Network.GetRandom()).ToList();
        }            
    }
}
