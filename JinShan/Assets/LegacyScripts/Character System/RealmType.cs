public enum RealmType
{
    LianQi, 
    ZhuJi,
    JieDan, 
    YuanYing, 
    HuaShen
}
public static class RealmTypeExtensions
{
    public static string ToCustomString(this RealmType realmType)
    {
        switch (realmType)
        {
            case RealmType.LianQi:
                return "炼气期";
            case RealmType.ZhuJi:
                return "筑基期";
            case RealmType.JieDan:
                return "结丹期";
            case RealmType.YuanYing:
                return "元婴";
            case RealmType.HuaShen:
                return "化神";
            default:
                return realmType.ToString();
        }
    }
}