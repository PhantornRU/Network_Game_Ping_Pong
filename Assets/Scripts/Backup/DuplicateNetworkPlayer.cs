using Unity.Netcode;
using UnityEngine;

public class DuplicateNetworkPlayer : NetworkBehaviour
{
    //������������ ������� ������ � ����
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    private GameManager gameManager;

    //���������� OnNetworkSpawn, ����� ����������� ����� ��������� ������ NetworkObject � ������������� ����.
    //�������������� OnNetworkSpawn, ��������� ������ � ������ � ������ �������.

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

    //����� ����������� �������� ��������
    public void Move()
    {
        //���� ����� ����������� �������, �� �� ��������� ��� ����������
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {   //���� �� �������� ��������, �� ���������� � ServerRpc,
            //������� ����� ���� ������ �������� ��� ���������� �� �������
            SubmitPositionRequestServerRpc();
        }
    }

    //������������� ������� NetworkVariable �� ������� ����� ������, ������� ��������� ����� �� ���������
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandomPositionOnPlane();
    }

    //��������� �������
    static Vector2 GetRandomPositionOnPlane()
    {
        return new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    }

    //���� ���������� �������
    void Update()
    {
        transform.position = Position.Value;
    }
}