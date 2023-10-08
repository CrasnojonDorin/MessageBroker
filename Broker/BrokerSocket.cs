using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    class BrokerSocket
    {
        private Socket _socket;
        private const int CONNECTIONS_LIMIT = 8;
        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        public void Start(string ip, int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _socket.Listen(port);
            Accept();
        }
        private void Accept()
        {
            _socket.BeginAccept(AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                connection.Socket = _socket.EndAccept(asyncResult);
                connection.Address = connection.Socket.RemoteEndPoint.ToString();
                //127.0.0.1:9000
                connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                    SocketFlags.None, ReceiveCallback, connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't accept. {ex.Message}");
            }
            finally
            {
                Accept();
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = asyncResult.AsyncState as ConnectionInfo;

            try
            {
                Socket senderSocket = connection.Socket;
                SocketError response;
               int buffSize= senderSocket.EndReceive(asyncResult,out response);

                if(response == SocketError.Success) {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connection.Data, payload, payload.Length);

                    //Handle payload
                    PayloadHandler.Handle(payload, connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't receive data: {ex.Message}");
           
            }
            finally
            {
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                        SocketFlags.None, ReceiveCallback, connection);
                }
                catch(Exception e) {
                    Console.WriteLine($"{e.Message}");

                    //extragem adresa
                    var address = connection.Socket.RemoteEndPoint.ToString();
                    
                    //stergem din storage
                    ConnectionsStorage.Remove(address);
                    
                    connection.Socket.Close();
                  
                }
            }
        }
    }
}
