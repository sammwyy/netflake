using System.Text;

namespace Netflake {
    public static class ByteUtils {
        public static string ToString (byte[] buffer) {
            return Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty);
        }
        public static byte[] ToBuffer (string str) {
            return Encoding.UTF8.GetBytes(str);
        }
    }

}