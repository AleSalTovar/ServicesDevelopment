using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class BankService
{
    private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/") };

    public async Task CreateAccount(BankAccount bankAccount)
    {
        var json = JsonSerializer.Serialize(bankAccount);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("accounts", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Error creating account in database.");
        }
    }

    public async Task<BankAccount> GetBalanceAccount(string accountNumber)
    {
        var response = await client.GetAsync($"accounts/{accountNumber}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BankAccount>(json);
        }
        return null;
    }

    public async Task<BankAccount> DepositAmount(string accountNumber, decimal amountValue)
    {
        var account = await GetBalanceAccount(accountNumber);
        if (account != null)
        {
            account.BalanceAmount += amountValue;
            await UpdateAccount(account);
        }
        return account;
    }

    public async Task<BankAccount> WithdrawalAmount(string accountNumber, decimal amountValue)
    {
        var account = await GetBalanceAccount(accountNumber);
        if (account != null && (account.BalanceAmount + account.OverdraftAmount) >= amountValue)
        {
            account.BalanceAmount -= amountValue;
            await UpdateAccount(account);
            return account;
        }
        return null;
    }

    private async Task UpdateAccount(BankAccount account)
    {
        var json = JsonSerializer.Serialize(account);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"accounts/{account.AccountNumber}", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Error updating account in database.");
        }
    }
}
