using System;
using Netflake;
using Netflake.Pipelines;

public class Program {
    // Servidor
    public static void Main(string[] args) {
        NetServer server = new NetServer();

        server.NetworkHandler.AddPipeline(new KeepAlivePipeline(2000));
        server.NetworkHandler.AddPipeline(new EncryptionPipeline(12));

        server.Listen(6502);
    }
}