using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Counsell;
using SDProject.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDProject.Utils {
    internal partial class Server {
        internal class Counseller {
            //private static readonly List<CounsellerItem> _centers = new List<CounsellerItem> {
            //    new CounsellerItem() {
            //        Id = 0,
            //        Name = "مجتبی مجتبی",
            //        PhoneNumber = "091-033-766-14",
            //        Description = "خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون"
            //    }
            //};
            //private static readonly List<ScheduleItem> _schedules = new List<ScheduleItem> {
            //    new ScheduleItem {
            //        CounsellerId = 0,
            //        UserId = 0,
            //        Start = JDateTime.Now.Date.AddHours(8),
            //        End = JDateTime.Now.Date.AddHours(9),
            //    },
            //    new ScheduleItem {
            //        CounsellerId = 0,
            //        UserId = 0,
            //        Start = JDateTime.Now.Date.AddHours(9),
            //        End = JDateTime.Now.Date.AddHours(10),
            //    },
            //    new ScheduleItem {
            //        CounsellerId = 0,
            //        UserId = Database.UserId,
            //        Start = JDateTime.Now.Date.AddHours(10),
            //        End = JDateTime.Now.Date.AddHours(11),
            //    },
            //    new ScheduleItem {
            //        CounsellerId = 0,
            //        UserId = 53,
            //        Start = JDateTime.Now.Date.AddHours(11),
            //        End = JDateTime.Now.Date.AddHours(12),
            //    }
            //};
            public static IList<CounsellerItem> GetCounsellers() {
                lock (serverLock) {
                    var client = new ConsultantService.ConsultantServiceClient(channel);
                    var response = client.FindAllConsultants(new FindAllConsultantsRequest(), new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Consultants.Select(item => (CounsellerItem)item).ToList();
                    //return _centers.ToArray().ToList();
                }
            }
            public static CounsellerItem GetCounseller(int id) {
                lock (serverLock) {
                    var client = new ConsultantService.ConsultantServiceClient(channel);
                    var response = client.FindConsultant(new FindConsultantRequest { ConsultantId = id }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Consultant;
                    //return _centers.First(post => post.Id == id);
                }
            }
            public static IList<ScheduleItem> GetSchedules(int consultantId, Timestamp start) {
                lock (serverLock) {
                    var client = new ScheduleService.ScheduleServiceClient(channel);
                    var response = client.FindAllSchedules(new FindAllSchedulesRequest { ConsultantId = consultantId, Start = start }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Schedules.Select(item => (ScheduleItem)item).ToList();
                    //return _schedules.Select(item => { item.Start += (JDateTime)start - JDateTime.Now.Date; item.End += (JDateTime)start - JDateTime.Now.Date; return item; }).ToArray().ToList();
                }
            }

            public static void Create(int consultantId, Timestamp start, Timestamp end, int each) {
                lock (serverLock) {
                    var client = new ScheduleService.ScheduleServiceClient(channel);
                    var response = client.Create(new CreateRequest { ConsultantId = consultantId, Each = each, Period = new Period {Start = start, End = end } }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                }
            }
            public static void Reserve(int consultantId, Timestamp start) {
                var client = new ReserveService.ReserveServiceClient(channel);
                client.Reserve(new ReserveRequest { ConsultantId = consultantId, Start = start, UserId = Database.UserId, CurrentPassword = Database.User.Password }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
            }
        }
    }
}