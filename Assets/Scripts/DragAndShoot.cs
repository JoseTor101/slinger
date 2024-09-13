using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragAndShoot : MonoBehaviour
{
    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    private Rigidbody rb;

    private bool isShooting;
    private bool canShoot; 
    [SerializeField] private float forceMultiplier = 3;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetCanShoot(true);
    }

    private void OnMouseDown()
    {   
        if (!canShoot)
            return; 
        mousePressDownPos = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        if (!canShoot)
            return;

        Vector3 forceInit = (Input.mousePosition - mousePressDownPos);
        Vector3 forceV = new Vector3(forceInit.x, forceInit.y, forceInit.y)*forceMultiplier;

        if(!isShooting)
            DrawTrajectory.Instance.UpdateTrajectory(forceV, transform.position, rb);
    }
    void Update()
    {
        Debug.Log($"Can Shoot: {canShoot}, Is Shooting: {isShooting}");
    }


    private void OnMouseUp()
    {   
        if (!canShoot)
            return;

        DrawTrajectory.Instance.HideLine();
        mouseReleasePos = Input.mousePosition;
        Shoot(mouseReleasePos-mousePressDownPos);
    }

    
    void Shoot(Vector3 Force)
    {
        if(isShooting || !canShoot)    
            return;
        
        rb.AddForce(new Vector3(Force.x,Force.y,Force.y*0) * -forceMultiplier);
        isShooting = true;
        canShoot = false;
    }
    
    public void SetCanShoot(bool value)
    {
        canShoot = value;
        isShooting = !value;
        Debug.Log($"SetCanShoot called. CanShoot is now: {canShoot}");
    }


}