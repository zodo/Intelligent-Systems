namespace NumberRecognition
{
    using System.Collections.Generic;

    public class Layer
    {
        public int Size { get; set; }
        public List<Neuron> Neurons { get; set; }
        public Layer(int size, int inputs)
        {
            Size = size;
            Neurons = new List<Neuron>(size);
            for (int i = 0; i < size; i++)
            {
                Neurons.Add(new Neuron(inputs));
            }
        }
    }
}
