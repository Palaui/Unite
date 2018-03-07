
namespace Unite.Neural
{
    public class NeuralDataSet
    {
        public double[] Values { get; set; }
        public double[] Targets { get; set; }

        public NeuralDataSet(double[] values, double[] targets)
        {
            Values = values;
            Targets = targets;
        }
    }
}
