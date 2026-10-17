using UnityEngine;

public class TextureUtils
{
	public static Sprite TextureToSprite(Texture2D texture)
	{
		Rect rec = new Rect(0, 0, texture.width, texture.height);
		Sprite.Create(texture, rec, new Vector2(0, 0), 1);
		return Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
	}
}
