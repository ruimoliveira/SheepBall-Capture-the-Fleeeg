using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCube : MonoBehaviour
{
    PickupPinkBox pickupCube;
    private GameObject camera;

    private void Awake()
    {
        pickupCube = gameObject.GetComponent<PickupPinkBox>();
        camera = GameObject.FindGameObjectWithTag("Camera");
    }

    GameObject shotNextFrame = null;

    private void shootCube()
    {
        Quaternion aux = camera.transform.rotation;
        float degAng = aux.eulerAngles.y;
        degAng = (degAng < 0) ? (degAng + 180) : (degAng - 180);
        aux = Quaternion.Euler(0, degAng, 0); ;
        gameObject.transform.rotation = aux;

        pickupCube.updatePinkBoxRotations();

        GameObject cube = pickupCube.removePinkCube();
        if (cube == null)
            return;

        shotNextFrame = cube;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            shootCube();
        }
    }

    private void FixedUpdate()
    {
        if (shotNextFrame != null)
        {
            Rigidbody rbCube = shotNextFrame.GetComponent<Rigidbody>();
            rbCube.AddRelativeForce(new Vector3(0, 0, 100), ForceMode.Impulse);
            shotNextFrame = null;
        }
    }
}
