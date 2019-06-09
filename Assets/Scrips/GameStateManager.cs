using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PlayerManager;

public class GameStateManager : NetworkMessageHandler
{

    public bool canSendNetworkMessage;
    public float networkSendRate = 5;

    const float DEFAULT_GAME_TIME =  300.0f;

    const int TEAM_RED = 1;
    const int TEAM_BLUE = 2;
    const int TEAM_YELLOW = 3;

    public bool gameRunning = false;
    public float time = DEFAULT_GAME_TIME;

    public int teamRedScore = 0;
    public int teamBlueScore = 0;
    public int teamYellowScore = 0;

    public GameObject[] teamRedPlayers;
    public GameObject[] teamBluePlayers;
    public GameObject[] teamYellowPlayers;

    public int teamRedPlayerCount = 0;
    public int teamBluePlayerCount = 0;
    public int teamYellowPlayerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void FixedUpdate()
    {
        if (!isServer)
            return;

        if (gameRunning)
            processGame();

        if (!canSendNetworkMessage)
        {
            canSendNetworkMessage = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    //updateGame
    void processGame()
    {
        time -= Time.fixedDeltaTime;
        if(time <= 0)
        {
            TerminateGame();
            gameRunning = false;
            time = DEFAULT_GAME_TIME;
        }


        checkTeamScores();
    }

    //tell clients game has finished
    void TerminateGame()
    {

    }

    //check how many sheep are in each teams' base
    void checkTeamScores()
    {

    }

    //try add player to specified team, return true on success and false otherwise
    public bool addToTeam(int team, string playerTransformName)
    {
        if (!isServer)
            return false;

        if(team == TEAM_RED)
        {
            if (teamRedPlayerCount < 2)
            {
                removePlayer(playerTransformName);

                if (teamRedPlayers[0] == null)
                    teamRedPlayers[0] = Manager.Instance.ConnectedPlayers[playerTransformName].gameObject;
                else
                    teamRedPlayers[1] = Manager.Instance.ConnectedPlayers[playerTransformName].gameObject;

                teamRedPlayerCount++;
                return true;
            }
            else
                return false;            
        }
        else if (team == TEAM_BLUE)
        {
            if (teamBluePlayerCount < 2)
            {
                removePlayer(playerTransformName);

                if (teamBluePlayers[0] == null)
                    teamBluePlayers[0] = Manager.Instance.ConnectedPlayers[playerTransformName].gameObject;
                else
                    teamBluePlayers[1] = Manager.Instance.ConnectedPlayers[playerTransformName].gameObject;

                teamBluePlayerCount++;
                return true;
            }
            else
                return false;
        }
        else if (team == TEAM_YELLOW)
        {
            if (teamYellowPlayerCount < 2)
            {
                removePlayer(playerTransformName);

                if (teamYellowPlayers[0] == null)
                    teamYellowPlayers[0] = Manager.Instance.ConnectedPlayers[playerTransformName].gameObject;
                else
                    teamYellowPlayers[1] = Manager.Instance.ConnectedPlayers[playerTransformName].gameObject;

                teamYellowPlayerCount++;
                return true;
            }
            else
                return false;
        }

        return false;
    }

    //attemp to add player to one of the teams
    public void addPlayer(string playerTransformName)
    {
        if (!isServer)
            return;

        if (addToTeam(TEAM_RED, playerTransformName))
            return;

        if (addToTeam(TEAM_BLUE, playerTransformName))
            return;

        if (addToTeam(TEAM_YELLOW, playerTransformName))
            return;

        kickPlayer(playerTransformName);
    }

    //remove player from its current team
    public void removePlayer(string transformName)
    {
        //check player 0
        if (teamRedPlayers[0] != null && teamRedPlayers[0].transform.name == transformName)
        {
            teamRedPlayers[0] = null;
            teamRedPlayerCount--;
            return;
        }

        //check player 1
        if (teamRedPlayers[1] != null && teamRedPlayers[1].transform.name == transformName)
        {
            teamRedPlayers[1] = null;
            teamRedPlayerCount--;
            return;
        }

        //check player 2
        if (teamBluePlayers[0] != null && teamBluePlayers[0].transform.name == transformName)
        {
            teamBluePlayers[0] = null;
            teamBluePlayerCount--;
            return;
        }

        //check player 3
        if (teamBluePlayers[1] != null && teamBluePlayers[1].transform.name == transformName)
        {
            teamBluePlayers[1] = null;
            teamBluePlayerCount--;
            return;
        }

        //check player 4
        if (teamYellowPlayers[0] != null && teamYellowPlayers[0].transform.name == transformName)
        {
            teamYellowPlayers[0] = null;
            teamYellowPlayerCount--;
            return;
        }

        //check player 5
        if (teamYellowPlayers[1] != null && teamYellowPlayers[1].transform.name == transformName)
        {
            teamYellowPlayers[1] = null;
            teamYellowPlayerCount--;
            return;
        }

    }

    //send a message that to signal player that they cant enter the match
    public void kickPlayer(string playerTransformName)
    {

    }

    public void updateMatch(MatchInfoMessage msg)
    {

    }

    public void updateLobby(LobbyInfoMessage msg)
    {
        teamRedPlayerCount = 0;
        teamBluePlayerCount = 0;
        teamYellowPlayerCount = 0;

        if (msg.hasPlayer0)
        {
            teamRedPlayers[0] = Manager.Instance.ConnectedPlayers[msg.player0transformName].gameObject;
            teamRedPlayerCount++;
        }
        else
            teamRedPlayers[0] = null;

        if (msg.hasPlayer1)
        {
            teamRedPlayers[1] = Manager.Instance.ConnectedPlayers[msg.player1transformName].gameObject;
            teamRedPlayerCount++;
        }
        else
            teamRedPlayers[1] = null;

        if (msg.hasPlayer2)
        {
            teamBluePlayers[0] = Manager.Instance.ConnectedPlayers[msg.player2transformName].gameObject;
            teamBluePlayerCount++;
        }
        else
            teamBluePlayers[0] = null;

        if (msg.hasPlayer3)
        {
            teamBluePlayers[1] = Manager.Instance.ConnectedPlayers[msg.player3transformName].gameObject;
            teamBluePlayerCount++;
        }
        else
            teamBluePlayers[1] = null;

        if (msg.hasPlayer4)
        {
            teamYellowPlayers[0] = Manager.Instance.ConnectedPlayers[msg.player4transformName].gameObject;
            teamYellowPlayerCount++;
        }
        else
            teamYellowPlayers[0] = null;

        if (msg.hasPlayer5)
        {
            teamYellowPlayers[1] = Manager.Instance.ConnectedPlayers[msg.player5transformName].gameObject;
            teamYellowPlayerCount++;
        }
        else
            teamYellowPlayers[1] = null;

    }

    private IEnumerator StartNetworkSendCooldown()
    {
        yield return new WaitForSeconds((1 / networkSendRate));
        if (gameRunning)
            SendMatchInfoMessage();
        else
            SendLobbyInfo();
    }

    private void SendLobbyInfo()
    {

        bool _hasPlayer0 = false;
        string _player0transformName = "";
        string _player0Nickname = "";

        bool _hasPlayer1 = false;
        string _player1transformName = "";
        string _player1Nickname = "";

        bool _hasPlayer2 = false;
        string _player2transformName = "";
        string _player2Nickname = "";

        bool _hasPlayer3 = false;
        string _player3transformName = "";
        string _player3Nickname = "";

        bool _hasPlayer4 = false;
        string _player4transformName = "";
        string _player4Nickname = "";

        bool _hasPlayer5 = false;
        string _player5transformName = "";
        string _player5Nickname = "";

        //player 0
        if(teamRedPlayers[0] != null)
        {
            _hasPlayer0 = true;
            _player0transformName = teamRedPlayers[0].transform.name;
            _player0Nickname = teamRedPlayers[0].GetComponent<NetworkPlayer>().nickName;
        }

        //player 1
        if (teamRedPlayers[1] != null)
        {
            _hasPlayer1 = true;
            _player1transformName = teamRedPlayers[1].transform.name;
            _player1Nickname = teamRedPlayers[1].GetComponent<NetworkPlayer>().nickName;
        }

        //player 2
        if (teamBluePlayers[0] != null)
        {
            _hasPlayer2 = true;
            _player2transformName = teamBluePlayers[0].transform.name;
            _player2Nickname = teamBluePlayers[0].GetComponent<NetworkPlayer>().nickName;
        }

        //player 3
        if (teamBluePlayers[1] != null)
        {
            _hasPlayer3 = true;
            _player3transformName = teamBluePlayers[1].transform.name;
            _player3Nickname = teamBluePlayers[1].GetComponent<NetworkPlayer>().nickName;
        }

        //player 4
        if (teamYellowPlayers[0] != null)
        {
            _hasPlayer4 = true;
            _player4transformName = teamYellowPlayers[0].transform.name;
            _player4Nickname = teamYellowPlayers[0].GetComponent<NetworkPlayer>().nickName;
        }

        //player 5
        if (teamYellowPlayers[1] != null)
        {
            _hasPlayer5 = true;
            _player5transformName = teamYellowPlayers[1].transform.name;
            _player5Nickname = teamYellowPlayers[1].GetComponent<NetworkPlayer>().nickName;
        }

        SendLobbyInfoMessage(_hasPlayer0, _player0transformName, _player0Nickname,
        _hasPlayer1, _player1transformName, _player1Nickname,
        _hasPlayer2, _player2transformName, _player2Nickname,
        _hasPlayer3, _player3transformName, _player3Nickname,
        _hasPlayer4, _player4transformName, _player4Nickname,
        _hasPlayer5, _player5transformName, _player5Nickname);

    }

    public void SendLobbyInfoMessage(bool _hasPlayer0, string _player0transformName, string _player0Nickname,
        bool _hasPlayer1, string _player1transformName, string _player1Nickname,
        bool _hasPlayer2, string _player2transformName, string _player2Nickname,
        bool _hasPlayer3, string _player3transformName, string _player3Nickname,
        bool _hasPlayer4, string _player4transformName, string _player4Nickname,
        bool _hasPlayer5, string _player5transformName, string _player5Nickname)
    {
        LobbyInfoMessage _msg = new LobbyInfoMessage()
        {
            hasPlayer0 = _hasPlayer0,
            player0transformName = _player0transformName,
            player0Nickname = _player0Nickname,

            hasPlayer1 = _hasPlayer1,
            player1transformName = _player1transformName,
            player1Nickname = _player1Nickname,

            hasPlayer2 = _hasPlayer2,
            player2transformName = _player2transformName,
            player2Nickname = _player2Nickname,

            hasPlayer3 = _hasPlayer3,
            player3transformName = _player3transformName,
            player3Nickname = _player3Nickname,

            hasPlayer4 = _hasPlayer4,
            player4transformName = _player4transformName,
            player4Nickname = _player4Nickname,

            hasPlayer5 = _hasPlayer5,
            player5transformName = _player5transformName,
            player5Nickname = _player5Nickname,
        };

        NetworkManager.singleton.client.Send(lobby_info_msg, _msg);

        canSendNetworkMessage = false;
    }

    public void SendMatchInfoMessage()
    {
        MatchInfoMessage _msg = new MatchInfoMessage()
        {
            scoreTeamRed = teamRedScore,
            scoreTeamBlue = teamBlueScore,
            scoreTeamYellow = teamYellowScore,
            time = this.time
        };

        NetworkManager.singleton.client.Send(match_info_msg, _msg);

        canSendNetworkMessage = false;
    }

}
