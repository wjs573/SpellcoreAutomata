public enum StageType
{
    ChuQi, //初期
    ZhongQi, //中期
    HouQi, //后期
    DaCheng //大成
}
public static class StageTypeExtensions
{
    public static string ToCustomString(this StageType stageType)
    {
        switch (stageType)
        {
            case StageType.ChuQi:
                return "初期";
            case StageType.ZhongQi:
                return "中期";
            case StageType.HouQi:
                return "后期";
            case StageType.DaCheng:
                return "大成";
            default:
                return stageType.ToString();
        }
    }
}