using System;

namespace Web.Data.Model
{
    public class UserAccount
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}