using System;

namespace Unite.Neural
{
    public class DNA<T>
    {
        // Variables
        #region Variables

        public T[] Genes { get; private set; }
        public float Fitness { get; private set; }

        private Random random;
        private Func<T> getRandomGene;
        private Func<int, float> fitnessFunction;

        #endregion

        // Override
        #region Override

        public DNA(int size, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
        {
            Genes = new T[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;

            if (shouldInitGenes)
            {
                for (int i = 0; i < Genes.Length; i++)
                    Genes[i] = getRandomGene();
            }
        }

        #endregion

        // Public
        #region Public

        public DNA<T> Crossover(DNA<T> otherParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

            for (int i = 0; i < Genes.Length; i++)
                child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];

            return child;
        }

        public float CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                    Genes[i] = getRandomGene();
            }
        }

        #endregion

    }
}