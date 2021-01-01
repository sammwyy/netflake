using System;

namespace Netflake {
    public class ClientTimeoutException : Exception {
        
        public ClientTimeoutException (int time) : base("Client didn't send any packet after " + time + " milliseconds") {}

    }
}