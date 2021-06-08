namespace Platform.Domain
{
    public class StreamEvent
    {
        public StreamEvent(object evt, Metadata meta)
        {
            Event    = evt;
            Metadata = meta;
        }

        public object Event { get; }

        public Metadata Metadata { get; }
    }
}
