using System;
using System.Threading;
using System.Threading.Tasks;

namespace locking
{
    class BankAccount
    {
        private object accountLock;
        private int balance;
        public int Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public BankAccount() 
        {
            accountLock = new object();
            balance = 0;
        }

        public void Deposit(int amount)
        {
            lock (accountLock)
            {
                balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (accountLock)
            {
                balance -= amount;
            }
        }

        public void Deposit2(int amount)
        {
            Interlocked.Add(ref balance, amount);
        }

        public void Withdraw2(int amount)
        {
            Interlocked.Add(ref balance, -amount);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var account = new BankAccount();

            for (int i = 0; i < 20; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 30; i++)
                    {
                        account.Deposit2(i + j);
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 30; i++)
                    {
                        account.Withdraw2(i + j);
                    }
                });
            }

            Console.WriteLine(account.Balance);
        }
    }
}
