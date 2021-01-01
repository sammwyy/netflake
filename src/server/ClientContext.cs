using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Netflake {
    public class ClientContext {
        public TcpClient Client;
        public Stream Stream;
        public byte[] Buffer = new byte[1024];
        public MemoryStream Message = new MemoryStream();
        public String CipherPassphrase;
        public ClientState State = ClientState.WAITING;

        /* Save values for future uses */
        public Dictionary<string, string> Strings = new Dictionary<string, string>();
        public Dictionary<string, int> Integers = new Dictionary<string, int>();
        public Dictionary<string, bool> Booleans = new Dictionary<string, bool>();
    }
}