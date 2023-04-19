using System.Linq;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

// this custom behavior allows an aura type power to start on rage and get terminated on rage stop
// it also enforce the condition to any other aura participant as soon as the barb enters rage
// finally it forces the condition to stop on barb turn start for any hero who had it but not in range anymore
public class CustomRagingAura :
    INotifyConditionRemoval, IOnAfterActionFeature, ICharacterTurnStartListener
{
    private readonly ConditionDefinition _conditionDefinition;
    private readonly FeatureDefinitionPower _powerDefinition;
    private readonly bool _friendlyAura;

    public CustomRagingAura(
        FeatureDefinitionPower powerDefinition,
        ConditionDefinition conditionDefinition,
        bool friendlyAura)
    {
        _powerDefinition = powerDefinition;
        _conditionDefinition = conditionDefinition;
        _friendlyAura = friendlyAura;
    }

    public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
    {
        var battle = Gui.Battle;

        if (battle == null)
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (_friendlyAura)
        {

            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.Side == locationCharacter.Side &&
                             x != locationCharacter &&
                             !gameLocationBattleService.IsWithinXCells(locationCharacter, x,
                                 _powerDefinition.EffectDescription.targetParameter)))
            {
                var targetRulesetCharacter = targetLocationCharacter.RulesetCharacter;
                var rulesetCondition =
                    targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == _conditionDefinition && x.SourceGuid == locationCharacter.Guid);

                if (rulesetCondition != null)
                {
                    targetRulesetCharacter.RemoveCondition(rulesetCondition);
                }
            }
        }
        else
        {
            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.Side != locationCharacter.Side &&
                             (!gameLocationBattleService.IsWithinXCells(locationCharacter, x, _powerDefinition.EffectDescription.targetParameter) ||
                             !locationCharacter.RulesetCharacter.HasConditionOfType("ConditionRaging"))))
            {
                var targetRulesetCharacter = targetLocationCharacter.RulesetCharacter;
                var rulesetCondition =
                    targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == _conditionDefinition && x.SourceGuid == locationCharacter.Guid);

                if (rulesetCondition != null)
                {
                    targetRulesetCharacter.RemoveCondition(rulesetCondition);
                }
            }
        }

        if(!locationCharacter.RulesetCharacter.HasConditionOfType("ConditionRaging"))
        {
            RemoveCondition(locationCharacter.RulesetActor);
        }

    }

    public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
    {
            RemoveCondition(removedFrom); 
    }

    public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        RemoveCondition(rulesetActor);
    }

    public void OnAfterAction(CharacterAction action)
    {
        if (action is CharacterActionSpendPower characterActionSpendPowerFriendly &&
            characterActionSpendPowerFriendly.activePower.PowerDefinition == _powerDefinition &&
            _friendlyAura == true)
        {
            AddCondition(action.ActingCharacter);
        }

        if (action is CharacterActionSpendPower characterActionSpendPowerUnfriendly &&
            characterActionSpendPowerUnfriendly.activePower.PowerDefinition == _powerDefinition &&
            _friendlyAura == false)
        {
            action.ActingCharacter.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect, _conditionDefinition.Name);
        }
    }

    private void RemoveCondition(ISerializable rulesetActor)
    {
        if (rulesetActor is not RulesetCharacter sourceRulesetCharacter)
        {
            return;
        }

        var rulesetEffectPower =
            sourceRulesetCharacter.PowersUsedByMe.FirstOrDefault(x => x.PowerDefinition == _powerDefinition);

        if (rulesetEffectPower == null)
        {
            return;
        }

        sourceRulesetCharacter.TerminatePower(rulesetEffectPower);

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        if (gameLocationCharacterService == null)
        {
            return;
        }

        if (_friendlyAura)
        {
            foreach (var targetRulesetCharacter in gameLocationCharacterService.AllValidEntities
                         .Select(x => x.RulesetActor)
                         .OfType<RulesetCharacter>()
                         .Where(x => x.Side == sourceRulesetCharacter.Side && x != sourceRulesetCharacter))
            {
                var rulesetCondition =
                    targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == _conditionDefinition && x.SourceGuid == sourceRulesetCharacter.Guid);

                if (rulesetCondition != null)
                {
                    targetRulesetCharacter.RemoveCondition(rulesetCondition);
                }
            }
        }
        else
        {
            foreach (var targetRulesetCharacter in gameLocationCharacterService.AllValidEntities
                        .Select(x => x.RulesetActor)
                        .OfType<RulesetCharacter>()
                        .Where(x => x.Side != sourceRulesetCharacter.Side))
            {
                var rulesetCondition =
                    targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == _conditionDefinition && x.SourceGuid == sourceRulesetCharacter.Guid);

                if (rulesetCondition != null)
                {
                    targetRulesetCharacter.RemoveCondition(rulesetCondition);
                }
            }
        }
    }

    private void AddCondition(GameLocationCharacter sourceLocationCharacter)
    {
        var battle = Gui.Battle;

        if (battle == null)
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var factionName = sourceLocationCharacter.RulesetCharacter.CurrentFaction.Name;

        if (_friendlyAura)
        {
            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.Side == sourceLocationCharacter.Side &&
                             x != sourceLocationCharacter &&
                             !x.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                             gameLocationBattleService.IsWithinXCells(sourceLocationCharacter, x, 2)))
            {
                var condition = RulesetCondition.CreateActiveCondition(
                    targetLocationCharacter.Guid,
                    _conditionDefinition,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    sourceLocationCharacter.Guid,
                    factionName);

                targetLocationCharacter.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagEffect,
                    condition);
            }
        }
        else
        {
            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.Side != sourceLocationCharacter.Side &&
                             x != sourceLocationCharacter &&
                             !x.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                             gameLocationBattleService.IsWithinXCells(sourceLocationCharacter, x, 2) &&
                             sourceLocationCharacter.RulesetCharacter.HasConditionOfCategory("ConditionRaging")))
            {
                var condition = RulesetCondition.CreateActiveCondition(
                    targetLocationCharacter.Guid,
                    _conditionDefinition,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    sourceLocationCharacter.Guid,
                    factionName);
        
                    targetLocationCharacter.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagEffect,
                        condition);
                
            }
        }
    }
}
