class Program
{
    private static BankService bankService = new BankService();

    static async Task Main(string[] args)
    {
        await Menu();
    }

    static async Task Menu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Banking Menu ---");
            Console.WriteLine("1. Create Account");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Check Balance");
            Console.WriteLine("4. Withdraw Money");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await CreateAccount(); break;
                case "2": await DepositAmount(); break;
                case "3": await GetBalance(); break;
                case "4": await WithdrawalAmount(); break;
                case "5": return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    static async Task CreateAccount()
    {
        string accountNumber;
        do
        {
            Console.Write("Account Number (10 digits): ");
            accountNumber = Console.ReadLine();
            if (!IsValidAccountNumber(accountNumber))
            {
                Console.WriteLine("Invalid account number. It must be exactly 10 numeric digits.");
            }
        } while (!IsValidAccountNumber(accountNumber));

        string accountOwner;
        do
        {
            Console.Write("Account Owner (max 50 characters): ");
            accountOwner = Console.ReadLine();
            if (accountOwner.Length > 50)
            {
                Console.WriteLine("Invalid account owner name. Maximum 50 characters allowed.");
            }
        } while (accountOwner.Length > 50);

        decimal balance;
        do
        {
            Console.Write("Initial Balance: ");
        } while (!decimal.TryParse(Console.ReadLine(), out balance));

        int accountType;
        do
        {
            Console.Write("Account Type (0: Savings, 1: Checking): ");
        } while (!int.TryParse(Console.ReadLine(), out accountType) || (accountType != 0 && accountType != 1));

        decimal overdraft = (accountType == 1) ? 1000000 : 0;

        BankAccount account = new BankAccount(accountNumber, accountOwner, balance, accountType, overdraft);
        await bankService.CreateAccount(account);
        Console.WriteLine("Account successfully created.");
    }

    static bool IsValidAccountNumber(string accountNumber)
    {
        return accountNumber.Length == 10 && accountNumber.All(char.IsDigit);
    }

    static async Task DepositAmount()
    {
        Console.Write("Account Number: ");
        string accountNumber = Console.ReadLine();
        Console.Write("Deposit Amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());

        var account = await bankService.DepositAmount(accountNumber, amount);
        if (account != null)
        {
            Console.WriteLine($"Deposit successful. New balance: {account.BalanceAmount}");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    static async Task GetBalance()
    {
        Console.Write("Account Number: ");
        string accountNumber = Console.ReadLine();

        var account = await bankService.GetBalanceAccount(accountNumber);
        if (account != null)
        {
            Console.WriteLine($"Current balance: {account.BalanceAmount}");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    static async Task WithdrawalAmount()
    {
        Console.Write("Account Number: ");
        string accountNumber = Console.ReadLine();
        Console.Write("Withdrawal Amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());

        var account = await bankService.WithdrawalAmount(accountNumber, amount);
        if (account != null)
        {
            Console.WriteLine($"Withdrawal successful. New balance: {account.BalanceAmount}");
        }
        else
        {
            Console.WriteLine("Insufficient funds or account does not exist.");
        }
    }
}
