using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    [SerializeField]
    private int flockSize = 20;

    [SerializeField]
    private float speedModifier = 5;

    [SerializeField]
    private float alignmentWeight = 1;

    [SerializeField]
    private float cohesionWeight = 1;

    [SerializeField]
    private float separationWeight = 1;

    [SerializeField]
    private float followWeight = 5;

    [Header("Boid Data")]
    [SerializeField]
    private Boid prefab;
    [SerializeField]
    private float spawnRadius = 3.0f;
    private Vector3 spawnLocation = Vector3.zero;

    [Header("Target Data")]
    [SerializeField]
    public Transform target;

    private Vector3 flockCenter;

    private Vector3 flockDirection;

    private Vector3 targetDirection;

    private Vector3 separation;

    public List<Boid> flockList = new List<Boid>();

    public float SpeedModifier { get { return speedModifier; } }

    private void Awake()
    {
        flockList = new List<Boid>(flockSize);
        for(int i = 0; i < flockSize; i++)
        {
            spawnLocation = Random.insideUnitSphere * spawnRadius + transform.position;
            Boid boid = Instantiate(prefab, spawnLocation, transform.rotation) as Boid;
            boid.transform.parent = transform;
            boid.FlockController = this;
            flockList.Add(boid);
        }
    }

    public Vector3 Flock(Boid boid, Vector3 boidPosition, Vector3 boidDirection)
    {
        flockDirection = Vector3.zero;
        flockCenter = Vector3.zero;
        targetDirection = Vector3.zero;
        separation = Vector3.zero;

        for(int i = 0; i < flockList.Count; i++)
        {
            Boid neighbor = flockList[i];

            if(neighbor != boid)
            {
                flockDirection += neighbor.Direction;

                flockCenter += neighbor.transform.localPosition;

                separation += neighbor.transform.localPosition - boidPosition;
                separation *= -1;
            }
        }

        flockDirection /= flockSize;
        flockDirection = flockDirection.normalized * alignmentWeight;


        flockCenter /= flockSize;
        flockCenter = flockCenter.normalized * cohesionWeight;


        separation /= flockSize;
        separation = separation.normalized * separationWeight;


        targetDirection = target.localPosition - boidPosition;
        targetDirection = targetDirection * followWeight;

        return flockDirection + flockCenter + separation + targetDirection;
    }
}
