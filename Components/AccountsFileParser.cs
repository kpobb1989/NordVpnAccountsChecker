using System.Text.RegularExpressions;

namespace NordVpnAccountsChecker.Components
{
    internal static class AccountsFileParser
    {
        public static IDictionary<string, string> ParseDirectory(string relativePath)
        {
            var accounts = new Dictionary<string, string>();

            foreach (var path in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, relativePath), "*.txt"))
            {
                try
                {
                    var file = File.ReadAllText(path);

                    var matches = Regex.Matches(file, @"(?<login>[^\s@]\S+\.\S+)\:(?<password>\S+)");

                    foreach (var item in matches
                                .Select(s => new KeyValuePair<string, string>(s.Groups["login"].Value, s.Groups["password"].Value))
                                .DistinctBy(s => s.Key))
                    {
                        accounts.TryAdd(item.Key, item.Value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return accounts;
        }
    }
}
