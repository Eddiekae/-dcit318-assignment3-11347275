using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // a) Record: immutable financial data model
    public readonly record struct Transaction(
        int Id,
        DateTime Date,
        decimal Amount,
        string Category
    );

    // b) Payment behavior via interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c) Concrete processors with distinct messaging
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing {transaction.Category} for {transaction.Amount:C} on {transaction.Date:d}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Sent {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Broadcast payment of {transaction.Amount:C} ({transaction.Category}) on {transaction.Date:d}.");
        }
    }

    // d) General account
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.", nameof(accountNumber));
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");

            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        protected static void Validate(Transaction transaction)
        {
            if (transaction.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(transaction.Amount), "Amount must be positive.");
            if (string.IsNullOrWhiteSpace(transaction.Category))
                throw new ArgumentException("Category is required.", nameof(transaction.Category));
            // (Optional) Add further integrity checks here (e.g., future-dated rules).
        }

        // Deducts the amount (default behavior)
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Validate(transaction);
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account:{AccountNumber}] Applied {transaction.Amount:C} for {transaction.Category}. New balance: {Balance:C}");
        }
    }

    // e) Sealed specialized account
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            Validate(transaction);

            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[Savings:{AccountNumber}] Deducted {transaction.Amount:C} ({transaction.Category}). Balance: {Balance:C}");
        }
    }

    // f) Integration & simulation
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i) Savings account
            var savings = new SavingsAccount("SA-001", initialBalance: 1000m);
            Console.WriteLine($"Created SavingsAccount {savings.AccountNumber} with balance {savings.Balance:C}");

            // ii) Sample transactions
            var t1 = new Transaction(1, DateTime.Today, 120.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Today, 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Today, 700m, "Entertainment");

            // iii) Processors
            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);

            // iv) Apply to account (with SavingsAccount rules)
            savings.ApplyTransaction(t1);
            savings.ApplyTransaction(t2);
            savings.ApplyTransaction(t3);

            // v) Track transactions
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("\n--- Transaction Log ---");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"#{tx.Id}: {tx.Date:d} | {tx.Category,-13} | {tx.Amount,10:C}");
            }

            Console.WriteLine($"\nFinal Balance: {savings.Balance:C}");
        }

        public static void Main()
        {
            new FinanceApp().Run();
        }
    }
}
