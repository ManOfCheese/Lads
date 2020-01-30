using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBads : MonoBehaviour
{
    #region Variables
    [Header("Self")]
    public int Hunger;
    public float Speed;
    public States CurrentState;

    public Color WayPointColor;
    public List<GameObject> WayPoints;

    [Header("Target")]
    public GameObject Target;
    public float DistanceToTarget;
    public float ChaseRadius;
    public float TargetDistance;
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, ChaseRadius);

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
        Target = newWayPointTarget;
        SetState(States.Wander);
    }

    public void Wander()
    {
        if (Target == null)
        {
            Debug.Log("No waypoint target. Finding new one");
            SetState(States.FindClosestWayPoint);
            return;
        }
        DistanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
        if (DistanceToTarget < .1f)
        {
            Target = GetNextWayPoint();
            return;
        }
        Debug.Log("Wander");
        transform.LookAt(Target.transform);
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    public void ChaseTarget()
    {
        // Only chases within a certain radius
        // Hunger affects speed while chasing
        // Hunger up == Speed up
        
    }

    public void EatTarget()
    {
        // Hunger -=1
        // Player fuckin dies (disappears)
    }
    #endregion

    #region Functions
    GameObject GetNextWayPoint()
    {
        GameObject CurrentWayPoint;
        GameObject NextWayPoint;
        for (int i = 0; i < WayPoints.Count; i++)
        {
            if (WayPoints[i] == Target)
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
