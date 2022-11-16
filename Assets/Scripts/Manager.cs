using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;


public class Manager : MonoBehaviour
{
    public GameObject picklePrefab;
    public GameObject start;
    public GameObject camera;

    [Header("Network Data")]
    private bool isTraining = false;
    public int populationSize = 100;
    public float trainingTime = 20f;
    public float generationNumber = 0;
    private int[] layers = new int[] { 20, 20, 20, 14 };

    [Header("Fitness")]
    public float HighestFitnessOfGeneration;

    [HideInInspector]
    public List<NeuralNetwork> nets;
    private List<Pickle> pickles;
    private float[] fitnesses;

    private int bestNet;

    private void Update()
    {
        /*Time.timeScale = 1.5f;*/
        if (!isTraining)
        {
            if (generationNumber == 0)
            {
                InitPickleBrains();
            }
            else
            {
                nets.Sort();
                Debug.Log(nets[nets.Count-1].fitness);
                HighestFitnessOfGeneration = nets[nets.Count-1].fitness;
                if (generationNumber % 10 == 0) { nets[nets.Count - 1].SaveNet(nets[nets.Count - 1]); }

                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[nets.Count - 1]);
                    nets[i].Mutate(0.45f);

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i + (populationSize / 2)].Mutate(0.001f);
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;
            isTraining = true;
            Invoke("Timer", trainingTime);
            CreateBodies();
        }
        /*bestNet
            = !fitnesses.Any() ? 0 : fitnesses.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;*/
        Debug.Log(bestNet);/*


        float highest = -10000000000000;
        int which = 0;
        //Camera
        for (int i = 0; i < populationSize - 1; i++)
        {
            float current = nets[i].GetFitness();
            if (current > highest)
            {
                highest = current;
                which = i;
                return;
            }
            else
            {
                pickles[i].gameObject.SetActive(false);
            }
        }
        camera.transform.position = new Vector3(pickles[which].limbs[0].transform.position.x, camera.transform.position.y, camera.transform.position.z);*/
    }

    void Timer()
    {
        isTraining = false;
    }

    void CreateBodies()
    {
        if (pickles != null)
        {
            for (int i = 0; i < pickles.Count; i++)
            {
                GameObject.Destroy(pickles[i]);
            }
        }

        pickles = new List<Pickle>();

        for (int i = 0; i < populationSize; i++)
        {
            Pickle pickle = ((GameObject)Instantiate(picklePrefab, start.transform.position, Quaternion.identity)).GetComponent<Pickle>();
            pickle.Init(nets[i]);
            pickle.ID = i;
            pickles.Add(pickle);
        }
    }

    void InitPickleBrains()
    {
        if (populationSize % 2 != 0)
        {
            populationSize -= 1;
        }

        nets = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate(0.68f);

            string fileName = Directory.GetCurrentDirectory() + @"\Assets\Network\net.nnet";

            if (File.Exists(fileName))
            {
                net = net.LoadNet(fileName, net);
            }
            nets.Add(net);
        }
    }

    private void OnApplicationQuit()
    {
        nets.Sort();
        /*nets[nets.Count-1].SaveNet(nets[nets.Count - 1]);*/
        Debug.Log("Process Terminated");
    }
}
