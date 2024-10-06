namespace RPN;
using MyStack;
using MyVector;

internal static class RPN
{
    private static HashSet<string> leftAssociativeInfixOperations = new() {
        "+", "-", "*", "/"
    };

    private static HashSet<string> rightAssociativeInfixOperations = new() {
        "^", "%", "//"
    };

    private static HashSet<string> unaryPrefixOperations = new() {
        "sqrt", "abs", "sign", "sin", "cos", "tg", "int", "~", "ln", "lg", "exp"
    };

    private static HashSet<string> bynaryPrefixOperations = new() {
        "log", "min", "max"
    };

    private static Dictionary<string, double> mathConsts = new() {
        { "pi", Math.PI }, {"e",  Math.E }
    };

    private static HashSet<string> variables = new() {
        "a", "b", "c", "d", "f", "g", "h", "i", "j", "k", "l", "m", //without "e"
        "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
        "result"
    };

    public static HashSet<string> LeftAssociativeInfixOperations { get => leftAssociativeInfixOperations; }
    public static HashSet<string> RightAssociativeInfixOperations { get => rightAssociativeInfixOperations; }
    public static HashSet<string> UnaryPrefixOperations { get => unaryPrefixOperations; }
    public static HashSet<string> BynaryPrefixOperations { get => bynaryPrefixOperations; }
    public static Dictionary<string, double> MathConsts { get => mathConsts; }
    public static HashSet<string> Variables { get => variables; }


    public static double Calculate(string expression, Dictionary<string, double> variables = null)
    {
        MyVector<string> tokens = ParseTokens(expression);

        return CalculateRPN(ConvertToRPN(tokens), variables);
    }

    public static MyVector<string> ParseTokens(string expression)
    {
        MyVector<string> tokens = new MyVector<string>();
        expression += " ";
        string token = "";
        foreach (char c in expression)
        {
            string s = c.ToString();
            if (s == " " || s == "\t")
            {
                if (token != "")
                {
                    tokens.Add(token);
                    token = "";
                }
            }
            else if(RightAssociativeInfixOperations.Contains(s) && RightAssociativeInfixOperations.Contains(token)
                || LeftAssociativeInfixOperations.Contains(s) && LeftAssociativeInfixOperations.Contains(token)) //second condition for future
            {
                tokens.Add(token);
                token = "";
            }
            else if (LeftAssociativeInfixOperations.Contains(s) || RightAssociativeInfixOperations.Contains(s) 
                || s == "(" || s == ")" || s == ";") {
                if (token != "")
                {
                    tokens.Add(token);
                    token = "";
                }
                tokens.Add(s);
            }
            else token += s;
        }

        if (tokens.IsEmpty()) throw new ArgumentException("Нет значимых лексем");
        if (!CheckTokens(tokens)) throw new ArgumentException("Недопустимые лексемы");

        UnaryMinus(ref tokens);

        return tokens;
    }

    public static bool CheckTokens(MyVector<string> tokens)
    {
        foreach (string token in tokens)
        {
            if (!(double.TryParse(token, out _)
                || LeftAssociativeInfixOperations.Contains(token) 
                || RightAssociativeInfixOperations.Contains(token)
                || UnaryPrefixOperations.Contains(token)
                || BynaryPrefixOperations.Contains(token)
                || MathConsts.ContainsKey(token)
                || Variables.Contains(token)
                || token == "(" || token == ")" || token == ";"))
                return false;
        }
        return true;
    }

    public static void UnaryMinus(ref MyVector<string> tokens)
    {
        if (tokens.FirstItem == "-") tokens.FirstItem = "~";

        for (int i = 1; i < tokens.Size() - 1; i++)
        {
            if (tokens[i] == "(" && tokens[i + 1] == "-")
                tokens[i+1] = "~";
            else if((tokens[i] == "-" || tokens[i] == "~") && tokens[i+1] == "-")
            {
                tokens[i] = "~";
                tokens[i + 1] = "~";
            }
        }
    }


    public static int Priority(string operation) => operation switch
    {
        "//" => 3,
        "%" => 3,
        "^" => 3,
        "*" => 2,
        "/" => 2,
        "+" => 1, 
        "-" => 1,
        _ => throw new NotImplementedException("Неизвестная операция")
    };

