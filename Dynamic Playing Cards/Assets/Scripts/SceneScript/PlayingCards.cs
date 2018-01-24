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
		textureManager.CreateRoundedRectangle("base");
		textureManager.CreateRoundedRectangle("back");
		textureManager.PackTextures();
		SpriteMeshType spriteMeshType= SpriteMeshType.FullRect;
		//Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("back"),new Vector2(0.5f,0.5f),1,0,spriteMeshType);
		//Sprite sprite= Sprite.Create(textureManager.TilingTexture,new Rect(0,0,textureManager.TilingTexture.width,textureManager.TilingTexture.height),new Vector2(0.5f,0.5f),1,0,spriteMeshType);
		Sprite sprite= Sprite.Create(textureManager.PackedTexture,new Rect(0,0,textureManager.PackedTexture.width,textureManager.PackedTexture.height),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
