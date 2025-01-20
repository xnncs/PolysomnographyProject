namespace PolysomnographyProject.Services.Implementation.Sleep;

using Abstract.Sleep;
using PolysomnographyProject.Models.Helping;
using PolysomnographyProject.Services.Abstract;

public class SleepQualityAnalyzer : ISleepQualityAnalyzer
{
    // Poor sleep if SDNN < 30ms
    private const double SDNNThreshold = 0.03;  
    
    // Stress threshold for LF/HF ratio
    private const double LFHFStressThreshold = 5.0; 
    
    // Minimum LF/HF ratio for good sleep
    private const double GoodLFHFMin = 1.5;   
    
    // Maximum LF/HF ratio for good sleep
    private const double GoodLFHFMax = 3.0;   

    
    private const double BaselineHF = 1.0; 

    public string AssessSleepQuality(SleepResultData data)
    {
        double lfHfRatio = data.HF != 0 ? data.LF / data.HF : double.PositiveInfinity;
        
        if (data.SDNN < SDNNThreshold)
        {
            return "Качество сна плохое: значение SDNN очень низкое, что указывает на недостаточное восстановление организма во время сна. Рекомендуется больше отдыхать и избегать стрессов.";
        }
        if (lfHfRatio > LFHFStressThreshold)
        {
            return "Качество сна плохое: значение LF/HF слишком высокое, что может свидетельствовать о высоком уровне стресса или недостаточной релаксации во время сна. Постарайтесь расслабиться перед сном.";
        }
        if (data.HF < BaselineHF * 0.7)
        {
            return "Качество сна плохое: низкое значение HF говорит о снижении активности парасимпатической нервной системы. Это может быть связано с усталостью или стрессом.";
        }
        if (lfHfRatio is < GoodLFHFMin or > GoodLFHFMax)
        {
            return "Качество сна среднее: отношение LF/HF находится вне оптимального диапазона. Это может означать небольшой дисбаланс между стрессом и расслаблением.";
        }
        return "Качество сна хорошее: показатели SDNN, LF и HF находятся в пределах нормы. Ваш сон способствует восстановлению организма и снижению стресса.";
    }
}