using GrpcServer.Counsell;
using SDProject.Extensions;

namespace SDProject.Types {
    public struct CounsellerItem {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public static implicit operator CounsellerItem(Consultant consultant) {
            return new CounsellerItem {
                Id = consultant.Id,
                Name = $"{consultant.Name}{((consultant.Name.IsEmpty() || consultant.Family.IsEmpty()) ? "" : " ")}{consultant.Family}",
                PhoneNumber = consultant.Phone,
                Description = consultant.Description
            };
        }
    }
}