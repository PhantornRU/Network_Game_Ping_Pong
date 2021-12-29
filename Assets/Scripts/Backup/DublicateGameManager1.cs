using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class DublicateGameManager1 : MonoBehaviour
{
    public GameObject player1, player2, ball;
	private Rigidbody2D player1Rb, player2Rb, ballRb;
	public TextMeshProUGUI playerScoreText;
	private GameObject borderUp, borderDown;

	public int startBallSpeed = 350;
    public float playerSpeed = 5;

    public int scorePlayer1;
    public int scorePlayer2;

    public Vector2 sizeWindow;

	[SerializeField] private bool checkGhoal = false;
	[SerializeField] private bool isGameActive = false;

	public NetworkScriptManager networkManager;

	//public bool isServer = true;

	// Start is called before the first frame update
	void Start()
    {
		//������ ������
        sizeWindow = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

		//���������� ������� ��� ��������
		player1Rb = player1.GetComponent<Rigidbody2D>();
		player2Rb = player2.GetComponent<Rigidbody2D>();
		ballRb = ball.GetComponent<Rigidbody2D>();

		//������������� ������� ������ � ����� ��� ������������ ����
		borderUp = GameObject.Find("BorderUp");
		borderDown = GameObject.Find("BorderDown");
		BorderChange();

		//������������� ��������� ������� ������������ ������
		player1.transform.position = new Vector2(-(sizeWindow.x - 1), 0);
		player2.transform.position = new Vector2(sizeWindow.x - 1, 0);
	}

	private float verticalInputWASD, verticalInputArrow;

	// Update is called once per frame
	void Update()
    {
		//�������� �������
		//PlayerMove();

		//���, �������� ���� � ��������� ���, ���� ���� ���������
		if (!isGameActive && Input.GetMouseButtonDown(0)) 
		{
			GameStart();
		}

		if (isGameActive)
        {
			CheckBall();
			BallSpeedBoost();
		}
	}

	private void GameStart()
	{
		isGameActive = true;
		checkGhoal = false;

		//������ ������� �������� ���� ���� � ���������� ���
		ballRb.WakeUp();
		ball.transform.position = new Vector2(0, 0);

		//������� ������� ���� � ��������� �����������
		Vector2 direction = new Vector2(1, Random.Range(1.5f, -1.5f));
		if (Random.Range(0, 2) == 1) direction.x *= -1;
		ballRb.AddForce(direction * startBallSpeed);
	}

	private void PlayerMove()
    {
		verticalInputWASD = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
		verticalInputArrow = Input.GetAxis("VerticalArrow") * Time.deltaTime * playerSpeed;

		if (Mathf.Abs(player1.transform.position.y + verticalInputWASD * playerSpeed) + player1.transform.localScale.y / 2 < sizeWindow.y )
		{
			player1.transform.Translate(0, verticalInputWASD * playerSpeed, 0);
		}

		if (Mathf.Abs(player2.transform.position.y + verticalInputArrow * playerSpeed) + player2.transform.localScale.y / 2 < sizeWindow.y )
		{
			player2.transform.Translate(0, verticalInputArrow * playerSpeed, 0);
		}

		//�������� �������� �������������. ������������ ��� �������� ������� ������� ������ � ������������� ������������, ����� �� �� ���������.
		//float Y = Mathf.Lerp(player2.transform.position.y, ball.transform.position.y, playerSpeed * Time.deltaTime);
		//player2.transform.position = new Vector2(player2.transform.position.x, Y);
	}

	public void BorderChange()
    {
		float borderY = sizeWindow.y + borderUp.transform.localScale.y * 16;
		borderUp.transform.position = new Vector2(borderUp.transform.position.x, borderY);
		borderDown.transform.position = new Vector2(borderDown.transform.position.x, - borderY);
	}

	private void CheckBall()
	{
		//����������� ���� ���� ��� ����� �� �����, ������������� ���
		if (!checkGhoal && Mathf.Abs(ball.transform.position.x) > sizeWindow.x)
		{
			if (ball.transform.position.x > 0) scorePlayer1++;
			if (ball.transform.position.x < 0) scorePlayer2++;

			playerScoreText.text = scorePlayer1 + " : " + scorePlayer2;

			checkGhoal = true;
			isGameActive = false;

			//�������� �������� ���� � ��������� ���
			ballRb.velocity = new Vector2(0, 0);
			ballRb.angularVelocity = 0;
			ballRb.Sleep();
		}
	}

	//����������� �������� ���� ������ �������� ����� � ��������
	private float timeLeftSpeedBoostBall = 0;
	private float timeSecondForSpeedBoostBall = 1;
	private float speedBallBoost = 0.25f;

	private void BallSpeedBoost()
    {
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
}


