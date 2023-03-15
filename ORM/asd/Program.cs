namespace asd
{
     class Program
    {
        static void Main()
        {
            int[] data = new[] { 1, 2, 3, 3, 3, 4, 1, 1, 1, 1, 1, 1 };
            // Your CODE goes here.
            int count = 0;
            int longest = 0;
            int num = 0;
            int index = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                if (data[i] == data[i+1])
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (longest < count)
                {
                    index = i - count + 1;
                    longest = count;
                    num = data[i];
                }
            }
            for (int i = index; i < data.Length; i++)
            {
                data[i] = 0;
            }

            Console.WriteLine(string.Join(",", data));
        }



        static string Result(string a, string p)
        {
            int counter = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == p[i])
                {
                    counter++;
                }
            }
            return counter.ToString();
        }
    }
}
