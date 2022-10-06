
// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;


int port = 11000; // listening port
UdpClient listener = new UdpClient(port); // create listener
IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);//listen to ANY ip, that sends on the given port!

//call function 60 times a sec Setup Timer
float updateInterval = 60;
System.Timers.Timer timer = new System.Timers.Timer();
timer.Interval = (double)1000f / updateInterval;

//Dummy values for proof of concept.
//This is the actual gamesate, should proably be configured to classes.
int ballXPos = 0;
int ballYPos = 0;
Pad pad = null;


timer.Elapsed += SedingTimer;

Thread ListeningThread = new Thread(Listening);
ListeningThread.Start();




void Listening()
{
    try
    {
        Console.WriteLine($"Listening on port: {port}");
        while (true)
        {
            //Console.WriteLine("waiting for the datas");
            var data = listener.Receive(ref groupEP);

            OtherHandleMessage(data, groupEP);

        }
    }
    catch (SocketException e)
    {
        Console.WriteLine(e);
    }
    finally
    {
        listener.Close();
    }
}


void SedingTimer(object? sender, ElapsedEventArgs e)
{
    //simular to update loop :)

    //update ball pos

    //is ball outside of resolution?? Does somehting happen?

    //Get player pos from worldstate?

    //All the actual game logic goes here. Or at least this is the starting point.
    List<float> list = new List<float>();

    list.Add(pad.PosY);

    ballXPos += 1;
    ballYPos += -1;
    SnapShot snapShot = new SnapShot() { ballXpos = ballXPos, ballYPos = ballYPos, playerYPos = list};
    SendTypedNetworkMessage(listener, groupEP, snapShot, MessageType.snapshot);

}
void OtherHandleMessage(byte[] data, IPEndPoint messageSenderInfo)
{
    var dataDeEncodedShouldBeJson = Encoding.UTF8.GetString(data);

    JObject? complexMessage = JObject.Parse(dataDeEncodedShouldBeJson);
    JToken? complexMessagType = complexMessage["type"];

    if (complexMessage != null && complexMessagType?.Type is JTokenType.Integer)
    {
        //we have a message that is successfully serialized
        //the message "Type" is int (enum)
        //safe to cast
        MessageType mesType = (MessageType)complexMessagType.Value<int>();
        JToken? complexMessagMessage = complexMessage["message"];
        if (complexMessagMessage == null)
        {
            return;
        }
        switch (mesType) // only messages that server wants to react to, therefore no need for initial join, that is meant for clients
        {
            case MessageType.movement:
                PlayerMovemenUpdate recievedMoveMessage = complexMessage["message"].ToObject<PlayerMovemenUpdate>();
                HandleMoveMessage(messageSenderInfo, listener, recievedMoveMessage);

                break;
            case MessageType.join:
                JoinMessage recievedJoinMessage = complexMessage["message"].ToObject<JoinMessage>();
                HandleJoinMessage(messageSenderInfo, listener, recievedJoinMessage);
                break;
            default:
                break;
        }
    }
}

void HandleMoveMessage(IPEndPoint messageSenderInfo, UdpClient listener, PlayerMovemenUpdate recievedJoinMessage)
{
    if (recievedJoinMessage.direction == Direction.up)
    {
        pad.PosY += 1;

    }
    if (recievedJoinMessage.direction == Direction.down)
    {
        pad.PosX -= 1;

    }


}


void HandleJoinMessage(IPEndPoint messageSenderInfo, UdpClient listener, JoinMessage recievedJoinMessage)
{
       pad = new Pad();
 
    pad.PosX = recievedJoinMessage.ResolutionX / 2;

    pad.PosY = recievedJoinMessage.ResolutionY / 2;

    ballXPos = recievedJoinMessage.ResolutionX / 2;
    ballYPos = recievedJoinMessage.ResolutionY / 2;
    var networkMessage = new SetInitialPositionsMessage()
    {
        ballXpos = recievedJoinMessage.ResolutionX / 2,
        ballYPos = recievedJoinMessage.ResolutionY / 2,
        leftPlayerXPos = 0,
        leftPlayeryYPos = recievedJoinMessage.ResolutionY / 2,
        rightPlayeryXPos = recievedJoinMessage.ResolutionX,
        rightPlayeryYPos = recievedJoinMessage.ResolutionY / 2
    };

    SendTypedNetworkMessage(listener, messageSenderInfo, networkMessage, MessageType.initialJoin);
    //should actually start when two players have joined...
    timer.Start();
}

static void SendTypedNetworkMessage(UdpClient listener, IPEndPoint groupEP, NetworkMessageBase networkMessageBase, MessageType messageType)
{
    var message = new NetworkMessage()
    {
        type = messageType,
        message = networkMessageBase
    };
    var serializedNetworkMessage = JsonConvert.SerializeObject(message);

    byte[] jsonAsBytes = Encoding.UTF8.GetBytes(serializedNetworkMessage);

    Debug.WriteLine($"sending json message{serializedNetworkMessage} to client!!");

    listener.Send(jsonAsBytes, groupEP);
}