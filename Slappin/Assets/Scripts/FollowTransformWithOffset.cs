using UnityEngine;

public class FollowTransformWithOffset : MonoBehaviour
{
    [SerializeField] private Transform transformToFollow;
    
    [SerializeField] private Vector3 offset = new(-1.67f, -1.46f, -1.57f);

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private Vector3 GetGoalPosition()
    {
        return transformToFollow.position - offset;
    }

    private void Update()
    {
        if (previousPosition != GetGoalPosition())
        {
            transform.position = GetGoalPosition();
        }
    }
}

