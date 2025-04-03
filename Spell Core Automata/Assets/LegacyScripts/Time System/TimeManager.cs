using System.Collections.Generic;
using JinShan;
public class TimeManager:MonoSingleton<TimeManager>
{
    private int elapsedHourCount; // 已过时辰数

    public  enum SolarTerms
    {
        立春, 雨水, 惊蛰, 春分, 清明, 谷雨,
        立夏, 小满, 芒种, 夏至, 小暑, 大暑,
        立秋, 处暑, 白露, 秋分, 寒露, 霜降,
        立冬, 小雪, 大雪, 冬至, 小寒, 大寒
    }

    public  struct Date
    {
        public int Month;
        public int Day;

        public Date(int month, int day)
        {
            Month = month;
            Day = day;
        }

        public override bool Equals(object obj)
        {
            if (obj is Date otherDate)
            {
                return this.Month == otherDate.Month && this.Day == otherDate.Day;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Month * 31 + Day;
        }
    }

    private  Dictionary<Date, SolarTerms> solarTermsDict = new Dictionary<Date, SolarTerms>
    {
        { new Date(2, 4), SolarTerms.立春 },
        { new Date(2, 19), SolarTerms.雨水 },
        { new Date(3, 6), SolarTerms.惊蛰 },
        { new Date(3, 21), SolarTerms.春分 },
        { new Date(4, 5), SolarTerms.清明 },
        { new Date(4, 20), SolarTerms.谷雨 },
        { new Date(5, 6), SolarTerms.立夏 },
        { new Date(5, 21), SolarTerms.小满 },
        { new Date(6, 6), SolarTerms.芒种 },
        { new Date(6, 22), SolarTerms.夏至 },
        { new Date(7, 7), SolarTerms.小暑 },
        { new Date(7, 23), SolarTerms.大暑 },
        { new Date(8, 8), SolarTerms.立秋 },
        { new Date(8, 23), SolarTerms.处暑 },
        { new Date(9, 8), SolarTerms.白露 },
        { new Date(9, 23), SolarTerms.秋分 },
        { new Date(10, 8), SolarTerms.寒露 },
        { new Date(10, 24), SolarTerms.霜降 },
        { new Date(11, 8), SolarTerms.立冬 },
        { new Date(11, 22), SolarTerms.小雪 },
        { new Date(12, 7), SolarTerms.大雪 },
        { new Date(12, 22), SolarTerms.冬至 },
        { new Date(1, 6), SolarTerms.小寒 },
        { new Date(1, 20), SolarTerms.大寒 }
    };

    private TimeManager()
    {
        // 初始化代码
    }

    public void TimeElapsed(int years, int months, int days, int hours)
    {
        // 增加时间
        elapsedHourCount += (years * 12 * 365 + months * 365 + days * 12 + hours);
    }

    public Date GetCurrentDate()
    {
        int totalDays = elapsedHourCount / 12;
        int month = (totalDays / 365) % 12 + 1;  // modulo to wrap around months
        int day = (totalDays % 365) + 1;  // modulo to wrap around days
        
        return new Date(month, day);
    }

    public SolarTerms GetCurrentSolarTerm()
    {
        Date currentDate = GetCurrentDate();
        
        if (solarTermsDict.TryGetValue(currentDate, out SolarTerms currentTerm))
        {
            return currentTerm;
        }

        return SolarTerms.立春; // 返回一个默认值或根据需要进行适当的处理
    }
}
