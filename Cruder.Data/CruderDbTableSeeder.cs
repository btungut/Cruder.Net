using Cruder.Core;
using Cruder.Core.ExceptionHandling;
using Cruder.Core.Module;
using Cruder.Data.Model;
using System;
using System.Data.Entity;

namespace Cruder.Data
{
    internal class CruderDbTableSeeder : DropCreateDatabaseIfModelChanges<CruderDbContext>
    {
        protected override void Seed(CruderDbContext context)
        {
            base.Seed(context);

            var userGroup = context.UserGroups.Add(new UserGroupEntity
                {
                    Name = "Default Admin Group",
                    Description = "It has been created by framework.",
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                });

            var user = context.Users.Add(new UserEntity
                {
                    Fullname = "Cruder Admin",
                    Username = "cruderadmin",
                    Mail = "admin@cruder.net",
                    Password = "123456",
                    IsSystemAdmin = true,
                    UserGroupId = userGroup.Id,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                });

            context.Configs.Add(new ConfigEntity
            {
                Key = "AppName",
                DevelopmentValue = "Cruder",
                ProductionValue = "Cruder",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            });

            context.SaveChanges();

            try
            {
                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.Configs ADD CONSTRAINT UK_Key UNIQUE ([Key])");

                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.Users ALTER COLUMN [Username] NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CS_AS");
                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.Users ALTER COLUMN [Password] NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CS_AS");
                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.Users ALTER COLUMN [Mail] NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CS_AS");
                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.Users ADD CONSTRAINT UK_Username UNIQUE (Username)");
                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.Users ADD CONSTRAINT UK_Mail UNIQUE (Mail)");

                context.Database.ExecuteSqlCommand("ALTER TABLE Cruder.UserGroups ADD CONSTRAINT UK_Name UNIQUE (Name)");
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("CruderDbTableSeeder<>().Seed()", "An error has occured while altering database constraints.", e);
                var logResult = Logger.Log(LogType.Error, Priority.High, exception.Message, exception, LogModule.Framework);

                throw exception;
            }
        }
    }
}
