using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Report;
using GrpcServer.User;
using System;

namespace SDProject.Utils {
    internal partial class Server {
        internal class Report {
            public static void ReportNow(string dangerType, string address, double? latitude = null, double? longitude = null) {
                lock (serverLock) {
                    var client = new ReportService.ReportServiceClient(channel);
                    var response = client.CreateReport(new CreateReportRequest { Report = new GrpcServer.Report.Report {UserId = Database.UserId, Subject = dangerType, Address = address, Lat = latitude, Long = longitude }, CurrentPassword = Database.User.Password }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    Database.UserStatus = UserStatus.Red;
                }
            }
            public static void ReportForFuture(string dangerType, string address, Timestamp until) {
                lock (serverLock) {
                    var client = new ReportService.ReportServiceClient(channel);
                    var response = client.CreateReport(new CreateReportRequest { Report = new GrpcServer.Report.Report { UserId = Database.UserId, Subject = dangerType, Address = address, Lat = null, Long = null, Until = until }, CurrentPassword = Database.User.Password }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    Database.UserStatus = UserStatus.Yellow;
                }
            }
        }
    }
}