    public static MyVector<string> ConvertToRPN(MyVector<string> tokens)
    {
        MyVector<string> RPN = new MyVector<string>();
        MyStack<string> stack = new MyStack<string>();
        foreach (string token in tokens) {
            if (double.TryParse(token, out _)
                || Variables.Contains(token)
                || MathConsts.ContainsKey(token)) RPN.Add(token);

            else if (UnaryPrefixOperations.Contains(token)
                || BynaryPrefixOperations.Contains(token)) stack.Push(token);//right-associative operations

            else if (token == ";")
            {
                while (!stack.IsEmpty() && stack.Peek() != "(")
                {
                    RPN.Add(stack.Pop());
                }
                if (stack.Peek() != "(")
                    throw new ArgumentException("Пропущена открывающая скобка");
            }

            else if (LeftAssociativeInfixOperations.Contains(token) || RightAssociativeInfixOperations.Contains(token))
            {
                while (!stack.IsEmpty() && stack.Peek() != "(" && (Priority(stack.Peek()) > Priority(token)
                    || Priority(stack.Peek()) == Priority(token) && LeftAssociativeInfixOperations.Contains(token)))
                {
                    RPN.Add(stack.Pop());
                }
                stack.Push(token);
            }

            else if (token == "(") stack.Push(token);

            else if (token == ")")
            {
                while (!stack.IsEmpty() && stack.Peek() != "(")
                {
                    RPN.Add(stack.Pop());
                }
                if (stack.Peek() != "(")
                    throw new ArgumentException("Пропущена открывающая скобка");
                stack.Pop();
                if (UnaryPrefixOperations.Contains(stack.Peek()) || BynaryPrefixOperations.Contains(stack.Peek()))
                    RPN.Add(stack.Pop());
            }
        }
        while (!stack.IsEmpty()) {
            if (stack.Peek() == "(") throw new ArgumentException("Пропущена закрывающая скобка");
            RPN.Add(stack.Pop());
        }
        return RPN;
    }

    public static double EvaluateFunction(string operation, params double[] args) => operation switch
    {
        "+" => args[0] + args[1],
        "-" => args[0] - args[1],
        "*" => args[0] * args[1],
        "/" => args[1] != 0 ? args[0]/args[1] : throw new DivideByZeroException("Деление на ноль"),
        "^" => Math.Pow(args[0], args[1]),
        "%" => args[0] % args[1],
        "//" => (int)args[0] / (int)args[1],
        "sqrt" => Math.Sqrt(args[0]),
        "abs" => Math.Abs(args[0]),
        "sign" => Math.Sign(args[0]),
        "sin" => Math.Sin(args[0]),
        "cos" => Math.Cos(args[0]),
        "tg" => Math.Tan(args[0]),
        "int" => (int)args[0],
        "~" => -args[0],
        "ln" => Math.Log(args[0]),
        "lg" => Math.Log10(args[0]),
        "log" => Math.Log(args[0], args[1]),
        "min" => Math.Min(args[0], args[1]),
        "max" => Math.Max(args[0], args[1]),
        "exp" => Math.Exp(args[0]),
        _ => throw new NotImplementedException("Неизвестная функция")
    };

    public static double CalculateRPN(MyVector<string> RPN, Dictionary<string, double> variables = null)
    {
        if (variables != null) {
            for (int i = 0; i < RPN.Size(); i++)
            {
                if (variables.ContainsKey(RPN[i]))
                    RPN[i] = Convert.ToString(variables[RPN[i]]);
            }
        }

        MyStack<double> numbers = new MyStack<double>();

        foreach (string token in RPN)
        {
            if (double.TryParse(token, out double number)) numbers.Push(number);

            else if(MathConsts.ContainsKey(token)) numbers.Push(MathConsts[token]);

            else if (LeftAssociativeInfixOperations.Contains(token) 
                || RightAssociativeInfixOperations.Contains(token)
                || BynaryPrefixOperations.Contains(token))
            {
                double b = numbers.Pop();
                double a = numbers.Pop();
                numbers.Push(EvaluateFunction(token, a, b));
            }

            else if (UnaryPrefixOperations.Contains(token))
            {
                numbers.Push(EvaluateFunction(token, numbers.Pop()));
            }
        }

        if (numbers.Size() != 1) throw new ArgumentException("Несогласованность операций и аргументов");

        return numbers.Pop();
    }
}
