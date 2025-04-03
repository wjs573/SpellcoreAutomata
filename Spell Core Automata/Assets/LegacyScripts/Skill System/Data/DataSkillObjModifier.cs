using System.Collections.Generic;

public static class DataSkillObjModifier
{
    public static Dictionary<string, SkillObjModifier> data = new Dictionary<string, SkillObjModifier>()
    {
        {"SummonSkeleton_MoreSkeleton",new SkillObjModifier(
            "AddTimelineNode",
            new object[]{new TimelineNode(0.10f, "SummonAICharacter",
                new string[]{},new object[] {
                        "Skeleton",
                        new ChaProperty(100, 0, 100, 1000, 2, 100,0,
                        50,20,10,0,
                        1.5f,0.1f,0,0.5f,0.5f,MoveType.ground),
                        0f, "Skeleton",
                        new string[] { "Skeleton" },
                        new AddBuffInfo[] {
                        },new string[]{"SkeletonSlam"},"AISkeleton"
                    })},
                new List<string>(){ })
        }
    };
}