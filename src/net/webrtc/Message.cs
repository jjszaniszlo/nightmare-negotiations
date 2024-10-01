using System.Text.Json.Serialization;
using Godot;

namespace NightmareNegotiations.net.webrtc;

public partial class Message : RefCounted
{
	[JsonPropertyName("type")] public MessageType Type { get; set; }
	[JsonPropertyName("id")] public long Id { get; set; }
	[JsonPropertyName("data")] public string Data { get; set; }
}
