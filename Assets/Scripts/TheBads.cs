using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBads : MonoBehaviour
{
    #region Variables
    [Header("Self")]
    public int Hunger;
    public float Speed = 1;
    public States CurrentState;

    public Color WayPointColor;
    public List<GameObject> WayPoints;
    public GameObject TargetWayPoint;

    [Header("Target")]
    public Player PlayerTarget;
    public Player GetPlayerTarget()
    {
        PlayerTarget = FindObjectOfType<Player>();
        if (PlayerTarget == null)
        {
            Debug.LogError("Did not find player");
        }
        return PlayerTarget;
    }
    public float DistanceToWayPoint;
    public float DistanceToPlayer;
    public float ChaseRadius = 1f;
    public float EatRadius = .5f;
    public float TargetDistance;
    #endregion

    #region Debug
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
    #endregion

    #region States
    #region State Setup
    public enum States
    {
        FindClosestWayPoint,
        Wander,
        ChaseTarget,
        EatTarget
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

        if (DistanceToWayPoint < .1f)
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
        lPlayerTarget.gameObject.SetActive(false);
        SetState(States.FindClosestWayPoint);
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
            default:
                break;
        }
    }
    #endregion
}
