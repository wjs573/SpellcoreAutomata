public delegate void OnUpgradeDelegate(RealmType upgradedRealm, StageType upgradedStage);

public class Realm
{
    public RealmType realmType;
    public StageType currentStage;
    public OnUpgradeDelegate onUpgradeDelegate;

    public string GetLevel()
    {
        return string.Concat(realmType.ToCustomString(),"·", currentStage.ToCustomString());
    }

    public Realm(RealmType realmType, StageType initialStage, OnUpgradeDelegate onUpgradeDelegate = null)
    {
        this.realmType = realmType;
        this.currentStage = initialStage;
        this.onUpgradeDelegate = onUpgradeDelegate;
    }

    public void Upgrade()
    {
        if (currentStage == StageType.DaCheng)
        {
            if (checkRealmUpgradeDelegate())
            {
                currentStage = StageType.ChuQi;
                onUpgradeDelegate(realmType, currentStage);
            }
        }
        else{ 
            int currentStageIndex = (int)currentStage;
            if (currentStageIndex < 3)
            { 
                currentStage = (StageType)(currentStageIndex + 1);
                onUpgradeDelegate(realmType, currentStage);
            }
        }
    }

    public bool checkRealmUpgradeDelegate()
    { 
        return true;
    }
}
