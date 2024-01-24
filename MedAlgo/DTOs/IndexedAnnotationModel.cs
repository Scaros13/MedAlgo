
namespace MedAlgo.DTOs
{
    public class IndexedAnnotationModel
    {
        public AnnotationModel AnnotationModel { get; }
        public int AnnotationIndex { get; }
        public ulong Beat { get; }

        public IndexedAnnotationModel(AnnotationModel annotationModel, int annotationIndex)
        {
            AnnotationModel = annotationModel;
            AnnotationIndex = annotationIndex;
        }
    }

}
