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

        private List<DataSet> GetDataSets()
        {
            var tuples =
                Enumerable.Range(0, 10)
                    .AsParallel()
                    .Select(x => new { x, bytes = _imgOperations.GetImagesBytesForDigit(x).ToArray() })
                    .SelectMany(x => x.bytes, (x, b) => new {x.x, b})
                    .Select(x => new DataSet(x.b, Enumerable.Range(0, 10).Select(ee => (double)(ee == x.x ? 1 : 0)).ToArray()))
                    .OrderBy(x => Network.GetRandom())
                    .ToList();
            return tuples;
        }
    }
}
