namespace Lingtren.Application.Common.Dtos
{
    public class ZoomRecordPayloadDto
    {
        public string Event { get; set; }
        public long Event_ts { get; set; }
        public Payload Payload { get; set; }
        public string Download_token { get; set; }
    }

    public class Payload
    {
        public string Account_id { get; set; }
        public Object Object { get; set; }
        public string PlainToken { get; set; }
    }

    public class Object
    {
        public long Id { get; set; }
        public int Duration { get; set; }
        public int Recording_count { get; set; }
        public IList<RecordingFiles> Recording_files { get; set; }
    }

    public class RecordingFiles
    {
        public string Id { get; set; }
        public string Meeting_id { get; set; }
        public string Recording_start { get; set; }
        public string Recording_end { get; set; }
        public string File_type { get; set; }
        public string File_name { get; set; }
        public int File_size { get; set; }
        public string File_extension { get; set; }
        public string Play_Url { get; set; }
        public string Download_url { get; set; }
        public string Status { get; set; }
        public string Recording_type { get; set; }
    }
}
