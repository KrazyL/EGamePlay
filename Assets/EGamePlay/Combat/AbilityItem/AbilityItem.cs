﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : Entity, IPosition
    {
        public Entity AbilityEntity => AbilityExecution.AbilityEntity;
        public IAbilityExecution AbilityExecution { get; set; }
        public ExecutionEffectComponent ItemExecutionEffectComponent { get; private set; }
        public EffectApplyType EffectApplyType { get; private set; }
        public Vector3 Position { get; set; }
        public float Direction { get; set; }
        public CombatEntity TargetEntity { get; set; }


        public override void Awake(object initData)
        {
            AbilityExecution = initData as IAbilityExecution;
            //ItemExecutionEffectComponent = AddComponent<ExecutionEffectComponent>();
            var abilityEffects = AbilityExecution.AbilityEntity.Get<AbilityEffectComponent>().AbilityEffects;
            foreach (var abilityEffect in abilityEffects)
            {
                if (abilityEffect.EffectConfig.Decorators != null)
                {
                    foreach (var effectDecorator in abilityEffect.EffectConfig.Decorators)
                    {
                        if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                        {
                            AddComponent<AbilityItemTargetCounterComponent>();
                        }
                    }
                }
            }
        }

        //结束单元体
        public void DestroyItem()
        {
            Destroy(this);
        }

        public void OnCollision(CombatEntity otherCombatEntity)
        {
            if (TargetEntity != null)
            {
                if (otherCombatEntity != TargetEntity)
                {
                    return;
                }
            }

            if (EffectApplyType == EffectApplyType.AllEffects)
            {
                AbilityEntity.Get<AbilityEffectComponent>().TryAssignAllEffectsToTargetWithAbilityItem(otherCombatEntity, this);
            }
            else
            {
                AbilityEntity.Get<AbilityEffectComponent>().TryAssignEffectByIndex(otherCombatEntity, (int)EffectApplyType - 1);
            }

            if (TryGet(out AbilityItemTargetCounterComponent targetCounterComponent))
            {
                targetCounterComponent.TargetCounter++;
            }

            if (TargetEntity != null)
            {
                DestroyItem();
            }
        }
    }
}