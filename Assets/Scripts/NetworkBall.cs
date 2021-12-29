using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkBall : NetworkBehaviour
{
    private GameManager gameManager;

    private Rigidbody2D ballRb;

    bool isActive = false;

    float speed = 300f;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        ballRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsServer)
        {
            if (!isActive && Input.GetKeyDown(KeyCode.Space))
            {
                StartBall();
            }
        }

        if (isActive)
        {
            CheckBall();
            BallSpeedBoost();
        }
    }

    private void StartBall()
    {
        Debug.Log("����");

        isActive = true;

        //������ ������� �������� ���� ���� � ���������� ���
        ballRb.WakeUp();
        transform.position = new Vector2(0, 0);

        //������� ������� ���� � ��������� �����������
        Vector2 direction = new Vector2(1, Random.Range(1.5f, -1.5f));
        if (Random.Range(0, 2) == 1) direction.x *= -1;
        ballRb.AddForce(direction * speed);
    }

    private void OnGUI()
    {
        if (!isActive) scoreText.text = scoreLeftTeam + " : " + scoreRightTeam;
    }

    private void CheckBall()
    {
        //����������� ���� ���� ��� ����� �� �����, ������������� ���
        if (isActive && Mathf.Abs(transform.position.x) > gameManager.sizeWindow.x)
        {
            if (transform.position.x > 0) scoreLeftTeam++;
            if (transform.position.x < 0) scoreRightTeam++;

            scoreText.text = scoreLeftTeam + " : " + scoreRightTeam;

            isActive = false;

            SleepBall();
        }
    }

    public TextMeshProUGUI scoreText;
    private int scoreLeftTeam, scoreRightTeam;

    private void SleepBall()
    {
        //�������� �������� ���� � ��������� ���
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0;
        ballRb.Sleep();
    }

    //����������� �������� ���� ������ �������� ����� � ��������
    private float timeLeftSpeedBoostBall = 0;
    private float timeSecondForSpeedBoostBall = 1;
    private float speedBallBoost = 0.25f;

    private void BallSpeedBoost()
    {   //���������� ����������� �������� ����
        timeLeftSpeedBoostBall -= Time.deltaTime;
        if (timeLeftSpeedBoostBall < 0)
        {
            //�������� ��������
            Debug.Log("������ ������, velocity ���� = " + ballRb.velocity);
            if (ballRb.velocity.x > 0)
                ballRb.velocity = new Vector2(ballRb.velocity.x + speedBallBoost, ballRb.velocity.y + speedBallBoost);
            else ballRb.velocity = new Vector2(ballRb.velocity.x - speedBallBoost, ballRb.velocity.y - speedBallBoost);

            //����������
            timeLeftSpeedBoostBall = timeSecondForSpeedBoostBall;
        }
    }

    //private NetworkVariable<int> scoreLeftTeam = new NetworkVariable<int>(0);
    //private NetworkVariable<int> scoreRightTeam = new NetworkVariable<int>(0);

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn(); 

    //    if (IsClient)
    //    {
    //        scoreLeftTeam.OnValueChanged -= OnScoreLeftChanged;
    //        scoreRightTeam.OnValueChanged -= OnScoreRightChanged;
    //    }
    //}
    //private void OnScoreLeftChanged(int previousAmount, int currentAmount)
    //{
    //    if (!IsOwner) return;
    //    Debug.LogFormat("Score Left {0} ", currentAmount);
    //    scoreText.text = scoreLeftTeam + " : " + scoreRightTeam;
    //    //if (NetworkManager.Singleton != null) NetworkManager.Singleton.SetScore(scoreLeftTeam.Value);
    //}
    //private void OnScoreRightChanged(int previousAmount, int currentAmount)
    //{
    //    if (!IsOwner) return;
    //    Debug.LogFormat("Score Right {0} ", currentAmount);
    //    scoreText.text = scoreLeftTeam + " : " + scoreRightTeam;
    //    //if (NetworkManager.Singleton != null) NetworkManager.Singleton.SetScore(scoreRightTeam.Value);
    //}
}
