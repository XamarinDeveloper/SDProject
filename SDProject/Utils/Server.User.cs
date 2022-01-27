using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.User;
using SDProject.Types;
using System;

namespace SDProject.Utils {
    internal partial class Server {
        internal class User {
            public static void Login(string phoneNumber, string password) {
                lock (serverLock) {
                    var client = new UserService.UserServiceClient(channel);
                    var response = client.Login(new LoginRequest { Phone = phoneNumber, Password = password }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    var utcOffset = TimeZoneInfo.Local.GetUtcOffset((JDateTime)response.User.Birth);
                    response.User.Birth = (JDateTime)response.User.Birth - utcOffset;
                    response.User.Password = password;
                    Database.User = response.User;
                }
            }
            public static void Register(string phoneNumber, string firstName, string lastName, string nationalId, Timestamp birthday, string password) {
                lock (serverLock) {
                    var utcOffset = TimeZoneInfo.Local.GetUtcOffset((JDateTime)birthday);
                    birthday = (JDateTime)birthday + utcOffset;
                    var client = new UserService.UserServiceClient(channel);
                    var response = client.Register(new RegisterRequest { Phone = phoneNumber, Name = firstName, Family = lastName, NationalCode = nationalId, Birth = birthday, Password = password }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    response.User.Birth = (JDateTime)response.User.Birth - utcOffset;
                    response.User.Password = password;
                    Database.User = response.User;
                }
            }
            public static void Edit(string firstName = null, string lastName = null, string nationalId = null, Timestamp birthday = null, string password = null) {
                lock (serverLock) {
                    var currentPassword = Database.User.Password;
                    var user = new GrpcServer.User.User() {
                        Id = Database.User.Id,
                        Phone = Database.User.PhoneNumber,
                        Name = firstName ?? Database.User.FirstName,
                        Family = lastName ?? Database.User.LastName,
                        Birth = birthday ?? Database.User.Birthday,
                        NationalCode = nationalId ?? Database.User.NationalId,
                        Password = password ?? currentPassword
                    };
                    var utcOffset = TimeZoneInfo.Local.GetUtcOffset((JDateTime)user.Birth);
                    user.Birth = (JDateTime)user.Birth + utcOffset;
                    var client = new UserService.UserServiceClient(channel);
                    var response = client.EditUser(new EditUserRequest { CurrentPassword = currentPassword, User = user }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    response.User.Birth = (JDateTime)response.User.Birth - utcOffset;
                    response.User.Password = password ?? currentPassword;
                    Database.User = response.User;
                }
            }
            public static void Update() {
                lock (serverLock) {
                    var password = Database.User.Password;
                    var user = Get(Database.UserId);
                    user.Password = password;
                    Database.User = user;
                }
            }
            public static Types.User Get(int id) {
                lock (serverLock) {
                    var client = new UserService.UserServiceClient(channel);
                    var response = client.FindUser(new FindUserRequest { Id = id }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    var utcOffset = TimeZoneInfo.Local.GetUtcOffset((JDateTime)response.User.Birth);
                    response.User.Birth = (JDateTime)response.User.Birth - utcOffset;
                    return response.User;
                }
            }
            public static void MakeSafe() {
                lock (serverLock) {
                    var password = Database.User.Password;
                    var client = new UserService.UserServiceClient(channel);
                    var response = client.SwapStatus(new SwapStatusRequest { UserId = Database.UserId, CurrentPassword = password }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    var utcOffset = TimeZoneInfo.Local.GetUtcOffset((JDateTime)response.User.Birth);
                    response.User.Birth = (JDateTime)response.User.Birth - utcOffset;
                    response.User.Password = password;
                    Database.User = response.User;
                }
            }
        }
    }
}