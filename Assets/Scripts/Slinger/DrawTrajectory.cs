using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;  // Line renderer to draw the trajectory
    [SerializeField] private int _lineSegmentCount = 20; 
    [SerializeField] private float _maxTrajectoryDistance = 30f; 

    private List<Vector3> _linePoints = new List<Vector3>();

    #region Singleton

    public static DrawTrajectory Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public void UpdateTrajectory(Vector3 forceVelocity, Vector3 startingPoint, Rigidbody rigidBody)
    {
        // Velocity based on force applied and mass of the rigidbody
        Vector3 velocity = (forceVelocity / rigidBody.mass) * Time.fixedDeltaTime;

        float flightDuration = (2 * velocity.y) / Physics.gravity.y;
        
        float stepTime = flightDuration / _lineSegmentCount;
        
        _linePoints.Clear();


        for (int i = 0; i < _lineSegmentCount; i++)
        {
            // Calculate the time passed for each step
            float stepTimePassed = stepTime * i;


            // Calculate the displacement for each point
            Vector3 MovementVector = new Vector3(
                velocity.x * stepTimePassed, 
                velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed, 
                0
            );

            Vector3 nextPoint = startingPoint + MovementVector;

            if (Mathf.Abs(nextPoint.x - startingPoint.x) > _maxTrajectoryDistance)
            {
                break; // Stop adding points if the max X distance is exceeded
            }
            // Add the calculated point to the list
            _linePoints.Add(-MovementVector + startingPoint);
        }

        // Set positions to the LineRenderer
        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(_linePoints.ToArray());
    }

    public void HideLine()
    {
        _lineRenderer.positionCount = 0; // Hide the line by resetting position count
    }
}
