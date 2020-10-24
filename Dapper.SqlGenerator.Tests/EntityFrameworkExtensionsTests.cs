using System;
using System.Collections.Generic;
using NUnit.Framework;
using Dapper.SqlGenerator.Extensions;

namespace Dapper.SqlGenerator.Tests
{
    [TestFixture]
    public class EntityFrameworkExtensionsTests
    {
        [Test]
        public void TestExtensionsCanConfigure()
        {
            OnModelCreating(DapperSqlGenerator.Configure());
        }

        private void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("People");

                entity.HasIndex(e => e.Id)
                    .IsUnique()
                    .HasFilter("([ImportGuid] IS NOT NULL)")
                    .HasName("IX_PersonId");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasDiscriminator<string>("Discriminator")
                    .HasValue<Employee>("Employee")
                    .HasValue<Contractor>("Contractor");
                
                entity.HasOne(d => d.MainAddress)
                    .WithMany()
                    .WithMany(p => p.MainAddressPeople)
                    .HasForeignKey(d => d.MainAddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.PersonInGroups_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasMany(x => x.MainAddressPeople)
                    .WithOne(x => x.MainAddress)
                    .HasForeignKey(x => x.MainAddressId);
            });
        }

        private class Person
        {
            public int Id { get; set; }
            
            public string Name { get; set; }
            
            public int Status { get; set; }
            
            public DateTime DateOfBirth { get; set; }

            public string Discriminator { get; set; }
            
            public int MainAddressId { get; set; }
            
            public Address MainAddress { get; set; }
            
            public ICollection<Address> Addresses { get; set; }
        }

        private class Address
        {
            public int Id { get; set; }
            
            public string Value { get; set; }
            
            public ICollection<Person> MainAddressPeople { get; set; }
        }

        private class Employee : Person { }

        private class Contractor : Person { }
    }
}