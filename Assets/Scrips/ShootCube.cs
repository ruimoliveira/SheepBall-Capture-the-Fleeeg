using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootCube : MonoBehaviour
{
    private PickupPinkBox pickupCube;
    private GameObject camera;
    private GameObject shotNextFrame = null;
    private const float InitialImpulseStrenght = 50;
    private float impulseStrenth = InitialImpulseStrenght;
    private const float impulseStrenthSpeed = 20f;
    private bool isShooting = false;
    private Text impulseUIText;

    private void Awake()
    {
        pickupCube = gameObject.GetComponent<PickupPinkBox>();
        camera = GameObject.FindGameObjectWithTag("Camera");
        impulseUIText = GameObject.FindGameObjectWithTag("ImpulseUI").GetComponent<Text>();
        impulseUIText.text = "Impulse: " + impulseStrenth;
    }

    private void orientPlayer()
    {
        Quaternion aux = camera.transform.rotation;
        float degAng = aux.eulerAngles.y;
        degAng = (degAng < 0) ? (degAng + 180) : (degAng - 180);
        aux = Quaternion.Euler(0, degAng, 0);
        gameObject.transform.rotation = aux;
    }

    private void shootCube()
    {
        if (pickupCube.getCubeStack().ToArray().Length == 0)
        {
            impulseStrenth = InitialImpulseStrenght;
            updateImpulseUI();
            return;
        }

        orientPlayer();
        pickupCube.updatePinkBoxRotations();
        GameObject cube = pickupCube.removePinkCube();
        shotNextFrame = cube;
    }

    private void updateImpulseUI()
    {
        impulseUIText.text = "Impulse: " + impulseStrenth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isShooting = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            shootCube();
            isShooting = false;
        }

        if (isShooting)
        {
            if (impulseStrenth >= 100)
                impulseStrenth = 100;
            else
                impulseStrenth += impulseStrenthSpeed * Time.deltaTime;
            updateImpulseUI();
        }

    }

    private void checkShootCube()
    {
        if (shotNextFrame != null)
        {

            Rigidbody rbCube = shotNextFrame.GetComponent<Rigidbody>();

            rbCube.AddRelativeForce(new Vector3(0,0,1.2f) * impulseStrenth,ForceMode.Impulse);

            //rbCube.AddRelativeForce(new Vector3(0, 0, impulseStrenth), ForceMode.Impulse);

            impulseStrenth = InitialImpulseStrenght;
            updateImpulseUI();

            shotNextFrame = null;
        }
    }

    private void FixedUpdate()
    {
        checkShootCube();
    }
}
