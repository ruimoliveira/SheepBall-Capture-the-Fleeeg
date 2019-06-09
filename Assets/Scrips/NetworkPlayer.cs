using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Cameras;
using SheepAnimationState;

public class NetworkPlayer : NetworkMessageHandler
{
    [Header("Player Properties")]
    public string playerID;
    public string nickName;
    public int team;

    [Header("Player Movement Properties")]
    public bool canSendNetworkMovement;
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

    public bool sendMovementMessages = false;

    private GameObject sheepManager;

    public GameObject gameStateManagerObj;

    public GameObject networkServerRelayPrefab;
    public GameObject gameStateManagerPrefab;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (isLocalPlayer)
            RegisterNetworkMessages();


        if(isServer)
        {
            if(GameObject.FindGameObjectWithTag("NetworkServerRelay") == null)
            {
                GameObject go = Instantiate(networkServerRelayPrefab);
                NetworkServer.Spawn(go);
            }

            if (GameObject.FindGameObjectWithTag("GameStateManager") == null)
            {
                GameObject go1 = Instantiate(gameStateManagerPrefab);
                NetworkServer.Spawn(go1);
            }

        }

        playerID = "player" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        nickName = playerID;

        Manager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);

        // Ignore collision between base and players
        GameObject[] baseWalls = GameObject.FindGameObjectsWithTag(Constants.BASE_WALL_TAG);
        Collider playerCollider = transform.Find("Graphics").GetComponent<CapsuleCollider>();
        foreach (GameObject baseWall in baseWalls)
        {
            Collider c = GetComponentInChildren<Collider>();
            Physics.IgnoreCollision(baseWall.GetComponentInChildren<Collider>(), playerCollider);
        }

        if (isLocalPlayer)
        {
            Manager.Instance.SetLocalPlayerID(playerID);

            sheepManager = GameObject.FindGameObjectWithTag("SheepManager");
            canSendNetworkMovement = false;
        }
        else
        {
            disableAllScripts();

            //inicializar variaveis lerp
            isLerpingPosition = false;
            isLerpingRotation = false;

            realPosition = this.transform.Find("Graphics").GetComponent<Transform>().position;
            realRotation = this.transform.Find("Graphics").GetComponent<Transform>().rotation;
        }
    }

    //associar o receber de uma dada mensagem a um handler
    private void RegisterNetworkMessages()
    {
        //player movement
        NetworkManager.singleton.client.RegisterHandler(player_movement_msg, OnReceivePlayerMovementMessage);

        //sheep movement
        NetworkManager.singleton.client.RegisterHandler(sheep_movement_msg, OnReceiveSheepMovementMessage);

        //picked up sheep
        NetworkManager.singleton.client.RegisterHandler(picked_up_sheep_message, OnReceivePickedUpSheepMessage);

        //dropped sheep
        NetworkManager.singleton.client.RegisterHandler(dropped_sheep_message, OnReceiveDroppedSheepMessage);

        //shoot sheep
        NetworkManager.singleton.client.RegisterHandler(shoot_sheep_message, OnReceiveShootSheepMessage);

        //land sheep
        NetworkManager.singleton.client.RegisterHandler(land_sheep_message, OnReceiveLandSheepMessage);

		//join team
        NetworkManager.singleton.client.RegisterHandler(join_team_msg, OnReceiveJoinTeamMessage);

        //lobby info
        NetworkManager.singleton.client.RegisterHandler(lobby_info_msg, OnReceiveLobbyInfoMessage);

        //match info
        NetworkManager.singleton.client.RegisterHandler(match_info_msg, OnReceiveMatchInfoMessage);
    }

    // desativar todos os scripts dos jogadores nao locais
    private void disableAllScripts()
    {
        GetComponentInChildren<ThirdPersonUserControl>().enabled = false;
        GetComponentInChildren<ThirdPersonCharacter>().enabled = false;
        GetComponentInChildren<PickupSheep>().enabled = false;
        GetComponentInChildren<ShootSheep>().enabled = false;
        GetComponentInChildren<FreeLookCam>().enabled = false;
        GetComponentInChildren<ProtectCameraFromWallClip>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
    }

    //recebe do servidor pedido de um jogador de se juntar a uma equipa
    private void OnReceiveJoinTeamMessage(NetworkMessage _message)
    {
        JoinTeamMessage _msg = _message.ReadMessage<JoinTeamMessage>();

        gameStateManagerObj.GetComponent<GameStateManager>().addToTeam(_msg.team, _msg.objectTransformName);
    }

    //recebe do servidor info dos outros players no lobby
    private void OnReceiveLobbyInfoMessage(NetworkMessage _message)
    {
        LobbyInfoMessage _msg = _message.ReadMessage<LobbyInfoMessage>();

        gameStateManagerObj.GetComponent<GameStateManager>().updateLobby(_msg);
    }

    //recebe do servidor info dos estado do jogo
    private void OnReceiveMatchInfoMessage(NetworkMessage _message)
    {
        MatchInfoMessage _msg = _message.ReadMessage<MatchInfoMessage>();

        gameStateManagerObj.GetComponent<GameStateManager>().updateMatch(_msg);
    }

    //recebe do servidor movement dos outros players
    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();

        //verifica se a mensagem NAO e sobre o proprio jogador
        if (_msg.objectTransformName != transform.name)
        {
            //aceder ao player unit de quem enviou a mensagem e atualizar os valores desse jogador
            Manager.Instance.ConnectedPlayers[_msg.objectTransformName].GetComponent<NetworkPlayer>().ProcessPlayerMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }

    //atualiza nome e equipa do jogador com info de uma mensagem
    public void ReceivePlayerInfoMessage(string nickname, int team)
    {
        this.nickName = nickname;
        this.team = team;
    }

    //atualizar variaveis de lerping vindas de uma mensagem
    public void ProcessPlayerMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if (realPosition != transform.position)
        {
            isLerpingPosition = true;
        }

        if (realRotation.eulerAngles != transform.rotation.eulerAngles)
        {
            isLerpingRotation = true;
        }

        timeStartedLerping = Time.time;
    }

    //recebe do servidor movement de uma ovelha
    private void OnReceiveSheepMovementMessage(NetworkMessage _message)
    {
        SheepMovementMessage _msg = _message.ReadMessage<SheepMovementMessage>();

        sheepManager.GetComponent<SheepAI>().processSheepMovementMessage(_msg);
    }

    //recebe do servidor uma pickedupsheep message
    private void OnReceivePickedUpSheepMessage(NetworkMessage _message)
    {
        PickedUpSheepMessage _msg = _message.ReadMessage<PickedUpSheepMessage>();

        if (_msg.playerName != transform.name)
        {
            sheepManager.GetComponent<SheepAI>().processPickedUpSheepMessage(_msg);
        }
    }

    //recebe do servidor uma droppedsheep message
    private void OnReceiveDroppedSheepMessage(NetworkMessage _message)
    {
        DroppedSheepMessage _msg = _message.ReadMessage<DroppedSheepMessage>();

        if (_msg.playerName != transform.name)
        {
            sheepManager.GetComponent<SheepAI>().processDroppedSheepMessage(_msg);
        }
    }

    //recebe do servidor uma shootsheep message
    private void OnReceiveShootSheepMessage(NetworkMessage _message)
    {
        ShootSheepMessage _msg = _message.ReadMessage<ShootSheepMessage>();

        if (_msg.playerName != transform.name)
        {
            sheepManager.GetComponent<SheepAI>().processShootSheepMessage(_msg);
        }
    }

    //recebe do servidor uma shootsheep message
    private void OnReceiveLandSheepMessage(NetworkMessage _message)
    {
        LandSheepMessage _msg = _message.ReadMessage<LandSheepMessage>();

        sheepManager.GetComponent<SheepAI>().processLandSheepMessage(_msg);
    }

    private void Update()
    {

        if(gameStateManagerObj == null)
            gameStateManagerObj = GameObject.FindGameObjectWithTag("GameStateManager");

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
        if(sendMovementMessages)
            SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendPlayerMovementMessage(playerID,
            this.transform.Find("Graphics").GetComponent<Transform>().position,
            this.transform.Find("Graphics").GetComponent<Transform>().rotation,
            (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendPlayerMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
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
        if (!isLocalPlayer)
        {
            NetworkLerp();
        }
    }

    //interpolaçao do movimento de players nao locais
    private void NetworkLerp()
    {

        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            this.transform.Find("Graphics").GetComponent<Transform>().position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            this.transform.Find("Graphics").GetComponent<Transform>().rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }

    public void setGraphicsActive(bool on)
    {
        GameObject go = this.gameObject.transform.GetChild(0).gameObject;
        go.active = on;
    }
}
