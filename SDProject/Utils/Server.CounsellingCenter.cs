using Grpc.Core;
using GrpcServer.Counsell;
using SDProject.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDProject.Utils {
    internal partial class Server {
        internal class CounsellingCenter {
            //private static readonly List<CounsellingCenterItem> _centers = new List<CounsellingCenterItem> {
            //    new CounsellingCenterItem() {
            //        Id = 0,
            //        Name = "مرکز مجتبی",
            //        Landline = "091-033-766-14",
            //        Website = "modarresy.ir",
            //        Address = "خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون خونه شون"
            //    }
            //};
            public static IList<CounsellingCenterItem> GetCenters() {
                lock (serverLock) {
                    var client = new CenterService.CenterServiceClient(channel);
                    var response = client.FindAll(new FindAllRequest(), new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Centers.Select(item => (CounsellingCenterItem)item).ToList();
                    //return _centers.ToArray().ToList();
                }
            }
            public static CounsellingCenterItem GetCenter(int id) {
                lock (serverLock) {
                    var client = new CenterService.CenterServiceClient(channel);
                    var response = client.Find(new FindRequest { CenterId = id }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Center;
                    //return _centers.First(post => post.Id == id);
                }
            }
        }
    }
}