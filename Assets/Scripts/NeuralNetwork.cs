using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class NeuralNetwork: IComparable<NeuralNetwork>
{
    public int[] layers { get; set; }
    public float[][] neurons { get; set; }
    public float[][][] weights { get; set; }
    public float fitness { get; set; }

    public Transform position;

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();
    }

    public NeuralNetwork(NeuralNetwork templateNetwork)
    {
        this.layers = new int[templateNetwork.layers.Length];

        for (int i = 0;i < templateNetwork.layers.Length; i++)
        {
            this.layers[i] = templateNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(templateNetwork.weights);
    }

    private void InitNeurons()
    {
        List<float[]> neuronList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++)
        {
            neuronList.Add(new float[layers[i]]);
        }

        neurons = neuronList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();
            int neuronsInPrevLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPrevLayer];

                for (int k = 0; k < neuronsInPrevLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightList.Add(neuronWeights);
            }
            weightList.Add(layerWeightList.ToArray());
        }
        weights = weightList.ToArray();
    }

    private void CopyWeights(float[][][] templateWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = templateWeights[i][j][k];
                }
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0;i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i-1][j][k] * neurons[i-1][k];
                }
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }
        return neurons[neurons.Length-1];
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; ++j)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //Mutation
                    float randomValue = UnityEngine.Random.Range(0, 100);

                    if (randomValue <= 20*mutationRate)
                    {
                        weight *= -1;
                    }
                    else if (randomValue <= 45*mutationRate)
                    {
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomValue <= 70*mutationRate)
                    {
                        float factor = UnityEngine.Random.Range(0, 1) + 1;
                        weight *= factor;
                    }
                    else if (randomValue <= 90*mutationRate)
                    {
                        float factor = UnityEngine.Random.Range(0, 1);
                        weight *= factor;
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    //Fitness
    public void UpdateFitness(float fit)
    {
        fitness += fit;
    }
    public void SetFitness(float fit)
    {
        fitness = fit;
    }
    public float GetFitness()
    {
        return fitness;
    }

    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
        {
            return 1;
        }
        if (fitness > other.fitness)
        {
            return 1;
        }
        else if (fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }



    public void SaveNet(NeuralNetwork net)
    {
        var NetJson = Newtonsoft.Json.JsonConvert.SerializeObject(net);
        string fileName = Directory.GetCurrentDirectory() + @"\Assets\Network\net.nnet";

        if (System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
        }

        System.IO.File.WriteAllText(fileName, NetJson);
        Debug.Log("Network Updated");
    }

    public NeuralNetwork LoadNet(string fileName, NeuralNetwork neuralnet)
    {
        string TextData = File.ReadAllText(fileName);
        var net = Newtonsoft.Json.JsonConvert.DeserializeObject<NetworkConstructor>(TextData);
        NeuralNetwork nnet = new NeuralNetwork(neuralnet);
        nnet.layers = net.layers;
        nnet.neurons = net.neurons;
        nnet.weights = net.weights;
        nnet.fitness = net.fitness;

        return nnet;
    }

    class NetworkConstructor
    {
        public int[] layers;
        public float[][] neurons;
        public float[][][] weights;
        public float fitness;
    }
}
