using Netflake;
using System;

namespace Netflake.Pipelines {
    public class EncryptionPipeline : Pipeline {

        private int _threshold = 10;

        public EncryptionPipeline () { }

        public EncryptionPipeline (int threshold) {
            this._threshold = threshold;
        }

        /* Client-side Pipeline */
        public override object HandleIncoming(NetClient client, object packet) {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            if (!client.Context.Strings.ContainsKey("enc_passphrase")) {
                byte[] buffer = (byte[]) packet;
                string data = ByteUtils.ToString(buffer);
                string[] lines = data.Split('\n');

                Console.WriteLine(lines[0]);

                if (lines[0].Equals("head_type=ack") && lines.Length > 0) {
                     Console.WriteLine("2222");
                    string secondLine = lines[1];
                    string[] parts = secondLine.Split('=');

                    if (parts.Length > 0) {
                        string key = parts[0];
                        string value = parts[1];

                         Console.WriteLine("333");

                        if (key.Equals("enc_passphrase")) {
                            client.Context.Strings.Add("enc_passphrase", value);
                            Console.WriteLine("Setting passphrase to " + value);
                            return null;
                        }
                    }
                } 

                return packet;
            }

            string passphrase = null;
            client.Context.Strings.TryGetValue("enc_passphrase", out passphrase);
            
            string raw = ByteUtils.ToString((byte[]) packet);
            string decrypted = CipherUtils.Decrypt(raw, passphrase);
            return ByteUtils.ToBuffer(decrypted);
        }

        public override object HandleOutgoing(NetClient client, object packet) {
            if (!client.Context.Strings.ContainsKey("enc_passphrase")) {
                return packet;
            }

            string passphrase = null;
            client.Context.Strings.TryGetValue("enc_passphrase", out passphrase);

            string data = ByteUtils.ToString((byte[]) packet);
            string encrypted = CipherUtils.Encrypt(data, passphrase);

            return ByteUtils.ToBuffer(encrypted);
        }

        /* Server-side Pipeline */
        public override void HandleClientConnection(Client client) {
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
            string passphrase = CipherUtils.GenerateRandomPassphrase(this._threshold);
            client.Write("head_type=ack\nenc_passphrase=" + passphrase);
            client.Context.Strings.Add("enc_passphrase", passphrase);
        }

        public override object HandleIncoming(Client client, object packet) {
            string passphrase = null;
            client.Context.Strings.TryGetValue("enc_passphrase", out passphrase);
            
            string data = ByteUtils.ToString((byte[]) packet);
            string decrypted = CipherUtils.Decrypt(data, passphrase);
            return ByteUtils.ToBuffer(decrypted);
        }

        public override object HandleOutgoing(Client client, object packet) {
            if (!client.Context.Strings.ContainsKey("enc_passphrase")) {
                return packet;
            }

            string passphrase = null;
            client.Context.Strings.TryGetValue("enc_passphrase", out passphrase);

            string data = ByteUtils.ToString((byte[]) packet);
            string encrypted = CipherUtils.Encrypt(data, passphrase);

            return ByteUtils.ToBuffer(encrypted);
        }
    }
}