
// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using UDPongServer;

int port = 11000; // listening port
UdpClient listener = new UdpClient(port); // create listener
IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);//listen to ANY ip, that sends on the given port!

//call function 60 times a sec Setup Timer
float updateInterval = 60;
System.Timers.Timer timer = new System.Timers.Timer();
timer.Interval = (double)1000f / updateInterval;
List<Pad> pads = new List<Pad>();

//Pad pad = null;

//Dummy values for proof of concept.
//This is the actual gamesate, should proably be configured to classes.
int ballXPos = 0;
int ballYPos = 0;

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
            Console.WriteLine("waiting for the datas");
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


    if (ballXPos <0)
    {
        //reset ?
    }
    List<float> listOfPadPositionY = new List<float>();
    foreach (var pad in pads)
    {
        listOfPadPositionY.Add(pad.PositionY);
    }
    ballXPos += 1;
    ballYPos += -1;
    SnapShot snapShot = new SnapShot() { ballXpos = ballXPos, ballYPos = ballYPos, playerYPos = listOfPadPositionY };
    foreach (var pad in pads)
    {
        SendTypedNetworkMessage(listener, pad.IPEndPoint, snapShot, MessageType.snapshot);
    }

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
                PlayerMovemenUpdate playerMoveMessage = complexMessage["message"].ToObject<PlayerMovemenUpdate>();
                HandleMoveMessage(messageSenderInfo, listener, playerMoveMessage);
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

void HandleMoveMessage(IPEndPoint messageSenderInfo, UdpClient listener, PlayerMovemenUpdate playerMoveUpdate)
{
    //Find pad based on message sender info
    var pad = pads.Where(x => x.IPEndPoint.ToString() == messageSenderInfo.ToString()).First();
    if (playerMoveUpdate.direction == Direction.down)
    {
        pad.PositionY += 1;
    }

    if (playerMoveUpdate.direction == Direction.up)
    {
        pad.PositionY -= 1;
    }
}

void HandleJoinMessage(IPEndPoint messageSenderInfo, UdpClient listener, JoinMessage recievedJoinMessage)
{
    Console.WriteLine("recieved join message!");
    var pad = new Pad(messageSenderInfo);
    pad.PositionX = recievedJoinMessage.ResolutionX / 2;
    pad.PositionY = recievedJoinMessage.ResolutionY / 2;

    pads.Add(pad);
    ballXPos = recievedJoinMessage.ResolutionX / 2;
    ballYPos = recievedJoinMessage.ResolutionY / 2;
    var networkMessage = new SetInitialPositionsMessage()
    {
        ballXpos = recievedJoinMessage.ResolutionX / 2,
        ballYPos = recievedJoinMessage.ResolutionY / 2,
        leftPlayerXPos = 0,
        leftPlayeryYPos = recievedJoinMessage.ResolutionY / 2,
        rightPlayeryXPos = recievedJoinMessage.ResolutionX,
        rightPlayeryYPos = recievedJoinMessage.ResolutionY / 2,
        isLeftPlayer = (pads.Count==1)
    };

    SendTypedNetworkMessage(listener, messageSenderInfo, networkMessage, MessageType.initialJoin);
    if (pads.Count == 2)
    {
        //should actually start when two players have joined...
        timer.Start();
    }

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