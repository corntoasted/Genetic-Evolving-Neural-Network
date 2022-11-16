using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickle : MonoBehaviour
{
    public Rigidbody2D[] limbs;
    public HingeJoint2D[] joints;

    public int ID;

    Vector2 currentPos;
    Vector2 prevPos;

    private NeuralNetwork net;
    private bool initialized = false;

    private void Start()
    {
        limbs = GetComponentsInChildren<Rigidbody2D>();
        currentPos = limbs[0].position;
    }

    private void FixedUpdate()
    {
        prevPos = currentPos;
        currentPos = limbs[0].position;
        if (initialized)
        {
            float[] inputs = new float[20];

            TakeInputs(inputs);

            float[] output = net.FeedForward(inputs);

            Move(output);

            

            if (currentPos != prevPos)
            {
                net.UpdateFitness(currentPos.x - prevPos.x);
            }
        }
    }

    private void TakeInputs(float[] inputs)
    {
        for (int i = 1; i < (limbs.Length*2) - 1; i += 2)
        {
            //Rotations
            inputs[i-1] = limbs[((i+1) / 2) - 1].rotation;

            //Angular Velocities
            inputs[i] = limbs[((i + 1) / 2) - 1].angularVelocity;
        }

        inputs[limbs.Length*2] = limbs[0].velocity.x;
        inputs[(limbs.Length*2)+1] = limbs[0].velocity.y;

        inputs[(limbs.Length*2)+2] = limbs[0].position.x;
        inputs[(limbs.Length*2)+3] = limbs[0].position.y;
    }

    public void Move(float[] outputs)
    {
        for (int i = 1; i < (limbs.Length*2)-2; i+=2)
        {
            JointMotor2D jointMotor = joints[((i + 1) / 2) - 1].motor;
            float jointSpeed = outputs[i - 1];
            float jointTorque = outputs[i];
            jointMotor.motorSpeed = 1500f * jointSpeed;
            jointMotor.maxMotorTorque = 1500f * jointTorque;
            joints[((i + 1) / 2) - 1].useMotor = true;
            joints[((i + 1) / 2) - 1].motor = jointMotor;

            /*float muscleForce = outputs[i - 1];
            float targetRotation = outputs[i];

            float angle = Mathf.LerpAngle(limbs[((i + 1) / 2) - 1].rotation, targetRotation, muscleForce*Time.deltaTime);
            if (angle > joints[((i + 1) / 2) - 1].limits.max)
            {
                angle = joints[((i + 1) / 2) - 1].limits.max;
            }
            if (angle < joints[((i + 1) / 2) - 1].limits.min)
            {
                angle = joints[((i + 1) / 2) - 1].limits.min;
            }
            limbs[((i + 1) / 2) -1].MoveRotation(angle);
            Debug.Log(angle);*/
        }
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initialized = true;
    }
}
