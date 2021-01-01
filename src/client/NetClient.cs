using System.Threading;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace Netflake {

    public class NetClient {

        private TcpClient _client;
        private Thread _thread;
        private Socket _socket;
        private NetworkHandler _handler;
        private ServerContext _context;
        private string _hostname;
        private int _port;

        public NetClient () {
            this._client = new TcpClient();
            this._handler = new NetworkHandler();
            this._context = new ServerContext();
        }

        public NetworkHandler NetworkHandler {
            get {
                return this._handler;
            }
        }

        public ServerContext Context {
            get {
                return this._context;
            }
        }

        private void _OnMessageReceived (byte[] buffer) {
            Console.WriteLine(ByteUtils.ToString(buffer));

            object packet = this._handler.HandleIncoming(this, buffer);
            if (packet != null) {
                string data = ByteUtils.ToString(buffer);
                Console.WriteLine(data);

                this.Write("Hello from client");
            }
        }

        private void _KeepAliveHandler () {
            while (true) {
                this.Write("head_type=keepalive");
                Thread.Sleep(1000);
            }
        }

        private void _OnServerRead (IAsyncResult asyncResult) {
            try {
                int read = this._context.Stream.EndRead(asyncResult);
                this._context.Memory.Write(this._context.Buffer, 0, read);

                int length = BitConverter.ToInt32(this._context.Buffer, 0);
                byte[] buffer = new byte[1024];

                while (true) {
                    read = this._context.Stream.Read(buffer, 0, Math.Min(buffer.Length, length));
                    this._context.Memory.Write(buffer, 0, read);

                    if (read != 0) {
                        this._OnMessageReceived(this._context.Buffer);
                    }
                }
            }

            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private void _OnClientConnect (IAsyncResult asyncResult) {
            try {
                this._context.Stream = this._client.GetStream();
                this._context.Stream.BeginRead(this._context.Buffer, 0, this._context.Buffer.Length, this._OnServerRead, 777);
                new Thread(new ThreadStart(this._KeepAliveHandler)).Start();
            }

            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private void _ConnectTask () {
            // Initialize Socket from client
            this._socket = this._client.Client;

            // Connect to the server
            this._client.BeginConnect(this._hostname, this._port, this._OnClientConnect, 777);
        
            // Lock thread
            while (this._socket.IsBound) {}
        }

        public void Connect (string hostname, int port) {
            this._hostname = hostname;
            this._port = port;

            this._thread = new Thread(new ThreadStart(this._ConnectTask));
            this._thread.Start();
        }

        public void Write (byte[] buffer) {
            Console.WriteLine("Trying to write " + buffer.Length + " bytes to server");

            object packet = this._handler.HandleOutgoing(this, buffer);
            if (packet != null) {
                Console.WriteLine("Packet isn't null, sending to server");
                byte[] bytes = (byte[]) packet;

                try {
                    this._socket.Send(bytes);
                    Console.WriteLine("Send succefully");
                } 
                
                catch (Exception e) {
                    Console.WriteLine(e);
                    this._socket.Close();
                    this._client.Close();
                    this._context.Stream.Dispose();
                    this._context.Memory.Dispose();
                }
            }
        }

        public void Write (string data) {
            byte[] buffer = ByteUtils.ToBuffer(data);
            this.Write(buffer);
        }

    }

}