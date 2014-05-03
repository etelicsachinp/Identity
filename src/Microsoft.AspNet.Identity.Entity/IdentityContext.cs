// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using System;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.Data.Entity;
using Microsoft.Data.SqlServer;
using Microsoft.Data.InMemory;
using Microsoft.Data.Entity.Metadata;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityContext :
        IdentityContext<EntityUser, EntityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public IdentityContext() { }
        public IdentityContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }

    public class IdentityContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : DbContext
        where TUser : EntityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : EntityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TKey : IEquatable<TKey>
    {

        public DbSet<TUser> Users { get; set; }
        public DbSet<TRole> Roles { get; set; }

        public IdentityContext(IServiceProvider serviceProvider)
        : base(serviceProvider) { }

        public IdentityContext() { }

        protected override void OnConfiguring(EntityConfigurationBuilder builder)
        {
//#if NET45
//            builder.SqlServerConnectionString(@"Server=(localdb)\v11.0;Database=IdentityDb3;Trusted_Connection=True;");
//#else
            builder.UseInMemoryStore();
//#endif
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TUser>()
                .Key(u => u.Id)
                .Properties(ps => ps.Property(u => u.UserName));
                //.ToTable("AspNetUsers");
            builder.Entity<TRole>()
                .Key(r => r.Id);
                //.ToTable("AspNetRoles");
 
            builder.Entity<TUserRole>()
                .Key(u => u.Id)
                //TODO: .Key(r => new { r.UserId, r.RoleId })
                .ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId))
                .ForeignKeys(fk => fk.ForeignKey<TRole>(f => f.RoleId));
                //.ToTable("AspNetUserRoles");

            builder.Entity<TUserLogin>()
                .Key(u => u.Id)
                //TODO: .Key(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId));
            //.ToTable("AspNetUserLogins");

            builder.Entity<TUserClaim>()
                .Key(c => c.Id)
                .ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId));
            //.ToTable("AspNetUserClaims");

        }

    }
}