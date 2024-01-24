namespace MedAlgo.DTOs
{
    public class AnnotationModel
    {
        public ulong IndexInSamples { get; set; } // right boundary (end) of beat
        public ArrhythmiaType ArrhythmiaType { get; set; }
    }
}
