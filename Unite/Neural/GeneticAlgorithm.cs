using System;
using System.Collections.Generic;

namespace Unite.Neural
{
    class GeneticAlgorithm<T>
    {
        // Variables
        #region Variables

        public List<DNA<T>> Population { get; private set; }
        public T[] BestGenes { get; private set; }
        public float BestFitness { get; private set; }
        public int Generation { get; private set; }

        public float mutationRate;
        public int elitism;

        private List<DNA<T>> newPopulation;
        private Random random;
        private float fitnessSum;

        #endregion

        // Override
        #region Override

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction,
            int elitism, float mutationRate = 0.01f)
        {
            Generation = 1;
            this.elitism = elitism;
            this.mutationRate = mutationRate;
            Population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            this.random = random;

            BestGenes = new T[dnaSize];

            for (int i = 0; i < populationSize; i++)
                Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
        }

        #endregion

        // Public
        #region Public

        public void NewGeneration()
        {
            if (Population.Count <= 0)
                return;

            CalculateFitness();
            Population.Sort(CompareDNA);
            newPopulation.Clear();

            for (int i = 0; i < Population.Count; i++)
            {
                if (i < elitism)
                    newPopulation.Add(Population[i]);
                else
                {
                    DNA<T> parent1 = ChooseParent();
                    DNA<T> parent2 = ChooseParent();

                    DNA<T> child = parent1.Crossover(parent2);

                    child.Mutate(mutationRate);

                    newPopulation.Add(child);
                }
            }

            List<DNA<T>> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;

            Generation++;
        }

        public int CompareDNA(DNA<T> a, DNA<T> b)
        {
            if (a.Fitness > b.Fitness)
                return -1;
            else if (a.Fitness < b.Fitness)
                return 1;
            else
                return 0;
        }

        public void CalculateFitness()
        {
            fitnessSum = 0;
            DNA<T> best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(i);

                if (Population[i].Fitness > best.Fitness)
                    best = Population[i];
            }

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        #endregion

        // Private
        #region Private

        private DNA<T> ChooseParent()
        {
            double randomNumber = random.NextDouble() * fitnessSum;

            for (int i = 0; i < Population.Count; i++)
            {
                if (randomNumber < Population[i].Fitness)
                    return Population[i];

                randomNumber -= Population[i].Fitness;
            }

            return null;
        }

        #endregion
    }
}
