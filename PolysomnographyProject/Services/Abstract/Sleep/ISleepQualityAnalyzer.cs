namespace PolysomnographyProject.Services.Abstract.Sleep;

using PolysomnographyProject.Models.Helping;

public interface ISleepQualityAnalyzer
{
    string AssessSleepQuality(SleepResultData data);
}