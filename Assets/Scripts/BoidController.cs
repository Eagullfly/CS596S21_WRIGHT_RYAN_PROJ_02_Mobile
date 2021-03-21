using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BoidController : MonoBehaviour
{
    [System.Serializable]
    public class ddvals
    {
        [Header("Behavior weights")]
        [Range(0f, 50f)]
        public float speed = 1.0f;
        [Range(-10f, 10f)]
        public float separateWeight = 1.0f;
        [Range(-10f, 10f)]
        public float alignmentWeight = 1.0f;
        [Range(-10f, 10f)]
        public float cohesionWeight = 1.0f;
        [Range(-10f, 10f)]
        public float waypointWeight = 1.0f;
    }

    public ddvals edval;

    public float minVelocity = 5f;
    public float maxVelocity = 20f;
    public float randomness = 0.15f;
    public float flockRadius = 10f;
    public int flockSize = 20;

    public Vector3 flockCenter;
    public Vector3 flockVelocity;

    [HideInInspector] [NonSerialized]
    public Transform waypoint;
    private Rigidbody rb;
    public Vector3 heading { get { return transform.forward; } }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 steeringPressure = Vector3.zero;
        IList<BoidController> flock = GetBoidsWithin(flockRadius);
        Dictionary<string, Vector3> flockVariable = new Dictionary<string, Vector3>();

        flockVariable = CalcFlockingVars(flock);
        flockCenter = flockVariable["flockCenter"];

        steeringPressure += Random.insideUnitSphere * randomness;
        steeringPressure += flockVariable["separation"] * edval.separateWeight;
        steeringPressure += flockVariable["alignment"] * edval.alignmentWeight;
        steeringPressure += flockVariable["cohesion"] * edval.cohesionWeight;
        steeringPressure += (waypoint.position - transform.position).normalized * edval.waypointWeight;

        float stepSize = steeringPressure.magnitude * Mathf.Deg2Rad;
        Vector3 finalHeading = Vector3.RotateTowards(heading, steeringPressure, stepSize, 0.0f);
        Quaternion rotation = Quaternion.LookRotation(finalHeading);

        transform.rotation = rotation;

        rb.velocity = edval.speed * heading;
        Debug.DrawRay(transform.position, heading);
    }

    Dictionary<string, Vector3> CalcFlockingVars(IList<BoidController> flock)
    {
        Dictionary<string, Vector3> flockingVariables = new Dictionary<string, Vector3>();
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 tracking = Vector3.zero;
        Vector3 fleeing = Vector3.zero;
        Vector3 avgAlignment = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        Vector3 deltaPosition = Vector3.zero;

        if(flock.Count == 0)
        {
            flockingVariables["alignment"] = Vector3.zero;
            flockingVariables["cohesion"] = Vector3.zero;
            flockingVariables["separation"] = Vector3.zero;
            flockingVariables["flockCenter"] = Vector3.zero;
            return flockingVariables;
        }

        foreach(BoidController boid in flock)
        {
            avgAlignment += boid.heading;
            avgPosition += boid.transform.position;
            deltaPosition = transform.position - boid.transform.position;
            if(deltaPosition.magnitude == 0)
            {
                deltaPosition = Random.onUnitSphere;
            }
            separation += deltaPosition / deltaPosition.magnitude;
        }

        avgAlignment = avgAlignment.normalized;
        flockingVariables["alignment"] = (avgAlignment * Vector3.Angle(heading, avgAlignment)) / 20f;

        avgPosition /= flock.Count;
        flockingVariables["flockCenter"] = avgPosition;
        flockingVariables["cohesion"] = avgPosition - transform.position;

        flockingVariables["separation"] = separation;
        flockingVariables["seek"] = (waypoint.position - transform.position).normalized;

        return flockingVariables;
    }

    private IList<BoidController> GetBoidsWithin(float radius)
    {
        var neighbors = FindComponentsInSphere<BoidController>(transform.position, radius);
        neighbors.Remove(this);
        return neighbors;
    }

    public static GameObject[] FindObjectsInSphere(Vector3 point, float radius)
    {
        var hits = Physics.OverlapSphere(point, radius);
        GameObject[] objects = new GameObject[hits.Length];
        for(int i = 0; i < hits.Length; ++i)
        {
            objects[i] = hits[i].gameObject;
        }
        return objects;
    }

    public IList<T> FindComponentsInSphere<T>(Vector3 point, float radius)
    {
        var components = new List<T>();
        var gameObjects = FindObjectsInSphere(point, radius);
        foreach(GameObject gameObject in gameObjects)
        {
            var component = gameObject.GetComponent<T>();
            if(component != null)
            {
                components.Add(component);
            }
        }
        return components;
    }
}
