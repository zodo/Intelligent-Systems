namespace NumberRecognition
{
    using System.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class Web
    {
        private const int Depth = 3;
        private const int InnerLayerSize = 60;
        private readonly int _outputs;
        private readonly int _inputs;
        private double _error;
        private readonly List<Layer> _layers;

        public Web(int inputs, int outputs)
        {
            _inputs = inputs;
            _outputs = outputs;
            _layers = new List<Layer>(Depth + 2) { new Layer(_inputs, 0) };
            for (var i = 1; i < Depth - 1; i++)
            {
                _layers.Add(new Layer(InnerLayerSize, _layers[i - 1].Size));
            }
            _layers.Add(new Layer(_outputs, InnerLayerSize));
        }

        public void Teach(IEnumerable<Tuple<byte[], byte[]>> tests, double error)
        {
            var tuples = tests.ToList();
            var currError = 0.0;

            do
            {
                tuples.ForEach(tuple =>
                    {
                        GoForward(tuple.Item1);
                        PrepareOutputLayer(tuple.Item2);
                        ErrorPropagation();
                        UpdateWeights();
                        currError += _error;
                    });
                currError /= tuples.Count;
                Debug.WriteLine("Ошибка: " + currError);
            }
            while (currError > error);
        }

        public List<double> Recognize(byte[] input)
        {
            var answer = new List<double>(_outputs);
            GoForward(input);
            for (int i = 0; i < _outputs; i++)
            {
                answer.Add(_layers.Last().Neurons[i].Output);
            }
            return answer;
        }

        private void GoForward(byte[] test)
        {
            PrepareInputLayer(test);
            CalculateNeuronsOutputs();
        }

        private void CalculateNeuronsOutputs()
        {
            for (int i = 1; i < Depth; i++)
            {
                for (int j = 0; j < _layers[i].Size; j++)
                {
                    var arg = 0.0;
                    for (int k = 0; k < _layers[i].Neurons[j].Inputs; k++)
                    {
                        arg += (_layers[i - 1].Neurons[k].Output * _layers[i].Neurons[j].Weights[k]);
                    }
                    _layers[i].Neurons[j].Output = ActivationFunc(arg);
                }
            }
        }

        private static double ActivationFunc(double arg)
        {
            return 1.0 / (1.0 + Math.Exp(-1.0 * arg));
        }

        private void PrepareInputLayer(byte[] test)
        {
            for (int i = 0; i < _inputs; i++)
            {
                _layers[0].Neurons[i].Output = test[i];
            }
        }

        private void UpdateWeights()
        {
            for (int i = 0; i < Depth; i++)
            {
                for (int j = 0; j < _layers[i].Size; j++)
                {
                    for (int k = 0; k < _layers[i].Neurons[j].Inputs; k++)
                    {
                        _layers[i].Neurons[j].Weights[k] += _layers[i].Neurons[j].Deltas[k];
                    }
                }
            }
        }

        private void ErrorPropagation()
        {
            for (int i = Depth - 2; i >= 0; i--)
            {
                for (int j = 0; j < _layers[i].Size; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < _layers[i + 1].Size; k++)
                    {
                        sum += _layers[i + 1].Neurons[k].Delta * _layers[i + 1].Neurons[k].Weights[j];
                    }
                    _layers[i].Neurons[j].Delta = 
                        sum * _layers[i].Neurons[j].Output * (1.0 - _layers[i].Neurons[j].Output);

                    for (int k = 0; k < _layers[i].Neurons[j].Inputs; k++)
                    {
                        _layers[i].Neurons[j].Deltas[k] = 0.5* (_layers[i].Neurons[j].Deltas[k]
                                                                + _layers[i].Neurons[j].Delta
                                                                * _layers[i - 1].Neurons[k].Output);
                    }
                }
            }
        }

        private void PrepareOutputLayer(byte[] testAnswer)
        {
            _error = 0;
            for (int i = 0; i < _outputs; i++)
            {
                var currOut = _layers.Last().Neurons[i].Output;
                _error += (testAnswer[i] - currOut) * (testAnswer[i] - currOut);
                _layers.Last().Neurons[i].Delta = (testAnswer[i] - currOut) * currOut * (1.0 - currOut);
                for (int j = 0; j < _layers.Last().Neurons[i].Inputs; j++)
                {
                    _layers.Last().Neurons[i].Deltas[j] = 0.5 * (_layers.Last().Neurons[i].Deltas[j]
                                                                + _layers.Last().Neurons[i].Delta
                                                                * _layers[Depth - 2].Neurons[j].Output);
                }
            }
            _error /= 2;

        }
    }
}
