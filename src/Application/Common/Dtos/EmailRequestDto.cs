namespace Lingtren.Application.Common.Dtos
{
    public class EmailRequestDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public IEnumerable<EmailAttachmentDto> Attachments { get; set; } =
            new List<EmailAttachmentDto>();
    }
}
