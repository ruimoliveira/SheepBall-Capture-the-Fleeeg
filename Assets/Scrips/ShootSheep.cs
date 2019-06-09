using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SheepAnimationState;
using PlayerManager;
using UnityStandardAssets.Characters.ThirdPerson;

public class ShootSheep : NetworkMessageHandler
{
    private PickupSheep pickupSheep;
    public GameObject camera;
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
    private GameObject[] baseWalls;
    private bool stunned = false;

    private void Awake()
    {
        pickupSheep = gameObject.GetComponent<PickupSheep>();
        impulseUIText = GameObject.FindGameObjectWithTag("ImpulseUI").GetComponent<Text>();
        impulseUIText.text = "Impulse: " + impulseStrenth;
        trajectory = transform.Find("trajectory").gameObject;
        trajectoryMeshRenderer = trajectory.GetComponentInChildren<MeshRenderer>();
        trajectoryMeshRendererAim = trajectoryMeshRenderer.gameObject.GetComponentInChildren<SpriteRenderer>();
        baseWalls = GameObject.FindGameObjectsWithTag(Constants.BASE_WALL_TAG);
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

    private IEnumerator OnCollisionEnter(Collision collision)
    {
        if (stunned)
            yield return 0;

        int sheep_state;
        if (collision.collider.tag == Constants.SHEEP_TAG)
        {
            sheep_state = collision.collider.gameObject.GetComponent<SheepMovement>().getState();
            if(sheep_state == (int)State.Flying)
            {
                stun();
                yield return new WaitForSeconds(3f);
                unstun();
            }
        }
    }

    private void stun()
    {
        Debug.Log("stunned");
        toggleInputScripts(false);
        // GetComponent<PickupSheep>().dropAllSheep();
        // make sheep run away
        GetComponent<Rigidbody>().isKinematic = true;
        // TO DO: change player animation
        stunned = true;
    }

    private void unstun()
    {
        Debug.Log("UNstunned");
        stunned = false;
        // TO DO: change player animation
        GetComponent<Rigidbody>().isKinematic = false;
        toggleInputScripts(true);
    }

    //impedir jogador nao local de receber input
    private void toggleInputScripts(bool enable)
    {
        GetComponent<ThirdPersonUserControl>().enabled = enable;
        GetComponent<ThirdPersonCharacter>().enabled = enable;
        GetComponent<PickupSheep>().enabled = enable;
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
        hideMira();

        GameObject sheep = pickupSheep.prepareToShoot();

        SheepMovement sheep_movement = sheep.GetComponentInChildren<SheepMovement>();
        Animator sheep_animator = sheep.GetComponentInChildren<Animator>();
        Rigidbody sheep_rb = sheep.GetComponent<Rigidbody>();

        // set state to flying and anim state to shot
        sheep_movement.setFlying();
        IAnimState animState = new Shot(ref sheep_animator);
        sheep_movement.SetAnimState(animState);

        sheep_rb.velocity = Vector3.zero;
        sheep_rb.useGravity = true;

        // sheepCollideWithBases(sheep);
        sheepCollideWithPlayers(sheep);
        sheep_movement.setPickedUpBy("");

        SendShootSheepMessage(sheep.transform.name, sheep_movement.getState(), sheep_animator.GetInteger("Index"), impulseStrenth);

        resetImpulseAndUI();
    }

    public void SendShootSheepMessage(string _sheepID, int state, int anim_index, float impulse)
    {
        ShootSheepMessage _msg = new ShootSheepMessage()
        {
            playerName = transform.name,
            sheepName = _sheepID,
            sheepState = state,
            sheepAnimation = anim_index,
            impulse = impulse
        };

        NetworkManager.singleton.client.Send(shoot_sheep_message, _msg);
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

    private void resetImpulseAndUI()
    {
        impulseStrenth = InitialImpulseStrenght;
        impulseStrenthSpeed = InitialImpulseSpeed;
        impulseAccel = InitialImpulseAccel;
        updateImpulseUI();
    }

    private void sheepCollideWithBases(GameObject sheep)
    {
        foreach (GameObject wall in baseWalls)
        {
            Physics.IgnoreCollision(sheep.GetComponent<Collider>(), wall.GetComponent<Collider>(), false);
        }
    }

    private void sheepCollideWithPlayers(GameObject sheep)
    {
        GameObject player;
        foreach (GameObject p in Manager.Instance.GetConnectedPlayers())
        {
            player = p.transform.Find("Graphics").gameObject;
            Physics.IgnoreCollision(sheep.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        }
    }
}
