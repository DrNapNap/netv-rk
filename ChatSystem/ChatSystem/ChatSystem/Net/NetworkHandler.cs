using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using static ChatSystem.NetworkMessageBaseEventHandler;

namespace ChatSystem
{
    public class NetworkHandler
    {

        static int port = 11000;
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        UdpClient client;
        NetworkMessageBaseEventHandler messageHandler;
        public NetworkHandler(NetworkMessageBaseEventHandler networkMessageBaseEventHandler)
        {
            this.messageHandler = networkMessageBaseEventHandler;
            client = new UdpClient();
            client.Connect(groupEP);
            Thread ListeningThread = new Thread(Listening);
            ListeningThread.IsBackground = true;
            ListeningThread.Start();
        }

        public void SendMessageToServer(NetworkMessageBase networkMessage)
        {
  
                var message = new NetworkMessage()
                {
                    type = MessageType.join,
                    message = networkMessage,
                    //playerName  = networkMessage.playerName
                };

                var serializedNetworkMessage = JsonConvert.SerializeObject(message);

                byte[] jsonAsBytes = Encoding.UTF8.GetBytes(serializedNetworkMessage);

                Debug.WriteLine($"sending json message{serializedNetworkMessage} to server!");



                client.Send(jsonAsBytes);
            
        }

        public void AddListener<T>(EventDelegate<T> setInitialPositionsMessage) where T : NetworkMessageBase
        {
            messageHandler.AddListener<T>(setInitialPositionsMessage);
        }
        public void RemoveListener<T>(EventDelegate<T> setInitialPositionsMessage) where T : NetworkMessageBase
        {
            messageHandler.RemoveListener<T>(setInitialPositionsMessage);
        }
        void Listening()
        {
            try
            {
                while (true)
                {
                    var data = client.Receive(ref groupEP); // listen on port 11000

                    var dataDeEncodedShouldBeJson = Encoding.UTF8.GetString(data);


                    JObject? complexMessage = JObject.Parse(dataDeEncodedShouldBeJson);
                    JToken? complexMessagType = complexMessage["type"];

                    Debug.WriteLine("got somehting");
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
                        NetworkMessageBase networkMessage = null;
                        switch (mesType) //Only messages meant for client to react to.
                        {
                            case MessageType.snapshot:
                                networkMessage = complexMessage["message"].ToObject<SnapShot>();
                                Debug.WriteLine("got a snapshot!" + complexMessage);
                                messageHandler.Raise(networkMessage);
                                break;
                            case MessageType.initialJoin:
                                networkMessage = complexMessage["message"].ToObject<SetInitialPositionsMessage>();
                                Debug.WriteLine("got a initialJoinMessage!!" + complexMessage);
                                messageHandler.Raise(networkMessage);
                                break;
                            default:
                                break;
                        }
                    }

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
