using Unity.Netcode;
using UnityEngine;

public class DuplicateNetworkPlayer : NetworkBehaviour
{
    //представляем позицию игрока в сети
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    private GameManager gameManager;

    //перезапись OnNetworkSpawn, метод запускается когда создается обхект NetworkObject и настраивается сеть.
    //Переопределяем OnNetworkSpawn, поскольку клиент и сервер с разной логикой.

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    //метод выполняющий действие движения
    public void Move()
    {
        //если игрок принадлежит серверу, то мы мгновенно его перемещаем
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {   //если мы являемся клиентом, мы обращаемся в ServerRpc,
            //который может быть вызван клиентом для выполнения на сервере
            SubmitPositionRequestServerRpc();
        }
    }

    //устанавливаем позицию NetworkVariable на сервере этого игрока, выбирая случайную точку на плоскости
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandomPositionOnPlane();
    }

    //случайная позиция
    static Vector2 GetRandomPositionOnPlane()
    {
        return new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    }

    //цикл обновления позиции
    void Update()
    {
        transform.position = Position.Value;
    }
}