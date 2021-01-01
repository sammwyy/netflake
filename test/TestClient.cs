using System;
using Netflake;
using Netflake.Pipelines;

public class Program {
    // Cliente
    public static void Main(string[] args) {
        NetClient client = new NetClient();
        client.NetworkHandler.AddPipeline(new EncryptionPipeline());
        // client.AddListener(new ExampleListener(client));
        client.Connect("127.0.0.1", 6502);
    }
}