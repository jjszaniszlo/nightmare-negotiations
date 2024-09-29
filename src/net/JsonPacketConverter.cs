using System;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NightmareNegotiations.net;

public class JsonPacketConverter : Newtonsoft.Json.Converters.CustomCreationConverter<BasicPacket>
{
    public override BasicPacket Create(Type objectType)
    {
        throw new NotImplementedException();
    }

    public BasicPacket Create(Type objectType, JObject jObject)
    {
        var messageProperty = (string)jObject.Property("message")!;

        if (int.TryParse(messageProperty, out var message))
        {
            switch ((Message)message)
            {
                case Message.UserInfo:
                    return new UserInfoPacket();
                case Message.LobbyList:
                    return new LobbyListReceivePacket();
                case Message.CreateLobby:
                    return new BasicPacket();
                case Message.LeaveLobby:
                    break;
                case Message.LobbyMessage:
                    break;
                case Message.LobbyDescription:
                    break;
                case Message.Offer:
                    break;
                case Message.Answer:
                    break;
                case Message.InteractiveConnectivityEstablishment:
                    break;
                case Message.StartSession:
                    return new BasicPacket();
            }
        }

        throw new ApplicationException("Packet not implemented!");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var target = Create(objectType, jObject);
        
        serializer.Populate(jObject.CreateReader(), target);

        return target;
    }
}
