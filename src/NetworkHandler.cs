using System;
using System.Collections.Generic;

namespace Netflake {

    public class NetworkHandler {

        private readonly List<Pipeline> pipelines;

        public NetworkHandler () {
            this.pipelines = new List<Pipeline>();
        }

        public List<Pipeline> Pipelines {
            get {
                return this.pipelines;
            }
        }

        public void AddPipeline (Pipeline pipeline) {
            this.pipelines.Add(pipeline);
        }


        public void ClearPipelines () {
            this.pipelines.Clear();
        }

        public object HandleIncoming (NetClient client, object packet) {
            foreach (Pipeline pipeline in this.pipelines) {
                object output = pipeline.HandleIncoming(client, packet);
                if (output == null) {
                    return null;
                } else {
                    packet = output;
                }
            }

            return packet;
        }

        public object HandleOutgoing (NetClient client, object packet) {
            foreach (Pipeline pipeline in this.pipelines) {
                object output = pipeline.HandleOutgoing(client, packet);
                if (output == null) {
                    return null;
                } else {
                    packet = output;
                }
            }

            return packet;
        }

        public void RemovePipeline (Pipeline pipeline) {
            this.pipelines.Remove(pipeline);
        }
    }

}