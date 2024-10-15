using MyPriorityQueue;

namespace Requests;

internal class Request : IComparable<Request>
{
    private int priority;
    private int number;
    private int step;

    public int Priority => priority;
    public int Number => number;
    public int Step => step;

    public Request(int priority, int number, int step)
    {
        this.priority = priority;
        this.number = number;
        this.step = step;
    }

    public int CompareTo(Request? r)
    {
        if (r == null) throw new ArgumentNullException();
        return priority - r.priority;
    }
}

internal class RequestHandler
{
    private MyPriorityQueue<Request> priorityQueue = new MyPriorityQueue<Request>();
    private StreamWriter writer;
    private int number = 0;
    private int step = 0;
    private Request? lastRemovedRequest = null;

    public RequestHandler(StreamWriter? writer) { 
        if (writer == null) throw new ArgumentNullException();
        this.writer = writer;
    }

    public (Request, int) Handle(int countOfSteps)
    {
        if(countOfSteps < 1) throw new ArgumentException();

        int time = 0;
        
        for (int _ = 0; _ < countOfSteps; _++)
        {
            time++;
            step++;
            AddStep();
            RemoveStep();
        }
        while (true)
        {
            step++;
            if (!RemoveStep()) break;
            time++;
        }

        return (lastRemovedRequest!, time);
    }

    public string RequestReport(Request? request)
    {
        if (request == null) return "";
        return $"Номер: {request.Number}; Приоритет: {request.Priority}; Шаг: {request.Step}";
    }

    private void AddStep()
    {
        Random random = new Random();
        int countOfRequests = random.Next(1, 11);
        for (int _ = 0; _ < countOfRequests; _++)
        {
            Request request = new Request(random.Next(1, 6), ++number, step);
            priorityQueue.Add(request);
            writer.WriteLine($"Add {RequestReport(request)}");
        }
    }

    private bool RemoveStep()
    {
        Request? removedRequest = priorityQueue.Pool();
        if (removedRequest == null) return false;
        lastRemovedRequest = removedRequest;
        writer.WriteLine($"Remove {RequestReport(removedRequest)}");
        return true;
    }
}

