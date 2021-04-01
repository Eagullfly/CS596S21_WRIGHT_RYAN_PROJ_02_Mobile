using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField]
    private FlockController flockController;

    //The modified direction for the boid.
    private Vector3 targetDirection;
    //The Boid's current direction.
    private Vector3 direction;
    private string currentMode;

    public FlockController FlockController
    {
        get { return flockController; }
        set { flockController = value; }
    }

    public Vector3 Direction { get { return direction; } }

    private void Awake()
    {
        direction = transform.forward.normalized;
        if (flockController != null)
        {
            Debug.LogError("You must assign a flock controller!");
        }
        currentMode = null;
    }

    private void Update()
    {

        targetDirection = FlockController.Flock(this, transform.localPosition, direction);

        if (flockController.GetMode() == "follow")
        {
            targetDirection += FlockController.FollowTarget(this);
        }
        else if (flockController.GetMode() == "circle")
        {
            targetDirection = FlockController.CircleATree(this);
        }
        else if (flockController.GetMode() == "lazy")
        {
            targetDirection = FlockController.LazyFlight(this);
        }

        if (targetDirection == Vector3.zero)
        {
            return;
        }
        direction = targetDirection.normalized;
        direction *= flockController.SpeedModifier;
        transform.Translate(direction * Time.deltaTime);
    }

    public void ToggleCircle()
    {
        currentMode = "circle";
    }

    public void ToggleLazy()
    {
        currentMode = "lazy";
    }
}
