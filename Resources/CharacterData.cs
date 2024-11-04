using Godot;

namespace NightmareNegotiations;

public partial class CharacterData : Resource
{
	[Export] public float MoveSpeed { get; set; }

	[Export] public float JumpVelocity { get; set; }
}
