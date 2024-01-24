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
    public static class CriticalEpisodesCalulation
    {
        [FunctionName("CriticalEpisodesCalulation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            const ulong samplesPerMinute = 18000;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            List<AnnotationModel> annotationList = JsonConvert.DeserializeObject<List<AnnotationModel>>(requestBody);

            var beats = CalculateBeats(samplesPerMinute, annotationList);
            var indexedList = annotationList.Select((item, index) => new IndexedAnnotationModel(item, index + 1)).ToList();

            List<List<IndexedAnnotationModel>> groupedEpisodes = GrupEpisodesByArythmiaType(indexedList);
            List<ResultModel> resultList = new List<ResultModel>();
            
            foreach (var episode in groupedEpisodes)
            {
                var firstItem = episode.First();
                var lastItem = episode.Last();
                ArrhythmiaType arrhythmiaType = ArrhythmiaType.Normal;
                bool isFirstIteration = true;

                foreach (var item in episode)
                {
                  arrhythmiaType = item.AnnotationModel.ArrhythmiaType;
                  break;
                }

                double averageIndexInSamples = episode.Average(item => (double)item.AnnotationModel.IndexInSamples);
                
                if (isCriticalEpisode(averageIndexInSamples, arrhythmiaType, episode.Count))
                {
                    var result = new ResultModel();
                    result.StartIndexInSample = firstItem.AnnotationModel.IndexInSamples;
                    result.EndIndexInSample = lastItem.AnnotationModel.IndexInSamples;
                    resultList.Add(result);
                }
            }
            return new OkObjectResult(resultList);
        }

        private static List<List<IndexedAnnotationModel>> GrupEpisodesByArythmiaType(List<IndexedAnnotationModel> indexedList)
        {
            var groupedEpisodes = new List<List<IndexedAnnotationModel>>();
            var currentEpisode = new List<IndexedAnnotationModel>();

            foreach (var annotation in indexedList)
            {
                if (!currentEpisode.Any() || currentEpisode.Last().AnnotationModel.ArrhythmiaType == annotation.AnnotationModel.ArrhythmiaType)
                {
                    currentEpisode.Add(annotation);
                }
                else
                {
                    groupedEpisodes.Add(currentEpisode.ToList());
                    currentEpisode.Clear();
                    currentEpisode.Add(annotation);
                }
            }
            if (currentEpisode.Any())
            {
                groupedEpisodes.Add(currentEpisode.ToList());
            }

            return groupedEpisodes;
        }
        private static bool isCriticalEpisode(double averageIndex, ArrhythmiaType arrhythmiaType, int beatCount)
        {

            if (arrhythmiaType == ArrhythmiaType.AST)
                return true;

            if (arrhythmiaType == ArrhythmiaType.AF && averageIndex >= 200 && beatCount >= 10)
                return true;

            if (arrhythmiaType == ArrhythmiaType.VT && averageIndex >= 100 && beatCount >= 5)
                return true;

            if (arrhythmiaType == ArrhythmiaType.SVT && averageIndex >= 200 && beatCount >= 5)
                return true;

            return false;
        }
        private static List<ulong> CalculateBeats(ulong samplesPerMinute, List<AnnotationModel> annotationList)
        {
            List<ulong> beats = new List<ulong>(annotationList.Count);
            for (int i = 1; i < annotationList.Count; i++)
            {
                var beat = samplesPerMinute / (annotationList[i].IndexInSamples - annotationList[i - 1].IndexInSamples);
                beats.Add(beat);
            }
            return beats;
        }
    }
}
