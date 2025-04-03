using System.Collections.Generic;

public class SkillObjModifier
{
    public string Modifier;
    public object[] Params;
    public List<string> PreconditionModifier;

    public SkillObjModifier(string modifier, object[] parameters, List<string> preconditionModifier)
    {
        Modifier = modifier;
        Params = parameters;
        PreconditionModifier = preconditionModifier;
    }
}