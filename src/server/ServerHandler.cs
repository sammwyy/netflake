using System;
using System.Collections.Generic;

namespace Netflake {

    public class ServerHandler : NetworkHandler {

        private readonly List<Client> clients;

        public ServerHandler () {
            this.clients = new List<Client>();
        }

        public List<Client> Clients {
            get { 
                return this.clients;
            }
        }

        public void AddClient (Client client) {
            this.HandleClientConnection(client);
            this.clients.Add(client);
        }

        public void Broadcast (byte[] bytes) {
            foreach (Client client in this.clients) {
                client.Write(bytes);
            }
        }

        public void Broadcast (string data) {
            foreach (Client client in this.clients) {
                client.Write(data);
            }
        }
        
        public void ClearClients () {
            this.clients.Clear();
        }

        public void HandleClientDisconnection (Client client) {
            foreach (Pipeline pipeline in this.Pipelines) {
                pipeline.HandleClientDisconnection(client);
            }
        }

        public void HandleClientConnection (Client client) {
            foreach (Pipeline pipeline in this.Pipelines) {
                pipeline.HandleClientConnection(client);
            }
        }

        public object HandleIncoming (Client client, object packet) {
            foreach (Pipeline pipeline in this.Pipelines) {
                object output = pipeline.HandleIncoming(client, packet);
                if (output == null) {
                    return null;
                } else {
                    packet = output;
                }
            }

            return packet;
        }

        public object HandleOutgoing (Client client, object packet) {
            foreach (Pipeline pipeline in this.Pipelines) {
                object output = pipeline.HandleOutgoing(client, packet);
                if (output == null) {
                    return null;
                } else {
                    packet = output;
                }
            }

            return packet;
        }

        public void RemoveClient (Client client) {
            this.HandleClientDisconnection(client);
            this.clients.Remove(client);
        }
    }
}