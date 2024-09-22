namespace CASD_Task5;
using MyArrayList;

internal class Program
{
    public static void Main(string[] args)
    {
        string filePath = "index.txt";
        Console.WriteLine(GetUniqueTags(filePath));
    }

    public static MyArrayList<string> GetUniqueTags(string filePath)
    {
        MyArrayList<string> tags = GetTags(filePath);
        for (int i = 1; i < tags.Size(); i++) 
        { 
            for (int j = i - 1; j >= 0; j--)
            {
                if (isSameTags(tags[j], tags[i]))
                {
                    tags.RemoveAt(j);
                    i--;
                }
            }
        }
        return tags;
    }

    public static bool isSameTags(string tag1, string tag2) { 
        MyArrayList<char> symbols1 = new MyArrayList<char>(tag1.ToCharArray());
        MyArrayList<char> symbols2 = new MyArrayList<char>(tag2.ToCharArray());
        symbols1.Remove('/');
        symbols2.Remove('/');
        if (symbols1.Size() == symbols2.Size())
        {
            for (int i = 0; i < symbols1.Size(); i++)
            {
                if (!(symbols1[i] == symbols2[i] || Math.Abs(symbols1[i] - symbols2[i]) == 32 
                    && Char.IsLetter(symbols1[i]) && Char.IsLetter(symbols2[i])))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public static MyArrayList<string> GetTags(string filePath)
    {
        MyArrayList<string> tags = new MyArrayList<string>();
        using (var streamReader = new StreamReader(filePath))
        {
            string fileData = streamReader.ReadToEnd();
            for(int i = 0; i < fileData.Length; i++)
            {
                string tag = "<";
                if (fileData[i] == '<')
                {
                    i++;
                    while (i < fileData.Length && fileData[i] != '>') {
                        tag += fileData[i];
                        i++;
                    }
                    if(i < fileData.Length) tag += fileData[i];
                    if (isTag(tag)) tags.Add(tag);
                }
            }
        }
        return tags;
    }

    public static bool isTag(string tag)
    {
        int cntSlash = 0;
        bool itsOk = true;
        for(int i = 1; itsOk && i < tag.Length - 1; i++)
        {
            char c = tag[i];
            if(c== '/') cntSlash++;
            else if (!(Char.IsDigit(c) && i != 0 || Char.IsLetter(c))) itsOk = false;
        }
        return tag[0] == '<' && tag[tag.Length - 1] == '>'
            && (cntSlash == 1 && tag[1] == '/' || cntSlash == 0) && itsOk;
    }
}
