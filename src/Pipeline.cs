using System;

namespace Netflake {

    public class Pipeline {
        public virtual object HandleIncoming (Client client, object packet) {
            return packet;
        }

        public virtual object HandleIncoming (NetClient client, object packet) {
            return packet;
        }

        public virtual object HandleOutgoing (Client client, object packet) {
            return packet;
        }

        public virtual object HandleOutgoing (NetClient client, object packet) {
            return packet;
        }

        public virtual void HandleClientConnection (Client client) {
            return;
        }

        public virtual void HandleClientDisconnection (Client client) {
            return;
        }
    }

}