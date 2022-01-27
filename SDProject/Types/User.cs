using GrpcServer.User;

namespace SDProject.Types {
    public struct User {
        public int Id;
        public string PhoneNumber;
        public string FirstName;
        public string LastName;
        public string NationalId;
        public string Password;
        public JDateTime Birthday;
        public UserStatus Status;

        public static implicit operator User(GrpcServer.User.User user) {
            return new User {
                Id = user.Id,
                FirstName = user.Name,
                LastName = user.Family,
                Password = user.Password,
                PhoneNumber = user.Phone,
                Birthday = user.Birth,
                NationalId = user.NationalCode,
                Status = user.UserStatus
            };
        }
    }
}