namespace BadCode;

public class Calculator
{
    public double DoCalculation(double a, double b, string op)
    {
        if (op == "add")
        {
            return a + b;
        }
        else if (op == "subtract")
        {
            return a - b;
        }
        else if (op == "multiply")
        {
            return a * b;
        }
        else if (op == "divide")
        {
            return a / b;
        }
        else
        {
            return -9999;
        }
    }

    public double ProcessAndCalculateAverage(int[] numbers)
    {
        double sum = 0;
        
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] < 0)
            {
                numbers[i] = Math.Abs(numbers[i]);
            }
            
            if (numbers[i] % 2 == 0)
            {
                sum += numbers[i];
            }
        }
        
        return sum / numbers.Length;
    }

    public static double ApplyTax(double amount)
    {
        const double taxRate = 0.2;
        return amount + (amount * taxRate);
    }
}
