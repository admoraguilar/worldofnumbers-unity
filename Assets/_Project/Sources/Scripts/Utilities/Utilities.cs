using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Text.RegularExpressions;


[Serializable]
public class MonitoredVariable<T> {
    public MonitoredVariable() { }
    public MonitoredVariable(T initValue) {
        this.value = initValue;
    }


    public event Action<T> OnValueSet = delegate { };
    public event Action<T> OnNewValueSet = delegate { };
    public event Action<T, T> OnChangedValue = delegate { };


    [ShowInInspector] public T Value {
        get { return value; }
        set {
            T lastValue = this.value;
            this.value = value;

            if(!Equals(lastValue, this.value)) {
                if(OnChangedValue != null) OnChangedValue(lastValue, this.value);
                if(OnNewValueSet != null) OnNewValueSet(this.value);
            }

            if(OnValueSet != null) OnValueSet(this.value);
        }
    }

    [OdinSerialize, HideInInspector] private T value;
}


public static class Utilities {
    public static int[] GenerateRandomNumbers(int targetSum, int divideCount, int maxNumbers, float minPercent, float maxPercent) {
        //List<float> numbers = new List<float>(maxNumbers);
        float[] numbers = new float[maxNumbers];

        // Generate a number of range for each number then
        // get the sum of those numbers
        float sum = 0f;
        for(int i = 0; i < divideCount; ++i) {
            numbers[i] = targetSum * (UnityEngine.Random.Range(minPercent, maxPercent) * .01f);
            sum += numbers[i];
        }

        // Make the numbers total to the target sum
        if(sum > targetSum) {
            float excess = (sum - targetSum) / divideCount;

            sum = 0f;
            for(int i = 0; i < divideCount; ++i) {
                numbers[i] -= excess;
                sum += numbers[i];
            }
        } else if(sum < targetSum) {
            float lacking = (targetSum - sum) / divideCount;

            sum = 0f;
            for(int i = 0; i < divideCount; ++i) {
                numbers[i] += lacking;
                sum += numbers[i];
            }
        }

        // Get the decimal part of the numbers then get its sum
        float totalDecimalValue = 0f;
        for(int i = 0; i < divideCount; ++i) {
            totalDecimalValue += numbers[i] - (int)numbers[i];
        }

        // Add the total decimal value to a random place
        int r = UnityEngine.Random.Range(0, divideCount);
        numbers[r] += Mathf.RoundToInt(totalDecimalValue);

        // Fill the rest of numbers
        int fillNumbersToAdd = maxNumbers - divideCount;
        for(int i = 0; i < fillNumbersToAdd; ++i) {
            numbers[divideCount + i] = UnityEngine.Random.Range(0, (int)(targetSum * .5f));
        }

        // Return converted int
        return Array.ConvertAll(numbers, number => (int)number);
        //return numbers.ConvertAll(number => (int)number).ToArray();
    }
    
    public static void Shuffle<T>(this IList<T> list) {
        System.Random rng = new System.Random();

        int n = list.Count;
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static bool IsEmailValid(string email) {
        string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
          + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
          + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(email);
    }
}
