﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ExecutionEffectEvent
    {
        public ExecutionEffect ExecutionEffect;
    }

    /// <summary>
    /// 执行体效果，针对于技能执行体的效果，如播放动作、生成碰撞体、位移等这些和技能表现相关的效果
    /// </summary>
    public partial class ExecutionEffect : Entity
    {
        public Effect ExecutionEffectConfig { get; set; }
        public SkillExecution ParentExecution => GetParent<SkillExecution>();


        public override void Awake(object initData)
        {
            ExecutionEffectConfig = initData as Effect;
            Name = ExecutionEffectConfig.GetType().Name;

            //时间到直接应用能力给目标效果
            if (ExecutionEffectConfig is ApplyToTargetEffect applyEffect)
            {
                var executionApplyToTargetComponent = AddComponent<ExecutionApplyToTargetComponent>();
                executionApplyToTargetComponent.EffectApplyType = applyEffect.EffectApplyType;
                if (applyEffect.TriggerTime > 0)
                {
                    AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)applyEffect.TriggerTime;
                }
                else
                {
                    ApplyEffect();
                }
            }
            //时间到生成碰撞体，碰撞体再触发应用能力效果
            if (ExecutionEffectConfig is SpawnItemEffect spawnItemEffect)
            {
                AddComponent<ExecutionSpawnItemComponent>().SpawnItemEffect = spawnItemEffect;
                if (spawnItemEffect.ColliderSpawnData.ColliderSpawnEmitter.time > 0)
                {
                    AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)spawnItemEffect.ColliderSpawnData.ColliderSpawnEmitter.time;
                }
                else
                {
                    ApplyEffect();
                }
            }
            //时间到播放动作
            if (ExecutionEffectConfig is AnimationEffect animationEffect)
            {
                AddComponent<ExecutionAnimationComponent>().AnimationEffect = animationEffect;
                if (animationEffect.AnimationData.StartTime > 0)
                {
                    AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)animationEffect.AnimationData.StartTime;
                }
                else
                {
                    ApplyEffect();
                }
            }

            if (ExecutionEffectConfig.Decorators != null)
            {
                foreach (var effectDecorator in ExecutionEffectConfig.Decorators)
                {
                    if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                    {

                    }
                }
            }

            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }
        }

        public void ApplyEffect()
        {
            //Log.Debug($"ExecutionEffect ApplyEffect");
            //AbilityEffect.ApplyEffectToOwner();
            this.Publish(new ExecutionEffectEvent() { ExecutionEffect = this });
        }

        public void ExecuteEffectAssignWithTarget(CombatEntity targetEntity)
        {
            //AbilityEffect.ApplyEffectTo(targetEntity, this);
        }
    }
}

//AbilityEffect = initData as AbilityEffect;
//Name = AbilityEffect.Name;

//foreach (var component in AbilityEffect.Components.Values)
//{
//    //时间到直接应用能力给目标效果
//    if (component is EffectExecutionApplyToTargetComponent applyToTargetComponent)
//    {
//        var executionApplyToTargetComponent = AddComponent<ExecutionApplyToTargetComponent>();
//        executionApplyToTargetComponent.EffectApplyType = applyToTargetComponent.EffectApplyType;
//        if (applyToTargetComponent.TriggerTime > 0)
//        {
//            AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)applyToTargetComponent.TriggerTime;
//        }
//        else
//        {
//            ApplyEffect();
//        }
//    }
//    //时间到生成碰撞体，碰撞体再触发应用能力效果
//    if (component is EffectExecutionSpawnItemComponent spawnItemComponent)
//    {
//        AddComponent<ExecutionSpawnItemComponent>().EffectSpawnItemComponent = spawnItemComponent;
//        if (spawnItemComponent.ColliderSpawnData.ColliderSpawnEmitter.time > 0)
//        {
//            AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)spawnItemComponent.ColliderSpawnData.ColliderSpawnEmitter.time;
//        }
//        else
//        {
//            ApplyEffect();
//        }
//    }
//    //时间到播放动作
//    if (component is EffectExecutionAnimationComponent animationComponent)
//    {
//        AddComponent<ExecutionAnimationComponent>().EffectAnimationComponent = animationComponent;
//        if (animationComponent.AnimationData.StartTime > 0)
//        {
//            AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)animationComponent.AnimationData.StartTime;
//        }
//        else
//        {
//            ApplyEffect();
//        }
//    }
//}

//if (AbilityEffect.EffectConfig is CustomEffect customEffect)
//{
//    if (customEffect.CustomEffectType == "按命中目标数递减百分比伤害")
//    {
//        if (Parent is AbilityItem abilityItem)
//        {
//            //abilityItem.GetComponent<ExecutionEffectComponent>().DamageExecutionEffect.AddComponent<ExecutionDamageReduceWithTargetCountComponent>(AbilityEffect);
//            abilityItem.AddComponent<ExecutionDamageReduceWithTargetCountComponent>(AbilityEffect);
//        }
//    }
//}
