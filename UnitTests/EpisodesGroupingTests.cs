using MedAlgo.DTOs;
using static MedAlgo.CriticalEpisodesCalculation;


namespace UnitTests
{
    public class EpisodesGroupingTests
    {
        [Fact]
        public void GroupEpisodesByArrhythmiaType_ShouldGroupBeatsByArrhythmiaType()
        {
            // Arrange
            var beats = new List<Beat>
        {
            new Beat { Annotation = new AnnotationModel { ArrhythmiaType = ArrhythmiaType.Normal } },
            new Beat { Annotation = new AnnotationModel { ArrhythmiaType = ArrhythmiaType.AF } },
            new Beat { Annotation = new AnnotationModel { ArrhythmiaType = ArrhythmiaType.AF } },
            new Beat { Annotation = new AnnotationModel { ArrhythmiaType = ArrhythmiaType.Normal } },
            new Beat { Annotation = new AnnotationModel { ArrhythmiaType = ArrhythmiaType.AF } }
        };

            // Act
            var groupedEpisodes = GroupEpisodesByArrhythmiaType(beats);

            // Assert
            Assert.Equal(4, groupedEpisodes.Count);

            // Check groupings
            Assert.Equal(1, groupedEpisodes[0].Count); // Normal
            Assert.Equal(2, groupedEpisodes[1].Count); // AF
            Assert.Equal(1, groupedEpisodes[2].Count); // Normal
            Assert.Equal(1, groupedEpisodes[3].Count); // AF
        }

        [Fact]
        public void GroupEpisodesByArrhythmiaType_ShouldHandleEmptyList()
        {
            // Arrange
            var beats = new List<Beat>();

            // Act
            var groupedEpisodes = GroupEpisodesByArrhythmiaType(beats);

            // Assert
            Assert.Empty(groupedEpisodes);
        }

    }
}