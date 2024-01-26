using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MedAlgo.DTOs;
using System.Linq;

namespace MedAlgo
{
    public static class CriticalEpisodesCalculation
    {
        [FunctionName("CriticalEpisodesCalulation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            const ulong samplesPerMinute = 18000;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var annotationList = JsonConvert.DeserializeObject<List<AnnotationModel>>(requestBody);

            var beats = CalculateBeats(samplesPerMinute, annotationList);
            var groupedEpisodes = GroupEpisodesByArrhythmiaType(beats);
            var resultList = new List<ResultModel>();

            foreach (var episode in groupedEpisodes)
            {
                var firstItem = episode.First();
                var lastItem = episode.Last();
                var arrhythmiaType = firstItem.Annotation.ArrhythmiaType; 
                var averageHR = episode.Average(item => (double)item.Hr);

                if (IsCriticalEpisode(averageHR, arrhythmiaType, episode.Count))
                {
                    var result = new ResultModel
                    {
                        StartIndexInSample = firstItem.Annotation.IndexInSamples,
                        EndIndexInSample = lastItem.Annotation.IndexInSamples
                    };
                    resultList.Add(result);
                }
            }

            return new OkObjectResult(resultList);
        }

        internal static List<List<Beat>> GroupEpisodesByArrhythmiaType(List<Beat> beats)
        {
            var groupedEpisodes = new List<List<Beat>>();
            var currentEpisode = new List<Beat>();

            foreach (var beat in beats)
            {
                if (!currentEpisode.Any() || currentEpisode.Last().Annotation.ArrhythmiaType == beat.Annotation.ArrhythmiaType)
                {
                    currentEpisode.Add(beat);
                }
                else
                {
                    groupedEpisodes.Add(currentEpisode.ToList());
                    currentEpisode.Clear();
                    currentEpisode.Add(beat);
                }
            }

            if (currentEpisode.Any())
            {
                groupedEpisodes.Add(currentEpisode.ToList());
            }

            return groupedEpisodes;
        }

        internal static bool IsCriticalEpisode(double averageHr, ArrhythmiaType arrhythmiaType, int beatCount)
        {
            switch (arrhythmiaType)
            {
                case ArrhythmiaType.AST:
                case ArrhythmiaType.AF when averageHr >= 200 && beatCount >= 10:
                case ArrhythmiaType.VT when averageHr >= 100 && beatCount >= 5:
                case ArrhythmiaType.SVT when averageHr >= 200 && beatCount >= 5:
                    return true;
                default:
                    return false;
            }
        }

        internal static List<Beat> CalculateBeats(ulong samplesPerMinute, List<AnnotationModel> annotationList)
        {
            var beats = new List<Beat>(annotationList.Count);
            for (int i = 1; i < annotationList.Count; i++)
            {
                var beat = new Beat
                {
                    Annotation = annotationList[i],
                    Hr = samplesPerMinute / (annotationList[i].IndexInSamples - annotationList[i - 1].IndexInSamples)
                };
                beats.Add(beat);
            }

            return beats;
        }

        internal class Beat
        {
            public AnnotationModel Annotation { get; init; }
            public ulong Hr { get; init; }
        }
    }
}