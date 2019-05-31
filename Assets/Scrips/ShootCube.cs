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
    private Image miraUIText;
    private GameObject trajectory;
    private MeshRenderer trajectoryMeshRenderer;
    private SpriteRenderer trajectoryMeshRendererAim;

    private void Awake()
    {
        pickupCube = gameObject.GetComponent<PickupPinkBox>();
        camera = GameObject.FindGameObjectWithTag("Camera");
        impulseUIText = GameObject.FindGameObjectWithTag("ImpulseUI").GetComponent<Text>();
        impulseUIText.text = "Impulse: " + impulseStrenth;
        trajectory = GameObject.FindGameObjectWithTag("Trajectory");
        trajectoryMeshRenderer = trajectory.GetComponentInChildren<MeshRenderer>();
        trajectoryMeshRendererAim = trajectoryMeshRenderer.gameObject.GetComponentInChildren<SpriteRenderer>();
        //miraUIText = GameObject.FindGameObjectWithTag("Mira").GetComponent<Image>();
    }

    private void orientPlayer()
    {
        Quaternion aux = camera.transform.rotation;
        float degAng = aux.eulerAngles.y;
        degAng = (degAng < 0) ? (degAng + 180) : (degAng - 180);
        aux = Quaternion.Euler(0, degAng, 0);
        gameObject.transform.rotation = aux;
    }

    private void hideMira()
    {
        trajectoryMeshRenderer.enabled = false;
        trajectoryMeshRendererAim.enabled = false;
    }

    private void showMira()
    {
        trajectoryMeshRenderer.enabled = true;
        trajectoryMeshRendererAim.enabled = true;
    }

    private void updateTrajectory(float impulseStrenthArg)
    {
        Vector3 scaleTraj = trajectory.transform.localScale;
        scaleTraj.x = (impulseStrenthArg / 200f) + 0.025f*(impulseStrenthArg - InitialImpulseStrenght);
        trajectory.transform.localScale = scaleTraj;

        Quaternion cameraRotQ = camera.transform.rotation;
        trajectory.transform.rotation = cameraRotQ;
        trajectory.transform.Rotate(new Vector3(0, 90, 0), Space.Self);

        Vector3 cameraPos = camera.transform.position;
        cameraPos.y = 0.028f * (impulseStrenthArg - InitialImpulseStrenght);
        camera.transform.position = cameraPos;
    }

    private void shootCube()
    {
        if (pickupCube.getCubeStack().ToArray().Length == 0)
        {
            impulseStrenth = InitialImpulseStrenght;

            //updateTrajectory(impulseStrenth);
            updateImpulseUI();
            return;
        }

        orientPlayer();
        pickupCube.updatePinkBoxRotations();
        GameObject cube = pickupCube.removePinkCube();
        shotNextFrame = cube;
        hideMira();
    }

    private void updateImpulseUI()
    {
        impulseUIText.text = "Impulse: " + impulseStrenth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && pickupCube.getCubeStack().ToArray().Length != 0)
        {
            showMira();
            isShooting = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && pickupCube.getCubeStack().ToArray().Length != 0)
        {
            shootCube();
            isShooting = false;
        }

        if (isShooting)
        {
            if (impulseStrenth >= 100)
            {
                impulseStrenth = 100;

                updateTrajectory(impulseStrenth);
            }
            else
            {
                impulseStrenth += impulseStrenthSpeed * Time.deltaTime;

                updateTrajectory(impulseStrenth);
            }
            updateImpulseUI();
        }

    }

    private void checkShootCube()
    {
        if (shotNextFrame != null)
        {

            Rigidbody rbCube = shotNextFrame.GetComponent<Rigidbody>();

            rbCube.AddRelativeForce(new Vector3(0,0.1f,0.7f) * impulseStrenth,ForceMode.Impulse);

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
