namespace BLL
{
    using System.Security.Cryptography;
    using System.Text;
    using DB_Connection;

    public sealed class UsersAuthentication
    {
        private readonly JournalDbContext _context;

        public UsersAuthentication(JournalDbContext context)
        {
            this._context = context;
        }

        public User AuthenticateUser(string username, string password)
        {
            var hashedPassword = this.GenerateSha256Hash(password);

            return this._context.Users
                .SingleOrDefault(u => u.Username == username && u.Password == hashedPassword);
        }

        private string GenerateSha256Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            var builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}