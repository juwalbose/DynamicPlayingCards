using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager {
	Rect[] packedAssets;
	Texture2D[] allGraphics;
	int textureIndex;
	Dictionary<string,int> textureDict;
	int resolution;
	FilterMode filterMode=FilterMode.Trilinear;
	Texture2D packedTexture;
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
		allGraphics=new Texture2D[6];
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
		texture.filterMode=filterMode;
		texture.wrapMode=TextureWrapMode.Clamp;
		PaintRectangle(texture,new Rect(0,0,resolution,resolution),Color.clear);
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

    public void CreateRoundedRectangle(string shape){
		textureDict.Add(shape.ToLower(),textureIndex);
		switch(shape.ToLower()){
			case "base":
			allGraphics[textureIndex++]=GetRoundedRectangle();
			break;
			case "back":
			allGraphics[textureIndex++]=GetRoundedBackRectangle();
			break;
		}
	}
	private Texture2D GetRoundedBackRectangle()
    {//366x512 tiling size
		int tileResolution=resolution/8;
		Texture2D tilingTexture = new Texture2D(tileResolution, tileResolution, TextureFormat.ARGB4444, false);
		tilingTexture.filterMode=filterMode;
		tilingTexture.wrapMode=TextureWrapMode.Clamp;
		PaintRectangle(tilingTexture,new Rect(0,0,tileResolution,tileResolution),Color.black);
		PaintTilingDesign(tilingTexture,tileResolution);
		tilingTexture.Apply();
		return tilingTexture;
    }

    private void PaintTilingDesign(Texture2D texture, int tileResolution)
    {
		Vector2 mid=new Vector2(tileResolution/2,tileResolution/2);
		float size=0.6f*tileResolution;
		PaintDiamond(texture,new Rect(mid.x-size/2,mid.y-size/2,size,size),mid,Color.red);
		mid=new Vector2(0,0);
		PaintDiamond(texture,new Rect(mid.x-size/2,mid.y-size/2,size,size),mid,Color.red);
		mid=new Vector2(tileResolution,0);
		PaintDiamond(texture,new Rect(mid.x-size/2,mid.y-size/2,size,size),mid,Color.red);
		mid=new Vector2(tileResolution,tileResolution);
		PaintDiamond(texture,new Rect(mid.x-size/2,mid.y-size/2,size,size),mid,Color.red);
		mid=new Vector2(0,tileResolution);
		PaintDiamond(texture,new Rect(mid.x-size/2,mid.y-size/2,size,size),mid,Color.red);
    }

    private Texture2D GetRoundedRectangle()
    {
		Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.ARGB4444, false);
		texture.filterMode=filterMode;
		texture.wrapMode=TextureWrapMode.Clamp;
		PaintRoundedRectangle(texture);
		texture.Apply();
		return texture;
    }

    private void PaintRoundedRectangle(Texture2D texture)
    {
		// 1 : 1.4 scale this
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				if(ValidRRPosition(i,j)){
					texture.SetPixel(i, j, Color.white);
				}else{
					texture.SetPixel(i, j, Color.clear);
				}
			}
		}
    }

    private bool ValidRRPosition(int i, float j)
    {
        bool isValid=false;
		float iValue;
		Vector2 mid;
		// (x/a)^n + (y/b)^n = 1
		//(x-h/a)^n + (y-k/b)^n = 1
		//x= (a/b (nth root of(b^n-(y-k)^n))) + h
		mid=new Vector2(resolution/2,resolution/2);
		int radius=(int)(resolution/2);
		//Debug.Log(radius);
		int a=radius;
		int b=radius;
		float n=16;
		float nRoot=1/n;
		float delta=j-mid.y;
		float partialOne=Mathf.Pow(b,n)-Mathf.Pow(delta,n);
		iValue=((a/b)*Mathf.Pow(partialOne,nRoot));//+mid.x
		if(i>mid.x-iValue && i<mid.x+iValue){
			isValid=true;
		}
		
		return isValid;
    }

    void PaintHearts(Texture2D texture){
		//another way to draw a better heart shape
		// x = 16 * sin^3(t) 0r x= 4 (3 sin(t) - sin(3 t))
		// y = 13 * cos(t) - 5 * cos(2 * t) - 2 * cos(3 * t) - cos(4 * t)
		// t = [0:2 * pi]
		
		//2 circles on top
		float radius =resolution*0.26f;
		Vector2 mid=new Vector2(radius,resolution-radius);
		PaintCircle(texture,radius,mid,Color.red);
		mid=new Vector2(resolution-radius,resolution-radius);
		PaintCircle(texture,radius,mid,Color.red);
		//triangle at bottom
		float width=resolution*0.58f;
		int endJ=(int)(resolution*0.65f);
		int startJ=(int)(resolution*0.1f);
		float delta=(width/endJ);
		float midI=resolution*0.5f;
		for (int i=0;i<resolution;i++){
			for (int j=startJ;j<endJ;j++){
				if(i>(midI-(delta*(j-startJ)))&&i<(midI+(delta*(j-startJ)))){
					texture.SetPixel(i, j, Color.red);
				}
			}
		}
	}

    void PaintClubs(Texture2D texture){
		int radius=(int)(resolution*0.24f);
		//3 circles
		Vector2 mid=new Vector2(resolution*0.5f,resolution-radius);
		PaintCircle(texture,radius,mid,Color.black);
		mid=new Vector2(resolution*0.25f,resolution-(2.5f*radius));
		PaintCircle(texture,radius,mid,Color.black);
		mid=new Vector2(resolution*0.75f,resolution-(2.5f*radius));
		PaintCircle(texture,radius,mid,Color.black);
		//base stalk
		radius=(int)(resolution*0.5f);
		float midY=resolution*0.42f;
		int stalkHeightJ=(int)(resolution*0.65f);
		float iValue;
		
		for (int i=0;i<resolution;i++){
			for (int j=0;j<stalkHeightJ;j++){
				mid=new Vector2(resolution*-0.035f,midY);
				iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
				if(i>mid.x+iValue){
					mid=new Vector2(resolution*1.035f,midY);
					iValue=(Mathf.Sqrt(radius*radius-((j-mid.y)*(j-mid.y))));//+mid.x;
					if(i<mid.x-iValue){
						texture.SetPixel(i, j, Color.black);
					}
				}
			}
		}
	}

    void PaintSpades(Texture2D texture){
		//2 circles on top
		float radius =resolution*0.26f;
		Vector2 mid=new Vector2(radius,resolution-2.2f*radius);
		PaintCircle(texture,radius,mid,Color.black);
		mid=new Vector2(resolution-radius,resolution-2.2f*radius);
		PaintCircle(texture,radius,mid,Color.black);
		//triangle at bottom
		float width=resolution*0.49f;
		int startJ=(int)(resolution*0.52f);
		float delta=(width/(resolution-startJ));
		float midI=resolution*0.5f;
		int alteredJ;
		radius=resolution*0.5f;
		float midJ=resolution*0.42f;
		float iValue;
		
		for (int i=0;i<resolution;i++){
			//top triangle
			for (int j=startJ;j<resolution;j++){
				alteredJ=resolution-j;
				if(i>(midI-(delta*alteredJ))&&i<(midI+(delta*alteredJ))){
					texture.SetPixel(i, j, Color.black);
				}
			}
			//bottom stalk
			for (int k=0;k<resolution*0.5f;k++){
				mid=new Vector2(0,midJ);
				iValue=(Mathf.Sqrt(radius*radius-((k-mid.y)*(k-mid.y))));//+mid.x;
				if(i>mid.x+iValue){
					mid=new Vector2(resolution,midJ);
					iValue=(Mathf.Sqrt(radius*radius-((k-mid.y)*(k-mid.y))));//+mid.x;
					if(i<mid.x-iValue){
						texture.SetPixel(i, k, Color.black);
					}
				}
			}
		}
	}

    void PaintDiamond(Texture2D texture){
		
		float width=resolution*0.35f;
		Debug.Log(width);
		for (int i=0;i<resolution;i++){
			for (int j=0;j<resolution;j++){
				if(ValidDiamondPosition(i,j,width)){
					texture.SetPixel(i, j, Color.red);
				}
			}
		}
		/* 
		Vector2 mid=new Vector2(resolution/2,resolution/2);
		float size=resolution;
		PaintDiamond(texture,new Rect(mid.x-size/2,mid.y-size/2,size,size),mid,Color.red,0.9f);
		*/
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
	private void PaintRectangle(Texture2D texture, Rect rectBounds, Color color)
    {
        for (int i=(int)rectBounds.x;i<rectBounds.x+rectBounds.width;i++){
			for (int j=(int)rectBounds.y;j<rectBounds.y+rectBounds.height;j++){
				texture.SetPixel(i, j, color);
			}
		}
    }

	private void PaintCircle(Texture2D texture,float radius, Vector2 midPoint, Color color){
		//i =col j = row
		//circle equation (x – h)2 + (y – k)2 = r2
		Rect circleBounds=new Rect();
		circleBounds.x=Mathf.Clamp(midPoint.x-(radius),0,resolution);
		circleBounds.y=Mathf.Clamp(midPoint.y-(radius),0,resolution);
		circleBounds.width=Mathf.Clamp(2*radius,0,resolution);
		circleBounds.height=Mathf.Clamp(2*radius,0,resolution);
		float iValue;
		for (int i=(int)circleBounds.x;i<circleBounds.x+circleBounds.width;i++){
			for (int j=(int)circleBounds.y;j<circleBounds.y+circleBounds.height;j++){
				iValue=(Mathf.Sqrt(radius*radius-((j-midPoint.y)*(j-midPoint.y))));
				if(i>midPoint.x-iValue&&i<midPoint.x+iValue){
					texture.SetPixel(i, j, color);
				}
			}
		}
	}
	private void PaintDiamond(Texture2D texture, Rect rectBounds, Vector2 midPoint, Color color, float n=0.8f)
    {
        float iValue;
		// (x/a)^n + (y/b)^n = 1
		//(x-h/a)^n + (y-k/b)^n = 1
		//x= (a/b (nth root of(b^n-(y-k)^n))) + h
		int a=(int)(rectBounds.width/2);
		int b=(int)(rectBounds.height/2);
		//float n=0.8f;
		float nRoot=1/n;
		float delta;
		float partialOne;
		rectBounds.width=Mathf.Clamp(rectBounds.x+rectBounds.width,0,resolution);
		rectBounds.height=Mathf.Clamp(rectBounds.y+rectBounds.height,0,resolution);
		rectBounds.x=Mathf.Clamp(rectBounds.x,0,resolution);
		rectBounds.y=Mathf.Clamp(rectBounds.y,0,resolution);
		for (int i=(int)rectBounds.x;i<rectBounds.width;i++){
			for (int j=(int)rectBounds.y;j<rectBounds.height;j++){
				delta=Mathf.Abs(j-midPoint.y);
				partialOne=Mathf.Pow(b,n)-Mathf.Pow(delta,n);
				iValue=((a/b)*Mathf.Pow(partialOne,nRoot));//+mid.x
				if(i>midPoint.x-iValue && i<midPoint.x+iValue){
					texture.SetPixel(i, j, color);
				}
			}
		}
    }

    //packs all textures into single texture to help reduce draw calls
    public void PackTextures()
    {
		packedTexture = new Texture2D(8, 8, TextureFormat.ARGB4444, false);
		packedTexture.filterMode=filterMode;
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
