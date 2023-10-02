using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using org.bidib.Net.Core.Models.Common;

namespace org.bidib.Net.Core.Models.Helpers;

/// <summary>
/// Extension methods for value calculation objects
/// </summary>
public static class ValueCalculationExtension
{
    public static double GetCalculatedValue(this ValueCalculation calc, int value)
    {
        if (calc?.Items == null || calc.Items.Length == 0)
        {
            return value;
        }

        return GetValueForItems(calc.Items, value);
    }

    private static double GetValueForItems(IEnumerable<ValueCalculationItem> items, int value)
    {
        double newValue = 0;
        var lastOperator = string.Empty;
        foreach (var item in items)
        {
            switch (item.Type)
            {
                case ValueCalculationItemType.Self:
                    newValue = value;
                    break;
                case ValueCalculationItemType.Operator:
                    lastOperator = item.Value;
                    break;
                case ValueCalculationItemType.Constant:
                    var constValue = double.Parse(item.Value, CultureInfo.InvariantCulture);

                    if (string.IsNullOrEmpty(lastOperator))
                    {
                        newValue = constValue;
                        break;
                    }

                    newValue = OperateValue(lastOperator, newValue, constValue);

                    break;
                case ValueCalculationItemType.CvValue:
                    break;
                case ValueCalculationItemType.Bracket:
                    var bracketValue = GetValueForItems(item.Items, value);
                    newValue = string.IsNullOrEmpty(lastOperator) ? bracketValue : OperateValue(lastOperator, newValue, bracketValue);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        return newValue;
    }

    private static double OperateValue(string lastOperator, double newValue, double constValue)
    {
        var operatedValue = newValue;
        switch (lastOperator)
        {
            case "+":
            {
                operatedValue += constValue;
                break;
            }

            case "-":
            {
                operatedValue -= constValue;
                break;
            }

            case "/":
            {
                operatedValue /= constValue;
                break;
            }

            case "*":
            {
                operatedValue *= constValue;
                break;
            }

            default:
            {
                operatedValue = constValue;
                break;
            }
        }

        return operatedValue;
    }
}