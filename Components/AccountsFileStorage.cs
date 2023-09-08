using Newtonsoft.Json;

namespace NordVpnAccountsChecker.Components
{
    internal class AccountsFileStorage
    {
        private readonly string _filePath;

        private readonly Dictionary<string, string> _accounts = new();

        public AccountsFileStorage(string fileName)
        {
            _filePath = Path.Combine(Environment.CurrentDirectory, $"{fileName}.json");
        }

        public void ReadAccounts()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            var content = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var accounts = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    _accounts.TryAdd(account.Key, account.Value);
                }
            }
        }

        public void AddAccount(string login, string password)
        {
            _accounts.TryAdd(login, password);

            File.WriteAllText(_filePath, JsonConvert.SerializeObject(_accounts, Formatting.Indented));
        }

        public IReadOnlyDictionary<string, string> GetAccounts() => _accounts;
    }
}
