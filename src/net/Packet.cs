using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace NightmareNegotiations.net;

public partial class BasicPacket : RefCounted
{
    [JsonProperty("message")] public Message Message { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
}

public partial class UserInfoPacket : BasicPacket
{
    [JsonProperty("username")] public string Username { get; set; }
    
    public UserInfoPacket()
    {
        Message = Message.UserInfo;
    }
}

public partial class LobbyListReceivePacket : BasicPacket
{
    [JsonProperty("lobby_list")] public List<Lobby> LobbyList { get; set; }

    public LobbyListReceivePacket()
    {
        Message = Message.LobbyList;
    }

    public class Lobby
    {
        [JsonProperty("lobby_code")] public string LobbyCode { get; private set; }
        [JsonProperty("lobby_description")] public string LobbyDescription { get; set; } = "";
    }
}