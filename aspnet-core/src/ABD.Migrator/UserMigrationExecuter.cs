using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Data;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ABD.Authorization.Users;
using ABD.Entities;
using ABD.EntityFrameworkCore;
using ABD.EntityFrameworkCore.Seed;
using ABD.MultiTenancy;

namespace ABD.Migrator
{
    public class UserMigrationExecuter : ITransientDependency
    {
        private const string _strKey = "2A03E2CF5ED7E73E";
        private const string _strIV = "9A3FD7CA41B36AF9";
        private const int AdminUserRoleId = 1;
        private const int AdUserRoleId = 3;
        private const int BdUserRoleId = 4;
        private readonly Log _log;
        public static ABDDbContext _context;
        private readonly UserManager _userManager;

        public UserMigrationExecuter(
            ABDDbContext context,
            UserManager userManager,
            Log log)
        {
            _log = log;
            _context = context;
            _userManager = userManager;
        }

        public bool  Run(bool skipConnVerification)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var hostConnStr = configuration.GetConnectionString("NMSDATA");
            var defaultConnStr = configuration.GetConnectionString("Default");

            if (hostConnStr.IsNullOrWhiteSpace())
            {
                _log.Write("Configuration file should contain a connection string named 'NMSDATA'");
                return false;
            }

            _log.Write("Host database: " + ConnectionStringHelper.GetConnectionString(hostConnStr));
            _log.Write("Default database: " + ConnectionStringHelper.GetConnectionString(defaultConnStr));
            if (!skipConnVerification)
            {
                _log.Write("Continue to transfer Users from Host to Default Database..? (Y/N): ");
                var command = Console.ReadLine();
                if (!command.IsIn("Y", "y"))
                {
                    _log.Write("Migration canceled.");
                    return false;
                }
            }

            _log.Write("HOST database migration started...");
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = hostConnStr;
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM CS_Accounts where userid!= 1 and PsEmail is not null order by USERID", conn);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string password = DecryptTextFromMemory(reader[2].ToString());
                            bool isAdUser = !string.IsNullOrEmpty(reader[27].ToString());
                            bool isBdUser = !string.IsNullOrEmpty(reader[32].ToString());

                            var user = _userManager.GetUserByIdAsync(Convert.ToInt32(reader[0].ToString()));

                            UpdateUsers(user.Result, password, isAdUser, isBdUser, defaultConnStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Write("An error occured during migration of host database:");
                _log.Write(ex.ToString());
                _log.Write("Canceled migrations.");
                return false;
            }

            _log.Write("HOST database migration completed.");
            _log.Write("--------------------------------------------------------");
            _log.Write("User data has been migrated successfully.");

            return true;
        }

        private string DecryptTextFromMemory(string strEnc)
        {
            try
            {
                byte[] Data = GetByte(strEnc);
                byte[] Key = GetByte(_strKey);
                byte[] IV = GetByte(_strIV);

                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(Data);

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, new DESCryptoServiceProvider().CreateDecryptor(Key, IV), CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[Data.Length + 1];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                // Convert the buffer into a string and return it.
                return new ASCIIEncoding().GetString(fromEncrypt);
            }
            catch (CryptographicException e)
            {
                _log.Write($"A Cryptographic error occurred: {e.Message}");
                return null;
            }
        }

        private byte[] GetByte(string sValue)
        {
            byte[] arrBInput = new byte[(sValue.Length / 2) - 1 + 1];
            int i;
            int x;

            for (x = 0; x <= (sValue.Length / 2) - 1; x++)
            {
                i = Convert.ToInt32(sValue.Substring(x * 2, 2), 16);
                arrBInput[x] = Convert.ToByte(i);
            }
            return arrBInput;
        }

        public void UpdateUsers(User user, string pass, bool isAdUser, bool isBdUser, string conn)
        {
            try
            {
                var builder = new DbContextOptionsBuilder<ABDDbContext>();
                ABDDbContextConfigurer.Configure(builder, conn);
                _context = new ABDDbContext(builder.Options);
                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, pass);
                var updatedUser = _context.Users.Update(user).Entity;
                if (isAdUser || isBdUser)
                {
                    var roleId = isAdUser && isBdUser ? AdminUserRoleId : isAdUser ? AdUserRoleId : BdUserRoleId;
                    _context.UserRoles.Add(new UserRole(1, updatedUser.Id, roleId));
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.Write("An error occured during insertion of user to default database");
                _log.Write(ex.ToString());
            }
        }
    }
}
