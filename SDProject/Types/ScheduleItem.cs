using GrpcServer.Counsell;

namespace SDProject.Types {
    public struct ScheduleItem {
        public int CounsellerId { get; set; }
        public int UserId { get; set; }
        public JDateTime Start { get; set; }
        public JDateTime End { get; set; }

        public static implicit operator ScheduleItem(Schedule schedule) {
            return new ScheduleItem {
                CounsellerId = schedule.ConsultantId,
                UserId = schedule.UserId,
                Start = schedule.Period.Start,
                End = schedule.Period.End
            };
        }
    }
}