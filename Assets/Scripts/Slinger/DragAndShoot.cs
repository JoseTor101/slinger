using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragAndShoot : MonoBehaviour
{
    [SerializeField] private AudioClip stretchSound;
    [SerializeField] private float forceMultiplier = 3;
    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;
    private Rigidbody rb;
    private AudioSource audioSource;

    private bool isShooting;
    private bool canShoot;
    private bool soundPlayedDuringDrag = false;


    void Start()
    {

        isShooting = false;
        canShoot = true;
        rb = GetComponent<Rigidbody>();
        SetCanShoot(true);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = stretchSound;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnMouseDown()
    {
        if (!canShoot)
            return;
        mousePressDownPos = Input.mousePosition;
        soundPlayedDuringDrag = false;
    }

    private void OnMouseDrag()
    {
        if (!canShoot)
            return;

        Vector3 forceInit = (Input.mousePosition - mousePressDownPos);
        Vector3 forceV = new Vector3(forceInit.x, forceInit.y, forceInit.y) * forceMultiplier;

        if (!isShooting)
            DrawTrajectory.Instance.UpdateTrajectory(forceV, transform.position, rb);

        if (!soundPlayedDuringDrag)
        {
            audioSource.Play();
            soundPlayedDuringDrag = true;
        }
    }

    private void OnMouseUp()
    {
        if (!canShoot)
            return;

        DrawTrajectory.Instance.HideLine();
        mouseReleasePos = Input.mousePosition;
        Shoot(mouseReleasePos - mousePressDownPos);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void Shoot(Vector3 Force)
    {
        if (isShooting || !canShoot)
            return;

        rb.AddForce(new Vector3(Force.x, Force.y, Force.y * 0) * -forceMultiplier);

        isShooting = true;
        canShoot = false;

    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
        isShooting = !value;
    }

    public bool GetCanShoot()
    {
        return canShoot;
    }

    public bool GetIsShooting()
    {
        return isShooting;
    }

}
