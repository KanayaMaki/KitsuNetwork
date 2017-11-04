using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeybord : MonoBehaviour
{
	public delegate void GetString(string _name);

	public GetString Str;


	[SerializeField]
	private Vector3 m_width;
	private Vector3 m_initPos;

	[SerializeField]
	private GameObject m_cursorImage;
	private Image m_allKey;

	private Text m_inputSpaceText;

	private char[][] m_keybord;

	private const int LINE = 5;
	private const int RAW = 17;

	private int m_cursorRaw = 0;
	private int m_cursorLine = 0;

	private SoundManager soundMng;

	// Use this for initialization
	void Start()
	{
		soundMng = GameObject.Find("SoundManagerRoom").GetComponent<SoundManager>();

		m_cursorRaw = 0;
		m_cursorLine = 0;

		m_initPos = m_cursorImage.GetComponent<RectTransform>().localPosition;

		var inputSpace = gameObject.transform.GetChild(0);
		m_inputSpaceText = inputSpace.GetChild(0).GetComponent<Text>();

		m_keybord = new char[RAW][];
		for (int raw = 0; raw < RAW; raw++)
		{
			m_keybord[raw] = new char[LINE];
			for (int line = 0; line < LINE; line++)
			{
				m_keybord[raw][line] = SetKey(raw, line);
			}
		}


	}

	char SetKey(int _raw, int _line)
	{
		string work = "";
		switch (_raw)
		{
			case 0:
				work = "あいうえお";
				break;
			case 1:
				work = "かきくけこ";
				break;
			case 2:
				work = "さしすせそ";
				break;
			case 3:
				work = "たちつてと";
				break;
			case 4:
				work = "なにぬねの";
				break;
			case 5:
				work = "はひふへほ";
				break;
			case 6:
				work = "まみむめも";
				break;
			case 7:
				work = "や　ゆ　よ";
				break;
			case 8:
				work = "らりるれろ";
				break;
			case 9:
				work = "わ　を　ん";
				break;
			case 10:
				work = "がぎぐげご";
				break;
			case 11:
				work = "ざじずぜぞ";
				break;
			case 12:
				work = "だぢづでど";
				break;
			case 13:
				work = "ばびぶべぼ";
				break;
			case 14:
				work = "ぱぴぷぺぽ";
				break;
			case 15:
				work = "ぁぃぅぇぉ";
				break;
			case 16:
				work = "ゃゅょっー";
				break;
		}

		return work[_line];
	}

	// Update is called once per frame
	void Update()
	{

		CursorMove();

		CursorPush();

		RemoveString();

		InputEnd();
	}



	void CursorMove()
	{
		int oldRaw = m_cursorRaw;
		int oldLine = m_cursorLine;

		CursolMoveData();

		CursorMoveImage(oldRaw, oldLine);
	}

	void CursolMoveData()
	{
		// Add:弓達　キー修正
		if (InputController.Instance.GetButtonTrigger(InputController.KeyData.RIGHT))
		{
			m_cursorRaw++;
			m_cursorRaw = m_cursorRaw % RAW;
			soundMng.PlaySE("NameCursorSE");
		}
		else if (InputController.Instance.GetButtonTrigger(InputController.KeyData.LEFT))
		{
			m_cursorRaw--;
			if (m_cursorRaw < 0)
				m_cursorRaw += RAW;
			soundMng.PlaySE("NameCursorSE");
		}
		else if (InputController.Instance.GetButtonTrigger(InputController.KeyData.UP))
		{
			m_cursorLine--;
			if (m_cursorLine < 0)
				m_cursorLine += LINE;
			soundMng.PlaySE("NameCursorSE");

		}
		else if (InputController.Instance.GetButtonTrigger(InputController.KeyData.DOWN))
		{
			m_cursorLine++;
			m_cursorLine = m_cursorLine % LINE;
			soundMng.PlaySE("NameCursorSE");
		}


	}

	void CursorMoveImage(int _oldRaw, int _oldLine)
	{
		Vector2 positionData = new Vector2(m_cursorRaw, m_cursorLine);
		Vector3 nextPosition = new Vector3(positionData.x * -m_width.x, positionData.y * -m_width.y);

		m_cursorImage.GetComponent<RectTransform>().localPosition = m_initPos + nextPosition;
	}

	// カーソルの文字入力
	void CursorPush()
	{
		if (m_inputSpaceText.text.Length >= 6)
			return;
		if (m_keybord[m_cursorRaw][m_cursorLine] == '　')
			return;

		if (InputController.Instance.GetButtonTrigger(InputController.KeyData.B))
		{
			m_inputSpaceText.text += m_keybord[m_cursorRaw][m_cursorLine];
			soundMng.PlaySE("NameInputSE");
		}
	}

	// 一文字消す
	void RemoveString()
	{
		var strlength = m_inputSpaceText.text.Length;
		if (strlength == 0)
			return;

		if (InputController.Instance.GetButtonTrigger(InputController.KeyData.A))
		{
			m_inputSpaceText.text = m_inputSpaceText.text.Remove(strlength - 1);
			soundMng.PlaySE("NameRemoveSE");
		}
	}


	// 入力終了（仮想キーボード呼び出し元に,入力文字列を渡す）
	void InputEnd()
	{
		if (!InputController.Instance.GetButtonTrigger(InputController.KeyData.X))
			return;

		soundMng.PlaySE("NameDecisionSE");
		Str(m_inputSpaceText.text);
		gameObject.SetActive(false);
	}

}
