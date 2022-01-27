using GrpcServer.Counsell;

namespace SDProject.Types {
    public struct CounsellingCenterItem {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Landline { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }

        public static implicit operator CounsellingCenterItem(Center center) {
            return new CounsellingCenterItem {
                Id = center.Id,
                Name = center.Name,
                Landline = center.Telephone,
                Website = center.Website,
                Address = center.Address
            };
        }
    }
}