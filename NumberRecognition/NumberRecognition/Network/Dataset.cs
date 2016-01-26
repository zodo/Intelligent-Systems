namespace NumberRecognition
{
	public class DataSet
	{
		public double[] Values { get; }
		public double[] Targets { get; }
		

		public DataSet(double[] values, double[] targets)
		{
			Values = values;
			Targets = targets;
		}
		
	}
}