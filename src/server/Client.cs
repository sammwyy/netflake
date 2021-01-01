using System;
using System.Net.Sockets;

namespace Netflake {

    public class Client {

        private ClientContext context;
        private ServerHandler handler;

        public Client (ClientContext context, ServerHandler handler) {
            this.context = context;
            this.handler = handler;
        }

        public ClientContext Context {
            get {
                return this.context;
            }
        }

        public Socket Socket {
            get {
                return this.context.Client.Client;
            }
        }

        public void Disconnect (Exception exception) {
            if (exception != null) {
                Console.WriteLine(exception);
            }

            context.State = ClientState.FINISHING;

            context.Client.Close();
            context.Stream.Dispose();
            context.Message.Dispose();
            
            this.handler.RemoveClient(this);
        }

        public void Write (byte[] buffer) {
            object packet = this.handler.HandleOutgoing(this, buffer);
            if (packet != null) {
                byte[] bytes = (byte[]) packet;

                try {
                    this.context.Client.Client.Send(bytes);
                } 
                
                catch (Exception e) {
                    context.Client.Close();
                    context.Stream.Dispose();
                    context.Message.Dispose();
                }
            }
        }

        public void Write (string data) {
            byte[] buffer = ByteUtils.ToBuffer(data);
            this.Write(buffer);
        }

        /*
        public void Write (Packet packet) {
            string data = packet.ToString();
            this.Write(data);
        }
        */
    }

}