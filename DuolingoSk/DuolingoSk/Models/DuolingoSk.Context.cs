﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DuolingoSk.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DuolingoSk_Entities : DbContext
    {
        public DuolingoSk_Entities()
            : base("name=DuolingoSk_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_AdminUsers> tbl_AdminUsers { get; set; }
        public virtual DbSet<tbl_GeneralSetting> tbl_GeneralSetting { get; set; }
        public virtual DbSet<tbl_Mp3Options> tbl_Mp3Options { get; set; }
        public virtual DbSet<tbl_QuestionsMaster> tbl_QuestionsMaster { get; set; }
        public virtual DbSet<tbl_QuestionType> tbl_QuestionType { get; set; }
    }
}
