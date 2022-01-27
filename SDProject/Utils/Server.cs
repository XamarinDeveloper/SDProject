using Grpc.Core;

namespace SDProject.Utils {
    internal partial class Server {
        private static readonly object serverLock = new object();
        private static readonly Channel channel = new Channel(Configs.ServerIPAddress, Configs.ServerPort, ChannelCredentials.Insecure);
    }
}