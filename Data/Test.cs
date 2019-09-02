using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureSpa.Data
{
    public class Test
    {
        public void CreateAccount(TestDbContext context)
        {
            var account = new Account();

            var user = new User
            {
                Email = "john@northwindtraders.com"
            };

            var membership = new Membership { Account = account, User = user };

            context.Users.Add(user);
            context.Accounts.Add(account);
            context.Memberships.Add(membership);

            context.SaveChanges();
        }
    }

    public class Account
    {
        public int Id { get; set; }
    }

    public class Membership
    {
        public int AccountId { get; set; }

        public int UserId { get; set; }

        public Account Account { get; set; }

        public User User { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
}
