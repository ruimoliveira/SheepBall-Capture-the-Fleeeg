using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Cameras;

public class NetworkPlayer : NetworkMessageHandler
{
    [Header("Player Properties")]
    public string playerID;

    [Header("Player Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

    [Header("Lerping Properties")]
    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public Vector3 realPosition;
    public Quaternion realRotation;
    public Vector3 lastRealPosition;
    public Quaternion lastRealRotation;
    public float timeStartedLerping;
    public float timeToLerp;

    private GameObject sheepManager;

    private void Start()
    {
        if(isLocalPlayer)
            RegisterNetworkMessages();

        Debug.Log("NetworkPlayer START");

        playerID = "player" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;

        Manager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);

        // Ignore collision between base and players
        GameObject[] baseWalls = GameObject.FindGameObjectsWithTag(Constants.BASE_WALL_TAG);
        Collider playerCollider = transform.Find("Graphics").GetComponent<Collider>();
        Debug.Log("num bases " + baseWalls.Length);
        Debug.Log("player collider " + playerCollider);
        foreach (GameObject baseWall in baseWalls)
        {
            Debug.Log("base: ");
            Debug.Log(baseWall);
            Collider c = GetComponentInChildren<Collider>();
            Physics.IgnoreCollision(baseWall.GetComponentInChildren<Collider>(), playerCollider);
        }

        if (isLocalPlayer)
        {
            Manager.Instance.SetLocalPlayerID(playerID);

            sheepManager = GameObject.FindGameObjectWithTag("SheepManager");
            Debug.Log("sheepManager" + sheepManager);
            //Camera.main.transform.position = transform.position + new Vector3(0, 0, -20);
            //Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);

            canSendNetworkMovement = false;
            
        }
        else
        {
            disableLocalPlayerScripts();

            //inicializar variaveis lerp
            isLerpingPosition = false;
            isLerpingRotation = false;

            realPosition = this.transform.Find("Graphics").GetComponent<Transform>().position;
            realRotation = this.transform.Find("Graphics").GetComponent<Transform>().rotation;
        }
    }

    //impedir jogador nao local de receber input
    private void disableLocalPlayerScripts()
    {
        GetComponentInChildren<ThirdPersonUserControl>().enabled = false;
        GetComponentInChildren<ThirdPersonCharacter>().enabled = false;
        GetComponentInChildren<FreeLookCam>().enabled = false;
        GetComponentInChildren<ProtectCameraFromWallClip>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
    }

    //associar o receber de uma dada mensagem a um handler
    private void RegisterNetworkMessages()
    {
        //player movement
        NetworkManager.singleton.client.RegisterHandler(player_movement_msg, OnReceiveMovementMessage);
        //sheep movement
        Debug.Log("Registei o handler das sheep");
        NetworkManager.singleton.client.RegisterHandler(sheep_movement_msg, OnReceiveSheepMovementMessage);
    }

    //recebe do servidor movement dos outros players
    private void OnReceiveMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();

        //verifica se a mensagem NAO é a resposta do proprio jogador
        if (_msg.objectTransformName != transform.name)
        {
            //aceder ao player unit de quem enviou a mensagem e atualizar os valores desse jogador
            Manager.Instance.ConnectedPlayers[_msg.objectTransformName].GetComponent<NetworkPlayer>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }

    //atualizar variaveis de lerping vindas de uma mensagem
    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
        /*Debug.Log(playerID + " n sou local, atualizar valores");
        Debug.Log("Pos" + _position.ToString());
        Debug.Log("Rot" + _rotation.ToString());
        Debug.Log("Time" + _timeToLerp.ToString());*/

        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if(realPosition != transform.position)
        {
            isLerpingPosition = true;
        }

        if(realRotation.eulerAngles != transform.rotation.eulerAngles)
        {
            isLerpingRotation = true;
        }

        timeStartedLerping = Time.time;
    }

    //recebe do servidor movement de uma ovelha
    private void OnReceiveSheepMovementMessage(NetworkMessage _message)
    {
        Debug.Log(playerID + " Recebi mensagem sheep movement");
        
        sheepManager.GetComponent<SheepAI>().receiveSheepMessage(_message);
    }

    private void Update()
    {
        //verificar se ja e suposto enviar mensagem e se sim enviar
        if (isLocalPlayer)
        {
            if (!canSendNetworkMovement)
            {
                canSendNetworkMovement = true;
                StartCoroutine(StartNetworkSendCooldown());
            }
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        //Debug.Log("posSend :" + this.transform.Find("Graphics").GetComponent<Transform>().position.ToString());

        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID,
            this.transform.Find("Graphics").GetComponent<Transform>().position,
            this.transform.Find("Graphics").GetComponent<Transform>().rotation,
            (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        PlayerMovementMessage _msg = new PlayerMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _playerID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(player_movement_msg, _msg);
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer)
        {
            NetworkLerp();
        }
    }

    //interpolaçao do movimento de players nao locais
    private void NetworkLerp()
    {

        if(isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            this.transform.Find("Graphics").GetComponent<Transform>().position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if(isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            this.transform.Find("Graphics").GetComponent<Transform>().rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }
}