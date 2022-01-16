using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public class PhoneCursor : Control
{
	public bool IsActivated;

	private Panel phoneScreen;
	private TextureRect cursor;
	
	public override void _Ready()
	{
		phoneScreen = GetNode<Panel>("PhoneScreen");
		cursor = phoneScreen.GetNode<TextureRect>("Cursor");
	}

	public override void _Process(float delta) //TODO: C# math vs godot mathf
	{
		Update();
		GD.Print(GetMouseAngle());
		GD.Print("opp: " + (GetGlobalMousePosition().y - GetPhoneCentre().y));
		GD.Print("adj: " + (GetGlobalMousePosition().x - GetPhoneCentre().x));
	}

	public override void _Draw()
	{
		//Centre to mouse line.
		DrawLine(GetPhoneCentre(), GetGlobalMousePosition(), Color.Color8(255, 255, 255, 255));
		//Quadrants
		//DrawLine(GetQuadrantVerticies()[0], GetQuadrantVerticies()[1], Colors.Gray);
		//DrawLine(GetQuadrantVerticies()[1], GetQuadrantVerticies()[2], Colors.Gray);
		//DrawLine(GetQuadrantVerticies()[0], GetQuadrantVerticies()[2], Colors.Red);
	}

	private float GetMouseAngle()
	{	//Use trigonometric tan function to find the angle between the centre of the rectangle, and the mouse.
		//var e = Mathf.Abs(GetGlobalMousePosition().y - GetPhoneCentre().y) /
		//        Mathf.Abs(GetGlobalMousePosition().x - GetPhoneCentre().x);
		//angle = Math.Atan(oppositeside / adjustantside)*180/Math.PI;
		//var angle = Mathf.Rad2Deg(Mathf.Atan(Mathf.Abs(GetGlobalMousePosition().y - GetPhoneCentre().y) / Mathf.Abs(GetGlobalMousePosition().x - GetPhoneCentre().x))); // * 180 / Mathf.PI;
		//return Mathf.Tan(angle);
		return 0.0f;
	}

	private Vector2[] GetVertexPositions()
	{	//Top left, Top right, Bottom left, Bottom right
		return new Vector2[]
		{
			phoneScreen.RectPosition,
			new Vector2(phoneScreen.RectPosition),
			phoneScreen.RectPosition + phoneScreen.RectSize
		};
	}

	//GetVertexAngle()
	//{
		
	//}

	private Vector2 GetPhoneCentre()
	{	//X position, Y position
		return new Vector2(phoneScreen.RectSize.x / 2 + phoneScreen.RectPosition.x, phoneScreen.RectSize.y / 2 + phoneScreen.RectPosition.y);
	}
}


/*
 extends TextureRect

export var FollowMouse = false;

func _ready():
	pass


func _physics_process(delta):
	if (FollowMouse):
		var finalTexturePos = get_global_mouse_position() - Vector2(20, 20)
		rect_position = lerp(finalTexturePos - (get_viewport().size - get_parent().rect_size), rect_position, 0.1)
		Input.set_mouse_mode(Input.MOUSE_MODE_HIDDEN)
		#get_viewport().warp_mouse(Vector2(0, 0))

 */
