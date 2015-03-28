﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Console
{
    public class TestDbContext : DbContext
    {
        public TestDbContext()
            : base("test") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ConfigureTeamEntity(modelBuilder);
            ConfigurePlayerEntity(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var sqliteConnectionInitializer = new TestDbContextInitializer(
                Database.Connection.ConnectionString, modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        private static void ConfigureTeamEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.RegisterEntityType(typeof(Team));
        }

        private static void ConfigurePlayerEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.RegisterEntityType(typeof(Player));

            modelBuilder.Entity<Player>().ToTable("TeamPlayer");

            modelBuilder.Entity<Player>()
                .HasRequired(p => p.Team)
                .WithMany(team => team.Players)
                .WillCascadeOnDelete(true);
        }
    }

    public class TestDbContextInitializer : SqliteContextInitializer<TestDbContext>
    {
        public TestDbContextInitializer(string connectionString, DbModelBuilder modelBuilder)
            : base(connectionString, modelBuilder) { }

        protected override void Seed(TestDbContext context)
        {
            context.Set<Team>().Add(new Team());
        }
    }
}
