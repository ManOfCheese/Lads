using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBads : MonoBehaviour
{
    #region Variables
    [Header("Self")]
    public int Hunger;
    [Range(.1f, 10f)]
    public float Speed = 1;
    public States CurrentState;
    public float WaitingTimer;
    [Tooltip("Waiting time in seconds that the waiting timer resets to upon reaching 0")] public float WaitingTimerResetValue = 3f;

    public Color WayPointColor = Color.yellow;
    public List<GameObject> WayPoints;
    public GameObject TargetWayPoint;
    public float DistanceToWayPoint;
    [Range(.1f, 10f)]
    public float WayPointApproachDistance = .1f;

    [Header("Target")]
    public Player PlayerTarget;
    public Player GetPlayerTarget()
    {
        if (PlayerTarget == null)
        {
            PlayerTarget = FindObjectOfType<Player>();
            if (PlayerTarget == null)
            {
                Debug.LogError("Did not find player");
            }
        }
        return PlayerTarget;
    }
    public float DistanceToPlayer;
    public float ChaseRadius = 1f;
    public float EatRadius = .5f;
    public float TargetDistance;
    #endregion

#region Debug
#if UNITY_ENGINE
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, ChaseRadius);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, EatRadius);

        // Draw lines between waypoints
        if (WayPoints == null || WayPoints.Count <= 1) return;
        for (int i = 0; i < WayPoints.Count; i++)
        {
            GameObject CurrentWayPoint = WayPoints[i];
            GameObject NextWayPoint;
            if (i + 1 >= WayPoints.Count)
                NextWayPoint = WayPoints[0];
            else
                NextWayPoint = WayPoints[i + 1];
            Gizmos.color = WayPointColor;
            Gizmos.DrawLine(CurrentWayPoint.transform.position, NextWayPoint.transform.position);
        }
    }
#endif
#endregion

#region States
#region State Setup
    public enum States
    {
        FindClosestWayPoint,
        Wander,
        ChaseTarget,
        EatTarget,
        Wait
    }

    public void SetState(States pState)
    {
        CurrentState = pState;
    }
#endregion

    public void FindClosestWayPoint()
    {
        GameObject newWayPointTarget = null;
        float closestDistance = 999f;

        foreach (GameObject WayPoint in WayPoints)
        {
            newWayPointTarget = WayPoint;
            if (newWayPointTarget != null)
            {
                float distance = Vector3.Distance(transform.position, WayPoint.transform.position);
                if (distance < closestDistance)
                {
                    newWayPointTarget = WayPoint;
                }
            }
        }
        TargetWayPoint = newWayPointTarget;
        SetState(States.Wander);
    }

    public void Wander()
    {
        if (TargetWayPoint == null)
        {
            Debug.Log("No waypoint target. Finding new one");
            SetState(States.FindClosestWayPoint);
            return;
        }
        DistanceToWayPoint = Vector3.Distance(transform.position, TargetWayPoint.transform.position);


        Player lPlayerTarget = GetPlayerTarget();
        if (lPlayerTarget != null)
        {
            DistanceToPlayer = Vector3.Distance(transform.position, lPlayerTarget.transform.position);
            if (DistanceToPlayer < ChaseRadius)
            {
                PlayerTarget = lPlayerTarget;
                SetState(States.ChaseTarget);
                return;
            }
        }

        if (DistanceToWayPoint < WayPointApproachDistance)
        {
            TargetWayPoint = GetNextWayPoint();
            return;
        }
        Debug.Log("Wander");
        transform.LookAt(TargetWayPoint.transform);
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    public void ChaseTarget()
    {
        Player lPlayerTarget = GetPlayerTarget();
        if (lPlayerTarget == null)
        {
            SetState(States.FindClosestWayPoint);
            return;
        }
        DistanceToPlayer = Vector3.Distance(transform.position, lPlayerTarget.transform.position);
        if (DistanceToPlayer > ChaseRadius)
        {
            Debug.Log("Player out of range. Finding closest waypoint");
            SetState(States.FindClosestWayPoint);
            return;
        }
        if (DistanceToPlayer < EatRadius)
        {
            Debug.Log("Eat player");
            SetState(States.EatTarget);
            return;
        }
        Debug.Log("Following player");
        transform.LookAt(PlayerTarget.transform);
        transform.position += transform.forward * Speed * Time.deltaTime;

        // Hunger affects speed while chasing
        // Hunger up == Speed up

    }

    public void EatTarget()
    {
        Hunger -= 1;
        Player lPlayerTarget = GetPlayerTarget();
        if (lPlayerTarget == null)
        {
            Debug.LogWarning("Player target is null during eat target state");
            SetState(States.FindClosestWayPoint);
            return;
        }
        // Player fuckin dies (disappears)
        Debug.Log("Eat the rich");
        lPlayerTarget.KillBottomLad();
        WaitingTimer = WaitingTimerResetValue;
        SetState(States.Wait);
    }

    public void Wait()
    {
        WaitingTimer -= Time.deltaTime;
        if (WaitingTimer <= 0f)
        {
            WaitingTimer = WaitingTimerResetValue;
            SetState(States.FindClosestWayPoint);
            return;
        }
    }
#endregion

#region Functions
    GameObject GetNextWayPoint()
    {
        GameObject CurrentWayPoint;
        GameObject NextWayPoint;
        for (int i = 0; i < WayPoints.Count; i++)
        {
            if (WayPoints[i] == TargetWayPoint)
            {
                CurrentWayPoint = WayPoints[i];
                if (i + 1 >= WayPoints.Count)
                    NextWayPoint = WayPoints[0];
                else
                    NextWayPoint = WayPoints[i + 1];
                return NextWayPoint;
            }
        }
        return null; 
    }
#endregion


#region Unity Functions
    private void Awake()
    {
        SetState(States.FindClosestWayPoint);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case States.FindClosestWayPoint:
                FindClosestWayPoint();
                break;
            case States.Wander:
                Wander();
                break;
            case States.ChaseTarget:
                ChaseTarget();
                break;
            case States.EatTarget:
                EatTarget();
                break;
            case States.Wait:
                Wait();
                break;
            default:
                break;
        }
    }
#endregion
}
