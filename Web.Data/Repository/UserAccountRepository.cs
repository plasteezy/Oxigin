using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Web.Data.Contract;
using Web.Data.Model;

namespace Web.Data.Repository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        public List<UserAccount> AllAccounts()
        {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            var accountList = new List<UserAccount>();

            using (var conn = new SqlConnection(connString))
            {
                var query = "SELECT * FROM AspNetUsers";
                var cmd = new SqlCommand(query, conn);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var account = new UserAccount
                        {
                            Id = reader["Id"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            DateCreated = DateTime.Parse(reader["DateCreated"].ToString()),
                            LastLoginDate = DateTime.Parse(reader["LastLoginDate"].ToString()),
                        };

                        accountList.Add(account);
                    }
                }
            }

            return accountList;
        }

        public UserAccount FindByUserId(string userId)
        {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            UserAccount account = null;

            using (var conn = new SqlConnection(connString))
            {
                var query = "SELECT * FROM AspNetUsers WHERE Id = '" + userId + "'";
                var cmd = new SqlCommand(query, conn);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        account = new UserAccount
                        {
                            Id = reader["Id"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            DateCreated = DateTime.Parse(reader["DateCreated"].ToString()),
                            LastLoginDate = DateTime.Parse(reader["LastLoginDate"].ToString()),
                        };
                    }
                }
            }

            return account;
        }

        public UserAccount FindByEmail(string email)
        {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            UserAccount account = null;

            using (var conn = new SqlConnection(connString))
            {
                var query = "SELECT * FROM AspNetUsers WHERE UserName = '" + email + "'";
                var cmd = new SqlCommand(query, conn);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        account = new UserAccount
                        {
                            Id = reader["Id"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            DateCreated = DateTime.Parse(reader["DateCreated"].ToString()),
                            LastLoginDate = DateTime.Parse(reader["LastLoginDate"].ToString()),
                        };
                    }
                }
            }

            return account;
        }

        public UserAccount FindByPhone(string phone)
        {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            UserAccount account = null;

            using (var conn = new SqlConnection(connString))
            {
                var query = "SELECT * FROM AspNetUsers WHERE PhoneNumber = '" + phone + "' and PhoneNumberConfirmed = 1";
                var cmd = new SqlCommand(query, conn);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        account = new UserAccount
                        {
                            Id = reader["Id"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            DateCreated = DateTime.Parse(reader["DateCreated"].ToString()),
                            LastLoginDate = DateTime.Parse(reader["LastLoginDate"].ToString()),
                        };
                    }
                }
            }

            return account;
        }

        public void UpdateLoginDate(string username)
        {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (var conn = new SqlConnection(connString))
            {
                var query = $"UPDATE AspNetUsers SET LastLoginDate = '{DateTime.Now}' WHERE UserName = '{username}'";

                var cmd = new SqlCommand(query, conn);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}