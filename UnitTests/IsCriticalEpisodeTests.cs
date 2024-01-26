using MedAlgo.DTOs;
using static MedAlgo.CriticalEpisodesCalculation;


namespace UnitTests
{
    public class IsCriticalEpisodeTests
    {
        [Theory]
        [InlineData(201, ArrhythmiaType.AST, 1, true)]
        [InlineData(199, ArrhythmiaType.AF, 9, false)]
        [InlineData(200, ArrhythmiaType.AF, 10, true)]
        [InlineData(201, ArrhythmiaType.AF, 10, true)]
        [InlineData(199, ArrhythmiaType.VT, 4, false)]
        [InlineData(200, ArrhythmiaType.VT, 5, true)]
        [InlineData(201, ArrhythmiaType.VT, 5, true)]
        [InlineData(199, ArrhythmiaType.SVT, 4, false)]
        [InlineData(200, ArrhythmiaType.SVT, 5, true)]
        [InlineData(201, ArrhythmiaType.SVT, 5, true)]
        public void IsCriticalEpisode_ShouldReturnExpectedResult(double averageHr, ArrhythmiaType arrhythmiaType, int beatCount, bool expectedResult)
        {
            // Act
            var result = IsCriticalEpisode(averageHr, arrhythmiaType, beatCount);

            // Assert
            Assert.Equal(expectedResult, result);
        }


    }
}