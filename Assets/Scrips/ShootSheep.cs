using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootSheep : MonoBehaviour
{
    private PickupSheep pickupSheep;
    private GameObject camera;
    private GameObject shotNextFrame = null;
    private const float InitialImpulseStrenght = 25;
    private const float InitialImpulseAccel = 5f;
    private const float InitialImpulseSpeed = 10f;
    private float impulseStrenth = InitialImpulseStrenght;
    private float impulseStrenthSpeed = InitialImpulseSpeed;
    private float impulseAccel = InitialImpulseAccel;
    private bool isShooting = false;
    private Text impulseUIText;
    private Image miraUIText;
    private GameObject trajectory;
    private MeshRenderer trajectoryMeshRenderer;
    private SpriteRenderer trajectoryMeshRendererAim;

    private void Awake()
    {
        pickupSheep = gameObject.GetComponent<PickupSheep>();
        camera = GameObject.FindGameObjectWithTag("Camera");
        impulseUIText = GameObject.FindGameObjectWithTag("ImpulseUI").GetComponent<Text>();
        impulseUIText.text = "Impulse: " + impulseStrenth;
        trajectory = GameObject.FindGameObjectWithTag("Trajectory");
        trajectoryMeshRenderer = trajectory.GetComponentInChildren<MeshRenderer>();
        trajectoryMeshRendererAim = trajectoryMeshRenderer.gameObject.GetComponentInChildren<SpriteRenderer>();

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
        cameraPos.y = 0.028f * (impulseStrenthArg - InitialImpulseStrenght) + transform.position.y;
        camera.transform.position = cameraPos;
    }

    private void shootSheep()
    {
        if (pickupSheep.getSheepStack().Count == 0)
        {

            impulseAccel = InitialImpulseAccel;
            impulseStrenthSpeed = InitialImpulseSpeed;
            impulseStrenth = InitialImpulseStrenght;

            updateImpulseUI();
            return;
        }

        orientPlayer();
        pickupSheep.updateSheepRotation();
        GameObject sheep = pickupSheep.dropSheep();
        shotNextFrame = sheep;
        hideMira();
    }

    private void updateImpulseUI()
    {
        impulseUIText.text = "Impulse: " + impulseStrenth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && pickupSheep.getSheepStack().Count != 0)
        {
            showMira();
            isShooting = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && pickupSheep.getSheepStack().Count != 0)
        {
            shootSheep();
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
                impulseStrenthSpeed += impulseAccel * Time.deltaTime;
                impulseStrenth += impulseStrenthSpeed * Time.deltaTime;

                updateTrajectory(impulseStrenth);
            }
            updateImpulseUI();
        }

    }

    private void checkShootSheep()
    {
        if (shotNextFrame != null)
        {
            Rigidbody rbSheep = shotNextFrame.GetComponent<Rigidbody>();

            //rbSheep.AddForceAtPosition(new Vector3(0, 0.1f, 1.2f) * impulseStrenth, shotNextFrame.transform.position, ForceMode.Impulse);
            rbSheep.AddRelativeForce(new Vector3(0,0.1f,-1.2f) * impulseStrenth, ForceMode.Impulse);

            impulseStrenth = InitialImpulseStrenght;
            impulseStrenthSpeed = InitialImpulseSpeed;
            impulseAccel = InitialImpulseAccel;
            updateImpulseUI();

            shotNextFrame = null;
        }
    }

    private void FixedUpdate()
    {
        checkShootSheep();
    }
}
