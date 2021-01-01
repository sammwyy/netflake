using Netflake;

namespace Netflake.Pipelines {
    public class KeepAlivePipeline : Pipeline {

        private int _timeout = -1;

        public KeepAlivePipeline (int timeout) {
            this._timeout = timeout;
        }

        public override void HandleClientConnection(Client client) {
            client.Socket.ReceiveTimeout = this._timeout;
            client.Socket.SendTimeout = this._timeout;
        }

        public override object HandleIncoming(Client client, object packet) {
            string data = ByteUtils.ToString((byte[]) packet);
            if (data.Equals("head_type=keepalive")) {
                return null;
            }

            return packet;
        }
    }
}