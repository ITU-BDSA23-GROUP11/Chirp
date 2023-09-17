namespace Chirp.CLI.Client;

public class UserInterface
{
    public static void Read(string filePath) {
        using (StreamReader sr = new StreamReader(filePath)) {
            sr.ReadLine();
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                int firstComma = line.IndexOf(",");
                int lastComma = line.LastIndexOf(",");
                string readUser = line.Substring(0, firstComma);
                string readMessage = line.Substring(firstComma + 1, lastComma);
                long readTimestamp = long.Parse(line.Substring(lastComma + 1));
                Console.WriteLine(readUser + " @ " + Conversion.TimeStampConversion(readTimestamp) + ": " + readMessage);
                //string[] split = line.Split(",");
                //Console.WriteLine(split[2]);
                //Console.WriteLine(split[0] + " @ " + timeStampConversion(long.Parse(split[2])) + ": " + split[1]);
            }
        }
    }
}