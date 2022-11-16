using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    private Manager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null)
        {
            return;
        }
        int layer = LayerMask.NameToLayer("floor");
        if (collision.gameObject.layer == layer)
        {
            if (gameObject.GetComponentInParent<Pickle>() == null) return;
            int id = gameObject.GetComponentInParent<Pickle>().ID;
            manager.nets[id].UpdateFitness(-0.5f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision == null)
        {
            return;
        }
        int layer = LayerMask.NameToLayer("floor");
        if (collision.gameObject.layer == layer)
        {
            if (gameObject.GetComponentInParent<Pickle>() == null)
            {
                return;
            }
            int id = gameObject.GetComponentInParent<Pickle>().ID;
            manager.nets[id].UpdateFitness(-0.03f);
            /*Debug.Log("Cuncussion");*/
        }
    }
}
