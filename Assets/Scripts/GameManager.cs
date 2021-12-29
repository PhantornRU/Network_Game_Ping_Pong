using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
	private GameObject borderUp, borderDown;
    public Vector2 sizeWindow;

	void Start()
    {
		//устанавливаем границы сверху и снизу для отскакивания мяча
		borderUp = GameObject.Find("BorderUp");
		borderDown = GameObject.Find("BorderDown");
		BorderChange();
	}

	public void BorderChange()
	{
		//размер экрана
		sizeWindow = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

		//местоположение верхних границ
		float borderY = sizeWindow.y + borderUp.transform.localScale.y * 16;
		borderUp.transform.position = new Vector2(borderUp.transform.position.x, borderY);
		borderDown.transform.position = new Vector2(borderDown.transform.position.x, - borderY);
	}
}


