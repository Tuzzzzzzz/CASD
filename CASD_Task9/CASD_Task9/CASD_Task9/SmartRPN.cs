namespace RPN;
using MyVector;
using static RPN;

static internal class SmartRPN{
    public static double CalculateWithInputedVariables(string expression, Dictionary<string, double> variables = null)
    {
        MyVector<string> tokens = ParseTokens(expression);

        if (tokens.Contains("result") && (variables == null || !variables.ContainsKey("result"))) 
            throw new ArgumentException("Недопустимое значение result");

        InputVariables(ref tokens);

        return CalculateRPN(ConvertToRPN(tokens), variables);
    }

    public static void InputVariables(ref MyVector<string> tokens)
    {
        for (int i = 0; i < tokens.Size(); i++)
        {
            if (Variables.Contains(tokens[i]) && tokens[i] != "result")
            {
                Console.Write($"{tokens[i]} = ");
                if (double.TryParse(Console.ReadLine(), out double value))
                {
                    tokens[i] = value.ToString();
                }
                else throw new ArgumentException("Неправильный ввод переменных");
            }
        }
    }

}
