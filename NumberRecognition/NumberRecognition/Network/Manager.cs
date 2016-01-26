namespace NumberRecognition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Manager
    {
        private readonly ImageOperations _imgOperations = new ImageOperations();

        private readonly Web _web = new Web(520, 10);

        public IEnumerable<Tuple<byte[], byte[]>> PrepareData()
        {
            var random = new Random();
            var tuples =
                Enumerable.Range(0, 10)
                    .Select(x => new { x, bytes = _imgOperations.GetImagesBytesForDigit(x).ToArray() })
                    .SelectMany(x => x.bytes, (x, b) => new {x.x, b})
                    .Select(x => new Tuple<byte[], byte[]>(x.b, Enumerable.Range(0, 10).Select(ee => (byte)(ee == x.x ? 1 : 0)).ToArray()))
                    .OrderBy(x => random.Next())
                    .ToList();
            return tuples;
        }

        public async Task Learn()
        {
           await Task.Run(
                () =>
                    {
                        var tuples = PrepareData();
                        _web.Teach(tuples, 0.05);
                    });
            
        }

        public List<double> Recognize(byte[] img)
        {
            return _web.Recognize(img);
        }


    }
}
