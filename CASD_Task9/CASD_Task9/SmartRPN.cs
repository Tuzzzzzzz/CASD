namespace RPN;
using MyVector;
using static RPN;

static internal class SmartRPN{
    public static double CalculateWithInputedVariables(string expression, Dictionary<string, double> variables = null)
    {
        MyVector<string> tokens = ParseTokens(expression);

        if (tokens.Contains("result") && (variables == null || !variables.ContainsKey("result"))) 
            throw new ArgumentException("Недопустимое значение result");

        variables = InputVariables(ref tokens);

        return CalculateRPN(ConvertToRPN(tokens), variables);
    }

    public static Dictionary<string, double> InputVariables(ref MyVector<string> tokens)
    {
        Dictionary<string, double> variables = new Dictionary<string, double>();
        foreach (string token in tokens) {
            if (Variables.Contains(token) && !variables.ContainsKey(token) && token != "result")
            {
                Console.Write($"{token} = ");
                if (double.TryParse(Console.ReadLine(), out double value))
                {
                    variables.Add(token, value);
                }
                else throw new ArgumentException("Неправильный ввод переменных");
            }
        }
        return variables;
    }

}
