namespace AcademyKit.Application.Common.Dtos
{
    using MimeKit;

    public class EmailAttachmentDto
    {
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public ContentType ContentType { get; set; }
    }
}
