using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ActionSetAnimationParameter : Action
{
    [SerializeField] private enum ValueType { Int, Float, Boolean, Trigger, Value };

    [SerializeField]
    private Animator animator;

    [SerializeField, AnimatorParam("animator")]
    private string          animationParameter;
    [SerializeField]
    private ValueType       valueType = ValueType.Int;
    [SerializeField, ShowIf("needsInt")]
    private int             integerValue;
    [SerializeField, ShowIf("needsFloat")]
    private float           floatValue;
    [SerializeField, ShowIf("needsBoolean")]
    private bool            boolValue;
    [SerializeField, ShowIf("needsValue")]
    private ValueHandler    valueHandler;
    [SerializeField, ShowIf("needsVariable")]
    private Variable        variable;

    private bool needsInt => valueType == ValueType.Int;
    private bool needsFloat => valueType== ValueType.Float;
    private bool needsBoolean => valueType== ValueType.Boolean;
    private bool needsValue => (valueType== ValueType.Value) && (variable == null);
    private bool needsVariable => (valueType == ValueType.Value) && (valueHandler == null);


    public override void Execute()
    {
        if (!enableAction) return;
        if (!animator) return;
        if (animationParameter == "") return;
        if (!EvaluatePreconditions()) return;

        switch (valueType)
        {
            case ValueType.Int:
                animator.SetInteger(animationParameter, integerValue);
                break;
            case ValueType.Float:
                animator.SetFloat(animationParameter, floatValue);
                break;
            case ValueType.Boolean:
                animator.SetBool(animationParameter, boolValue);
                break;
            case ValueType.Trigger:
                animator.SetTrigger(animationParameter);
                break;
            case ValueType.Value:
                var v= variable;
                if (valueHandler) v = valueHandler.GetVariable();
                if (v != null)
                {
                    animator.SetFloat(animationParameter, v.currentValue);
                }
                break;
            default:
                break;
        }
    }

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        if (animator == null)
        {
            desc += "Sets the value of a animation parameter to a value (animator set set)!";
        }
        else
        {
            switch (valueType)
            {
                case ValueType.Int:
                    desc += $"Sets animation parameter {animationParameter} of object {animator.name} to {integerValue}";
                    break;
                case ValueType.Float:
                    desc += $"Sets animation parameter {animationParameter} of object {animator.name} to {floatValue}";
                    break;
                case ValueType.Boolean:
                    desc += $"Sets animation parameter {animationParameter} of object {animator.name} to {boolValue}";
                    break;
                case ValueType.Trigger:
                    desc += $"Triggers the animation parameter {animationParameter} of object {animator.name}";
                    break;
                case ValueType.Value:
                    if (valueHandler)
                    {
                        desc += $"Sets animation parameter {animationParameter} of object {animator.name} to the value of {valueHandler.name}";
                    }
                    else
                    {
                        desc += $"Sets animation parameter {animationParameter} of object {animator.name} to the value of variable {variable.name}";
                    }
                    break;
            }
        }

        return desc;
    }
}