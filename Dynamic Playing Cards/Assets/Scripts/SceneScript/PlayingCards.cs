using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCards : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	private TextureManager textureManager;
	// Use this for initialization
	void Start () {
		textureManager=new TextureManager(256);
		textureManager.CreateTexture("hearts");
		textureManager.CreateTexture("diamond");
		textureManager.CreateTexture("clubs");
		textureManager.CreateTexture("spades");
		textureManager.PackTextures();
		Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("spades"),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
