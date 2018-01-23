using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager {
	Rect[] packedAssets;
	Texture2D packedTexture;
	Texture2D[] allGraphics;
	int textureIndex;
	Dictionary<string,int> textureDict;
	int resolution;

    public Texture2D PackedTexture
    {
        get
        {
            return packedTexture;
        }
    }

	/*
	Manages textures which are all dynamically created at runtime, packed and referenced via dictionary
	 */
    public TextureManager(int _resolution){
		resolution=_resolution;
		allGraphics=new Texture2D[5];
		textureIndex=0;
		textureDict=new Dictionary<string, int>();
	}
	public Rect GetTextureRectByName(string textureName){
		textureName=textureName.ToLower();
		int textureIndex;
		Rect textureRect=new Rect(0,0,0,0);
		if(textureDict.TryGetValue(textureName, out textureIndex)){
			textureRect=ConvertUVToTextureCoordinates(packedAssets[textureIndex]);
		}else{
			Debug.Log("no such texture "+textureName);
		}
		return textureRect;
	}
	public void CreateTexture(string shape){
		textureDict.Add(shape.ToLower(),textureIndex);
		allGraphics[textureIndex++]=GetShapeTexture(shape);
	}

	//load art text from resources and create individual enemies and bullets
	Texture2D GetShapeTexture(string shape)
    {
		Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.ARGB4444, false);
		texture.filterMode=FilterMode.Bilinear;
		texture.wrapMode=TextureWrapMode.Clamp;
		switch(shape.ToLower()){
			case "hearts":
			PaintHearts(texture);
			break;
			case "clubs":
			PaintClubs(texture);
			break;
			case "spades":
			PaintSpades(texture);
			break;
			case "diamond":
			PaintDiamond(texture);
			break;
		}
		texture.Apply();
		return texture;
    }
	void PaintHearts(Texture2D texture){
		/* //another way to draw a better heart shape
		// x = 16 * sin^3(t) 0r x= 4 (3 sin(t) - sin(3 t))
		// y = 13 * cos(t) - 5 * cos(2 * t) - 2 * cos(3 * t) - cos(4 * t)
		// t = [0:2 * pi]
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				texture.SetPixel(i, j, Color.clear);
			}
		}
		float interval=0.001f;
		float x,y;
		float offsetVal=resolution/2;
		int multiplier=resolution/32;
		for(float t=0;t<=2*Mathf.PI;t+=interval){
			y = (13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t));
			x =  (4*(3* Mathf.Sin(t)- Mathf.Sin(3*t)));
			texture.SetPixel((int)(offsetVal+multiplier*x), (int)(1.1f*offsetVal+multiplier*y), Color.white);
		}
		*/
		
		
		float radius=(resolution/3.0f);
		Debug.Log(radius);
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				if(ValidHeartPosition(i,j,radius)){
					texture.SetPixel(i, j, Color.white);
				}else{
					texture.SetPixel(i, j, Color.clear);
				}
			}
		}
	}

    private bool ValidHeartPosition(int i, int j, float radius)
    {
        //i =col j = row
		//circle equation (x – h)2 + (y – k)2 = r2
		bool isValid=false;
		float iValue;
		/* //two circles and one square approach
		//for a resolution of x we can have a radius of x/3 as one side od the square will be 2*radius
		if(j<2*radius){//then both circle and square exists in this row
			if(i>radius){//square
				if(j!=0)
				isValid=true;
			}else{//circle with centre at r,r (i=(sqrt(r^2-(j-r)^2))+r)
				iValue=(Mathf.Sqrt(radius*radius-((j-radius)*(j-radius))))+radius;
				offsetiValue=(-1*iValue)+(2*radius);//move wrt centre of the circle
				if(i>offsetiValue){
					isValid=true;
				}
			}
		}else {//if(j!=resolution-1){
			if(i>radius){//circle with centre at 2r,2r (j=(sqrt(r^2-(i-2r)^2))+2r)
				jValue=(Mathf.Sqrt(radius*radius-((i-(2*radius))*(i-(2*radius)))))+(2*radius);
				if(j<jValue){
					isValid=true;
				}
			}
		}*/
		//two circles and slope fill method
		radius =resolution*0.26f;
		Vector2 mid=new Vector2(radius,resolution-radius);
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		mid=new Vector2(resolution-radius,resolution-radius);
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		float width=resolution*0.58f;
		int endJ=(int)(resolution*0.65f);
		int startJ=(int)(resolution*0.1f);
		float delta=(width/endJ);
		float midI=resolution*0.5f;
		if(j>startJ&&j<endJ){
			if(i>(midI-(delta*(j-startJ)))&&i<(midI+(delta*(j-startJ)))){
				isValid=true;
			}
		}
		return isValid;
    }

    void PaintClubs(Texture2D texture){
		int radius=(int)(resolution*0.24f);
		Debug.Log(radius);
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				if(ValidClubPosition(i,j,radius)){
					texture.SetPixel(i, j, Color.white);
				}else{
					texture.SetPixel(i, j, Color.clear);
				}
			}
		}
	}

    private bool ValidClubPosition(int i, int j, int radius)
    {//i =col j = row
        bool isValid=false;
		float iValue;
		Vector2 mid=new Vector2(resolution*0.5f,resolution-radius);
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		mid=new Vector2(resolution*0.25f,resolution-(2.5f*radius));
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		mid=new Vector2(resolution*0.75f,resolution-(2.5f*radius));
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		//draw the base

		return isValid;
    }

    void PaintSpades(Texture2D texture){
		float radius=(resolution/3.0f);
		Debug.Log(radius);
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				if(ValidSpadePosition(i,j,radius)){
					texture.SetPixel(i, j, Color.white);
				}else{
					texture.SetPixel(i, j, Color.clear);
				}
			}
		}
	}

    private bool ValidSpadePosition(int i, int j, float radius)
    {
        //two circles and slope fill method
		//i =col j = row
        bool isValid=false;
		radius =resolution*0.26f;
		float iValue;
		Vector2 mid=new Vector2(radius,resolution-2.2f*radius);
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		mid=new Vector2(resolution-radius,resolution-2.2f*radius);
		iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
		if(i>mid.x-iValue&&i<mid.x+iValue){
			isValid=true;
		}
		
		float width=resolution*0.49f;
		int startJ=(int)(resolution*0.52f);
		float delta=(width/(resolution-startJ));
		float midI=resolution*0.5f;
		if(j>startJ&&j<resolution){
			j=resolution-j;
			if(i>(midI-(delta*(j)))&&i<(midI+(delta*(j)))){
				isValid=true;
			}
		}
		//draw base
		
		return isValid;
    }

    void PaintDiamond(Texture2D texture){
		float width=resolution*0.35f;
		Debug.Log(width);
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				if(ValidDiamondPosition(i,j,width)){
					texture.SetPixel(i, j, Color.white);
				}else{
					texture.SetPixel(i, j, Color.clear);
				}
			}
		}
		
	}

    private bool ValidDiamondPosition(int i, int j, float width)
    {
		//i =col j = row
        bool isValid=false;
		float midI=(resolution/2.0f);
		float midJ=(resolution/2.0f);
		float delta=(width/midJ);
		if(j>midJ){
			j=resolution-j;
			if(i>(midI-(delta*j))&&i<(midI+(delta*j))){
				isValid=true;
			}
		}else{
			if(i>(midI-(delta*j))&&i<(midI+(delta*j))){
				isValid=true;
			}
		}
		return isValid;
    }

    //packs all textures into single texture to help reduce draw calls
    public void PackTextures()
    {
		packedTexture = new Texture2D(8, 8, TextureFormat.ARGB4444, false);
		packedTexture.filterMode=FilterMode.Bilinear;
		packedTexture.wrapMode=TextureWrapMode.Clamp;
		packedAssets=packedTexture.PackTextures(allGraphics,1);
		Debug.Log("packed textures to "+packedTexture.width+"x"+packedTexture.height);
		
		foreach (Texture2D tex in allGraphics)
		{
			Texture2D.DestroyImmediate(tex, true);
		}
		allGraphics=null;
    }
	//needed to rotate array clockwise due to origin difference for texture2d
	int[,] rotateCW(int[,] arr,int rows,int cols) {
		int M = rows;
		int N = cols;
		int[,] ret = new int[N,M];
		for (int r = 0; r < M; r++) {
			for (int c = 0; c < N; c++) {
				ret[c,M-1-r] = arr[r,c];
			}
		}
		return ret;
	}
	//needed to convert UV (0,1) coordinates to texture pixel coordinates
	private Rect ConvertUVToTextureCoordinates(Rect rect)
    {
        return new Rect(rect.x*packedTexture.width,
			rect.y*packedTexture.height,
			rect.width*packedTexture.width,
			rect.height*packedTexture.height
		);
    }
}
