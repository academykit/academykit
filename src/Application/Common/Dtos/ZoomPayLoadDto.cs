namespace Lingtren.Application.Common.Dtos
{
    public class ZoomPayLoadDto
    {
        public string Event { get; set; }
        public PayLoad Payload { get; set; }
    }

    public class PayLoad
    {
        public string Account_Id { get; set; }
        public PayloadObject Object { get; set; }
        public string PlainToken { get; set; }
    }

    public class PayloadObject
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public string Start_Time { get; set; }
        public int Duration { get; set; }
        public string Timezone { get; set; }
        public Participant Participant { get; set; }
    }

    public class Participant
    {
        public string User_Name { get; set; }
        public string Join_Time { get; set; }
        public string Leave_Time { get; set; }
        public string Email { get; set; }
        public string Customer_Key { get; set; }
    }
}
