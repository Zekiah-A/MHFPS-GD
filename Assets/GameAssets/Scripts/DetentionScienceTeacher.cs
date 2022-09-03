using Godot;
using System;
using System.Drawing;
using System.Linq;
using Color = Godot.Color;

public class DetentionScienceTeacher : Node //: DetentionManager
{
	private float minWhiteValue = 0.05f;
	
	private readonly string[] numbers =
	{
		"##########\n###****###\n##*####*##\n#*######*#\n#*######*#\n#*######*#\n#*######*#\n##*####*##\n###****###\n##########", //O as ascii `*` & `#`
		"##########\n####*#####\n###**#####\n##*#*#####\n####*#####\n####*#####\n####*#####\n####*#####\n##*****###\n##########", //1
		"##########\n###****###\n##*####*##\n##*####*##\n######*###\n#####*####\n####*#####\n###*######\n##******##\n##########", //2
		"##########\n##*****###\n#######*##\n#######*##\n######**##\n##*****###\n#######*##\n#######*##\n##*****###\n##########"  //3
	};
	
	public override void _Ready()
	{
		OnSubmitAnswerClicked();
	}

	public async void OnSubmitAnswerClicked()
	{
		for (int n = 0; n < numbers.Length; n++)
		{
			for (int i = 0; i < 4; i++) //Wait 4 frames for screen to load
				await ToSignal(GetTree(), "idle_frame");

			var uImg = GetViewport().GetTexture().GetData();
			uImg.FlipY();
			var img = uImg.GetRect(new Rect2(GetNode<Control>("AnswerDrawArea").RectGlobalPosition, GetNode<Control>("AnswerDrawArea").RectSize));
			
			for (int i = 0; i < 4; i++) //160 -> 80 //80 -> 40 // 40 -> 20 // 20 -> 10
				img.ShrinkX2();
			img.Lock();

			var match = 0;
			var total = 0;
			
			for (int h = 0; h < img.GetHeight(); h++)
			{
				for (int w = 0; w < img.GetWidth(); w++)
				{
					var asciiColour = numbers[n].Split("\n")[h][w] == '#' ? Colors.Black : Colors.White;
					if (asciiColour == Colors.White)
						total++;
					if (img.GetPixel(w, h).r > minWhiteValue && img.GetPixel(w, h).g > minWhiteValue && img.GetPixel(w, h).b > minWhiteValue && asciiColour == Colors.White)
						match++;
				}
			}
			GD.Print($"ascii: {n} match:{match} total: {total} percent: {(float)match/total*100}");
		}
	}
}
