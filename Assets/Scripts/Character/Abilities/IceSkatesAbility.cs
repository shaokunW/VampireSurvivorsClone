namespace Vampire
{
    public class IceSkatesAbility : FloatUpgradeAbility<UpgradeableMovementSpeed>
    {
        public override bool RequirementsMet()
        {
            bool baseRequirementsMet = base.RequirementsMet();
            bool movementSpeedInUse = abilityManager.MovementSpeedUpgradeablesCount > 0;
            return baseRequirementsMet && movementSpeedInUse;
        }
    }
}