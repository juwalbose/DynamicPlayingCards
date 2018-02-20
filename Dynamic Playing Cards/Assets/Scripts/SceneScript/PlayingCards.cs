using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingCards : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	public RectTransform overlayRectTransform;
	public Text topText;
	public Text bottomText;
	public Image topSignImage;
	public Image bottomSignImage;
	public Image baseImage;
	
	[System.Serializable]
	public struct Layout{
		public string cardValue;
		public int[] layer_5;
		public int[] layer_4;
		public int[] layer_3;
		public int[] layer_2;
		public int[] layer_1;
	}
	struct Card {
		public Sprite sign;
		public Layout cardLayout;
	}
	public Layout[] layouts;
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
		Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("back"),new Vector2(0.5f,0.5f),1,0,spriteMeshType);
		//Sprite sprite= Sprite.Create(textureManager.TilingTexture,new Rect(0,0,textureManager.TilingTexture.width,textureManager.TilingTexture.height),new Vector2(0.5f,0.5f),1,0,spriteMeshType);
		//Sprite sprite= Sprite.Create(textureManager.PackedTexture,new Rect(0,0,textureManager.PackedTexture.width,textureManager.PackedTexture.height),new Vector2(0.5f,0.5f),1);
		//Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("clubs"),new Vector2(0.5f,0.5f),1);
		
		spriteRenderer.sprite=sprite;
		spriteRenderer.size=new Vector2(183,256);

		CreateCard(Random.Range(1,53));
		//CreateCard(10);
	}

    private void CreateCard(int cardId)
    {
        int cardType=1+((int)(cardId/13.0f));
		string cardTypeString="";
		switch(cardType){
			case 1:
			cardTypeString="clubs";
			break;
			case 2:
			cardTypeString="diamond";
			break;
			case 3:
			cardTypeString="hearts";
			break;
			case 4:
			cardTypeString="spades";
			break;
		}
		Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName(cardTypeString),new Vector2(0.5f,0.5f),1);
		Card card=new Card();
		card.sign=sprite;
		int cardValue=cardId%13;
		if(cardValue==0)cardValue=13;
		card.cardLayout=layouts[cardValue-1];
		Debug.Log("created id " +cardId+" is "+card.cardLayout.cardValue+" of "+cardTypeString);
		
		DisplayCard(card);
    }

    private void DisplayCard(Card card)
    {
        Layout cardLayout=card.cardLayout;
		topText.text=bottomText.text=cardLayout.cardValue;
		topSignImage.sprite=card.sign;
		bottomSignImage.sprite=card.sign;
		baseImage.sprite=Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("base"),new Vector2(0.5f,0.5f),1);
		Vector2 maxOffsets=new Vector2();
		maxOffsets.x=Mathf.Abs(overlayRectTransform.sizeDelta.x*0.5f);
		maxOffsets.y=Mathf.Abs(overlayRectTransform.sizeDelta.y*0.5f);
		Vector2 pos=new Vector2(-1*maxOffsets.x,maxOffsets.y);
		float xPadding=maxOffsets.x;
		float specialYOffset=maxOffsets.y*0.66f;
		if(cardLayout.cardValue.Equals("7"))specialYOffset=maxOffsets.y*0.5f;
		AddSigns(cardLayout.layer_5,1.3f,pos,xPadding,1.0f);
		pos.y=maxOffsets.y*0.33f;
		AddSigns(cardLayout.layer_4,1.3f,pos,xPadding,1.0f,specialYOffset);
		pos.y=0;
		AddSigns(cardLayout.layer_3,1.3f,pos,xPadding,0.5f);
		pos.y=maxOffsets.y*-0.33f;
		AddSigns(cardLayout.layer_2,1.3f,pos,xPadding,0.0f,specialYOffset*-1);
		pos.y=-1*maxOffsets.y;
		AddSigns(cardLayout.layer_1,1.3f,pos,xPadding,0.0f);
	}


    private void AddSigns(int[] arr, float scale, Vector2 pos,float xPadding, float anchorY,float specialYOffset=0)
    {
		Image displaySign;
		Vector2 anchor=new Vector2(0,anchorY);
		Vector2 specialPos=new Vector2();
		
		for(int i=0;i<arr.Length;i++){
			if(Mathf.Abs(arr[i])==1){
				displaySign=Instantiate(topSignImage,Vector2.zero,Quaternion.identity);
				displaySign.transform.SetParent(overlayRectTransform,false);
				displaySign.rectTransform.anchorMax=displaySign.rectTransform.anchorMin=anchor;
				displaySign.rectTransform.sizeDelta=topSignImage.rectTransform.sizeDelta*scale;
				displaySign.rectTransform.localPosition=pos;
				if(i==1&&Mathf.Abs(specialYOffset)>0){
					specialPos.x=pos.x;
					specialPos.y=specialYOffset;
					displaySign.rectTransform.localPosition=specialPos;
				}
				if(arr[i]==-1){
					displaySign.rectTransform.localScale=Vector2.one*-1;
				}
				
			}
			anchor.x+=0.5f;
			pos.x+=xPadding;
		}
    }
}
