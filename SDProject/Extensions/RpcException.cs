using Grpc.Core;

namespace SDProject.Extensions {
    public static class RpcExceptionExtension {
        public static string GetMessage(this RpcException e) {
            switch (e.StatusCode) {
                case StatusCode.NotFound:
                case StatusCode.AlreadyExists:
                case StatusCode.PermissionDenied:
                case StatusCode.FailedPrecondition:
                    return e.Status.Detail;
                case StatusCode.Internal:
                    return "خطای نامشخص، با پشتیبانی تماس بگیرید";
                default:
                    return "خطا در ارتباط با سرور";
            }
        }
    }
}