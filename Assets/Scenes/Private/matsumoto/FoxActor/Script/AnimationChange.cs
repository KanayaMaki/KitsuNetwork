using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationChange : MonoBehaviour {

	/// <summary>
	/// キャラクターが取りうるアニメーション状態。ビット演算を行うため2の累乗に設定している
	/// </summary>
	public enum AnimeState {
		WAIT = 0,
		DAMAGE = 1,
		HASIRU = 2,
		JUMP = 4,
		OTIHAJIME = 8,
		OTITEIRUTOKI = 16,
		TYAKUTI = 32,
		TOMARU = 64,
		WALK = 128,
		TAIKI = 256,
		BURABURA = 512
	}

	//使用するテクスチャ
	public Texture damegeTex;
	public Texture hasiruTex;
	public Texture jumpTex;
	public Texture otihajimeTex;
	public Texture otiteirutokiTex;
	public Texture taikiTex;
	public Texture tyakutiTex;
	public Texture tomaruTex;
	public Texture walkTex;
	public Texture yorokobiTex;
	public Texture buraburaTex;

	Material material1;

	Animator animator;                           //キャラについているアニメーションコントローラを取得する
    AnimeState animeState;      //現在設定しているアニメーションナンバー

	//今回使用するモデルが二つのメッシュを持っている
	GameObject Child1;

	public void SetAnime(AnimeState anime) {
		if (animeState != anime) {
			animeState = anime;
			animator.SetInteger("StateNo", (int)animeState);
		}
	}

	// Use this for initialization
	void Start() {
		Child1 = transform.Find("pCube1").gameObject;
		material1 = Child1.GetComponent<Renderer>().material;
		animator = GetComponent<Animator>();
		SetAnime(AnimeState.WAIT);  //初期は待ち状態からスタート
	}

	public void InitAnimator(Animator _animator) {
		animator = _animator;
	}

	//==================アニメーションイベント=========================

	void TaikiTex() {
		Texture tex = taikiTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void WalkTex() {
		Texture tex = walkTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void HasiruTex() {
		Texture tex = hasiruTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void DamageTex() {
		Texture tex = damegeTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void JumpTex() {
		Texture tex = jumpTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);


		//ジャンプ音の再生
		FindObjectOfType<SoundManager>().PlaySE("Jump", 0.0f);
	}

	void OtihajimeTex() {
		Texture tex = otihajimeTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void OtiteirutokiTex() {
		Texture tex = otihajimeTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void TyakutiTex() {
		Texture tex = tyakutiTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

	void TomaruTex() {
		Texture tex = tomaruTex;
		material1.SetTexture("_BaseMap", tex);
		material1.SetTexture("_1st_ShadeMap", tex);
		material1.SetTexture("_2nd_ShadeMap", tex);
	}

    // アニメーション取得
    public AnimeState GetAnimeState()
    {
        return animeState;
    }
}