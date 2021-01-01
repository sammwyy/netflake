using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Netflake {

    public class NetServer {

        private TcpListener _listener;
        private Thread _thread;
        private int _port = 6502;
        private int _encryptionThreshold = -1;
        private ServerHandler handler;

        public NetServer () {
            this.handler = new ServerHandler();
        }

        public ServerHandler NetworkHandler {
            get {
                return this.handler;
            }
        }

        private void _OnPacketReceived (Client client, object packet) {
            byte[] buffer = (Byte[]) packet;
            Console.WriteLine("Packet received » " + ByteUtils.ToString(buffer));
        }

        private void _OnMessageReceived (Client client) {
            Console.WriteLine("Message received » " + ByteUtils.ToString(client.Context.Buffer));

            ClientContext context = client.Context;
            Object packet = this.handler.HandleIncoming(client, context.Buffer);

            if (packet != null) {
                this._OnPacketReceived(client, packet);
            }
        }

        private void _OnClientRead (IAsyncResult asyncResult) {
            Console.WriteLine("Reading client in async way");

            ClientContext context = asyncResult.AsyncState as ClientContext;
            if (context == null) {
                return;
            }

            Client client = new Client(context, this.handler);

            this.handler.AddClient(client);

            context.State = ClientState.ESTABLISHED;

            try {
                int read = context.Stream.EndRead(asyncResult);
                context.Message.Write(context.Buffer, 0, read);

                int length = BitConverter.ToInt32(context.Buffer, 0);
                byte[] buffer = new byte[1024];

                Console.WriteLine("Reading packet from buffer");

                while (context.State == ClientState.ESTABLISHED) {
                    read = context.Stream.Read(buffer, 0, Math.Min(buffer.Length, length));
                    context.Message.Write(buffer, 0, read);

                    Console.WriteLine("Buffer read size » " + read);
                    if (read != 0) {
                        Console.WriteLine("Buffer read size is 0, parsing packet.");
                        this._OnMessageReceived(client);
                    }
                }
            }

            catch (Exception e) {
                client.Disconnect(null);
                Console.WriteLine(e);
                context = null;
            }

            finally {
                if (context != null) {
                    Console.WriteLine("Read complete");
                    context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, this._OnClientRead, context);
                }
            }
        }

        private void _OnClientAccepted (IAsyncResult asyncResult) {
            Console.WriteLine("Accepting new client in async way");
            try {
                ClientContext context = new ClientContext();
                context.Client = this._listener.EndAcceptTcpClient(asyncResult);
                context.Stream = context.Client.GetStream();
                context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, this._OnClientRead, context);
                Console.WriteLine("Streaming client socket");
            }

            finally {
                Console.WriteLine("Client accepted");
                this._listener.BeginAcceptTcpClient(this._OnClientAccepted, this._listener);
            }
        }

        private void _ListenTask () {
            this._listener = new TcpListener(
                new IPEndPoint(IPAddress.Any, this._port)
            );

            this._listener.Start();
            this._listener.BeginAcceptTcpClient(this._OnClientAccepted, this._listener);

            Console.WriteLine("Listening server, locking thread...");

            while (this._listener.Server.IsBound) {}
        }

        public bool Active {
            get {
                return this._listener.Server.IsBound;
            }
        }

        public void Listen (int port) {
            if (this._thread != null && this._thread.IsAlive) {
                this._thread.Abort();
            }

            this._port = port;
            this._thread = new Thread(new ThreadStart(this._ListenTask));
            this._thread.Start();
        }

        public void Stop () {
            this._listener.Stop();
        }
    }

}