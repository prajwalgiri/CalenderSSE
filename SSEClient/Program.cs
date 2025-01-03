
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Start Connection");
var client = new HttpClient();
int count = 1;
using var stream = await client.GetStreamAsync("https://localhost:7183/calender");
using (StreamReader reader = new(stream))
{
    while (!reader.EndOfStream)
    {
        var line = await reader.ReadLineAsync();
        Console.WriteLine("Received Data Listening Count:"+count.ToString());
        Console.WriteLine(line);
        count++;
    }
    
}
Console.ReadLine();