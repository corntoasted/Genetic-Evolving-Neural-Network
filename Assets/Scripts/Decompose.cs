using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decompose : MonoBehaviour
{
    Manager manager;
    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        StartCoroutine(DecomposeSelf());
    }

    IEnumerator DecomposeSelf()
    {
        yield return new WaitForSeconds(manager.trainingTime);
        GameObject.Destroy(gameObject);
    }
}
