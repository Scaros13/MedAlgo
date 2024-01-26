using MedAlgo.DTOs;
using static MedAlgo.CriticalEpisodesCalculation;


namespace UnitTests
{
    public class CalculateBeats
    {
        [Fact]
        public void CalculateBeats_ShouldCalculateBeatsCorrectly()
        {
            // Arrange
            ulong samplesPerMinute = 18000;
            var annotationList = new List<AnnotationModel>
        {
            new AnnotationModel { IndexInSamples = 0 },
            new AnnotationModel { IndexInSamples = 500 },
            new AnnotationModel { IndexInSamples = 1000 },
            new AnnotationModel { IndexInSamples = 1500 }
        };

            // Act
            var beats = CalculateBeats(samplesPerMinute, annotationList);

            // Assert
            Assert.Equal(3, beats.Count);

            Assert.Equal(36.0, beats[0].Hr, 2); 
            Assert.Equal(36.0, beats[1].Hr, 2); 
            Assert.Equal(36.0, beats[2].Hr, 2); 
        }

        [Fact]
        public void CalculateBeats_ShouldHandleEmptyList()
        {
            // Arrange
            ulong samplesPerMinute = 18000;
            var annotationList = new List<AnnotationModel>();

            // Act
            var beats = CalculateBeats(samplesPerMinute, annotationList);

            // Assert
            Assert.Empty(beats);
        }


    }
}