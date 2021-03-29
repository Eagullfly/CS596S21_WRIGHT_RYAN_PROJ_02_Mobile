using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlocking : MonoBehaviour
{
    private GameObject Controller;
    private bool inited = false;
    private float minVelocity;
    private float maxVelocity;
    private float randomness;
    private GameObject chasee;
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine("BoidSteering");
    }

    IEnumerator BoidSteering()
    {
        while (true)
        {
            if (inited)
            {
                rb.velocity = rb.velocity + Calc() * Time.deltaTime;

                float speed = rb.velocity.magnitude;
                if(speed > maxVelocity)
                {
                    rb.velocity = rb.velocity.normalized * maxVelocity;
                }
                else if(speed < minVelocity)
                {
                    rb.velocity = rb.velocity.normalized * minVelocity;
                }
            }

            float waitTime = Random.Range(0.3f, 0.5f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Vector3 Calc()
    {
        Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);

        randomize.Normalize();
        BoidController boidController = Controller.GetComponent<BoidController>();
        Vector3 flockCenter = boidController.flockCenter;
        Vector3 flockVelocity = boidController.flockVelocity;
        Vector3 follow = chasee.transform.localPosition;

        flockCenter = flockCenter - transform.localPosition;
        flockVelocity = flockVelocity - rb.velocity;
        follow = follow - transform.localPosition;

        return (flockCenter + flockVelocity + follow * 2 + randomize * randomness);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
