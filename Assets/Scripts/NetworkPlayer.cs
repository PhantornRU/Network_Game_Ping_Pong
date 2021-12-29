using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    // server movement
    private NetworkVariable<float> m_Movement = new NetworkVariable<float>();

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(new FixedString32Bytes(""));

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        //располагаем наших клиентов по позициям через одного на разных сторонах
        if (IsClient && OwnerClientId % 2 != 0)
        {
            transform.position = Vector2.right * ( - (gameManager.sizeWindow.x - OwnerClientId / 2 - 1 * transform.localScale.x));
        }
        else
        {
            transform.position = Vector2.right * (gameManager.sizeWindow.x - OwnerClientId / 2 - 1 * transform.localScale.x);
        }
    }

    //void LateUpdate()
    //{
    //    if (IsLocalPlayer)
    //    {   //камера
    //        // center camera.. only if this is MY player!
    //        Vector3 pos = transform.position;
    //        pos.z = -50;
    //        Camera.main.transform.position = pos;
    //    }
    //}

    //перезапись OnNetworkSpawn, метод запускается когда создается обхект NetworkObject и настраивается сеть.
    //Переопределяем OnNetworkSpawn, поскольку клиент и сервер с разной логикой.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerName.Value = $"Player {OwnerClientId}";
        }
    }

    //цикл обновления
    void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }

        if (IsClient)
        {
            UpdateClient();
        }
    }
    //скорость
    float speed = 5.0f;

    void UpdateServer()
    {
        // update thrust
        if (m_Movement.Value != 0)
        {
            //float speed2 = speed;
            Vector3 thrustVec = transform.up * (m_Movement.Value * speed * Time.deltaTime);
            bool checkBorderY = Mathf.Abs(transform.position.y + thrustVec.y) + transform.localScale.y / 2 < gameManager.sizeWindow.y;
            if (checkBorderY)
            {   //перемещаем
                transform.Translate(thrustVec); 
            }
        }
    }

    void UpdateClient()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        int moveForce = 0;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0)
        {
            moveForce += 1;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Vertical") < 0)
        {
            moveForce -= 1;
        }

        if (m_OldMoveForce != moveForce)
        {
            MoveServerRpc(moveForce);
            m_OldMoveForce = moveForce;
        }
    }

    // --- ServerRPCs ---

    //переменные для дросселирования тактов клиентского движения с целью синхронизации
    float m_OldMoveForce = 0;

    [ServerRpc]
    public void MoveServerRpc(float movement)
    {
        m_Movement.Value = movement;
        //m_Movement.Value = thrusting;
    }

    [ServerRpc]
    public void SetNameServerRpc(string name)
    {
        PlayerName.Value = name;
    }

    void OnGUI()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        // draw the name with a shadow (colored for buf)	
        GUI.color = Color.black;
        GUI.Label(new Rect(pos.x - 20, Screen.height - pos.y - 30, 400, 30), PlayerName.Value.Value);
    }
}