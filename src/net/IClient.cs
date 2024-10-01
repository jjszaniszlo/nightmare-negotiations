using Godot;

namespace NightmareNegotiations.net;

public interface IClient
{
	public Error Connect(string url);
	public void Close();
}