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
    
        public virtual DbSet<tbl_Mp3Options> tbl_Mp3Options { get; set; }
        public virtual DbSet<tbl_QuestionsMaster> tbl_QuestionsMaster { get; set; }
        public virtual DbSet<tbl_QuestionType> tbl_QuestionType { get; set; }
        public virtual DbSet<tbl_Exam> tbl_Exam { get; set; }
        public virtual DbSet<tbl_ExamResultDetails> tbl_ExamResultDetails { get; set; }
        public virtual DbSet<tbl_QuestionLevel> tbl_QuestionLevel { get; set; }
        public virtual DbSet<tbl_Package> tbl_Package { get; set; }
        public virtual DbSet<tbl_Feedback> tbl_Feedback { get; set; }
        public virtual DbSet<tbl_PackageBuyDetails> tbl_PackageBuyDetails { get; set; }
        public virtual DbSet<tbl_StudentFee> tbl_StudentFee { get; set; }
        public virtual DbSet<tbl_CouponCode> tbl_CouponCode { get; set; }
        public virtual DbSet<tbl_AgentPackage> tbl_AgentPackage { get; set; }
        public virtual DbSet<tbl_Materials> tbl_Materials { get; set; }
        public virtual DbSet<tbl_StudentWebinar> tbl_StudentWebinar { get; set; }
        public virtual DbSet<tbl_Webinar> tbl_Webinar { get; set; }
        public virtual DbSet<tbl_GeneralSetting> tbl_GeneralSetting { get; set; }
        public virtual DbSet<tbl_Students> tbl_Students { get; set; }
        public virtual DbSet<tbl_AdminUsers> tbl_AdminUsers { get; set; }
        public virtual DbSet<tbl_ContactForm> tbl_ContactForm { get; set; }
    }
}
