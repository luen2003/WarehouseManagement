namespace PetroBM.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;

    using PetroBM.Domain.Entities;

    public partial class PetroBMContext : DbContext
    {
        public PetroBMContext()
            : base("PetroBMConnection")
        {
        }

        public virtual DbSet<MAlarm> MAlarms { get; set; }
        public virtual DbSet<MAlarmType> MAlarmTypes { get; set; }
        public virtual DbSet<MBarem> MBarems { get; set; }
        public virtual DbSet<MEvent> MEvents { get; set; }
        public virtual DbSet<MEventType> MEventTypes { get; set; }
        public virtual DbSet<MPermission> MPermissions { get; set; }
        public virtual DbSet<MProduct> MProducts { get; set; }
        public virtual DbSet<MTank> MTanks { get; set; }
        public virtual DbSet<MTankDensity> MTankDensity { get; set; }
        public virtual DbSet<MTankGrp> MTankGrps { get; set; }
        public virtual DbSet<MTankLive> MTankLives { get; set; }
        public virtual DbSet<MTankLog> MTankLogs { get; set; }
        public virtual DbSet<MTankManual> MTankManuals { get; set; }
        public virtual DbSet<MClock> MClock { get; set; }
        public virtual DbSet<MClockExport> MClockExport { get; set; }
        public virtual DbSet<MImportInfo> MImportInfo { get; set; }
        public virtual DbSet<MTankImport> MTankImport { get; set; }
        public virtual DbSet<MTankImportTemp> MTankImportTemps { get; set; }
        public virtual DbSet<MUser> MUsers { get; set; }
        public virtual DbSet<MUserGrp> MUserGrps { get; set; }
        public virtual DbSet<MUserGrpPermission> MUserGrpPermissions { get; set; }
        public virtual DbSet<WSystemSetting> WSystemSettings { get; set; }
        public virtual DbSet<MDriver> MDriver { get; set; }
        public virtual DbSet<MConfigArm> MConfigArm { get; set; }
        public virtual DbSet<MCustomer> MCustomer { get; set; } 
        public virtual DbSet<MCustomerGroup> MCustomerGroup { get; set; }
        public virtual DbSet<MCard> MCard { get; set; }
        public virtual DbSet<MPrice> MPrice { get; set; }
        public virtual DbSet<MDensity> MDensity { get; set; }
        public virtual DbSet<MSeal> MSeal { get; set; }
        public virtual DbSet<MCommand> MCommand { get; set; }
        public virtual DbSet<MCommandDetail> MCommandDetail { get; set; }
        public virtual DbSet<MVehicle> MVehicle { get; set; }
        public virtual DbSet<MWareHouse> MWareHouse { get; set; }
        public virtual DbSet<MShip> MShip { get; set; }
        public virtual DbSet<MImage> MImage { get; set; }
        public virtual DbSet<MDispatchWaterHist> MDispatchWaterHist { get; set; }
        public virtual DbSet<MInvoice> MInvoice { get; set; }
        public virtual DbSet<MInvoiceDetail> MInvoiceDetail { get; set; }
        public virtual DbSet<MLiveDataArm> MLiveDataArm { get; set; }
        public virtual DbSet<MTankGrpTank> MTankGrpTank { get; set; }
        public virtual DbSet<MConfigArmGrp> MConfigArmGrp { get; set; }
        public virtual DbSet<MConfigArmGrpConfigArm> MConfigArmGrpConfigArm { get; set; }
        public virtual DbSet<MUserGroupUser> MUserGroupUser { get; set; }
        public virtual DbSet<MWareHousePermission> MWareHousePermission { get; set; }

        public virtual DbSet<MExportArmImport> MExportArmImport { get; set; }

        public virtual DbSet<MDispatch> MDispatch { get; set; }

        public virtual DbSet<MDispatchWater> MDispatchWater { get; set; }

        public virtual DbSet<MDepartment> MDepartment { get; set; }

        public virtual DbSet<MLocation> MLocation { get; set; }

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MAlarm>()
               .Property(e => e.Value)
               .IsUnicode(false);

            modelBuilder.Entity<MAlarm>()
                .Property(e => e.ConfirmUser)
                .IsUnicode(false);

            modelBuilder.Entity<MAlarm>()
                .Property(e => e.SolveUser)
                .IsUnicode(false);
            

            //modelBuilder.Entity<MImportInfo>()
            //.Property(e => e.InsertUser)
            //.IsUnicode(false);

            //modelBuilder.Entity<MImportInfo>()
            //    .Property(e => e.UpdateUser)
            //    .IsUnicode(false);

            //modelBuilder.Entity<MImportInfo>()
            //    .HasMany(e => e.MTankImport)
            //    .WithRequired(e => e.MImportInfo)
            //    .HasForeignKey(e => new { e.ImportInfoId, e.WareHouseCode })
            //    .WillCascadeOnDelete(false);


            modelBuilder.Entity<MTankGrpTank>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MTankGrpTank>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MBarem>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MBarem>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MEvent>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MEventType>()
                .HasMany(e => e.MEvents)
                .WithRequired(e => e.MEventType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MPermission>()
                .HasMany(e => e.MUserGrpPermissions)
                .WithRequired(e => e.MPermission)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MProduct>()
                .Property(e => e.ProductCode)
                .IsUnicode(false);

            modelBuilder.Entity<MProduct>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<MProduct>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MProduct>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MProduct>()
               .HasMany(e => e.MImportInfo)
               .WithRequired(e => e.MProduct)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<MTank>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MTank>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);
            modelBuilder.Entity<MTankDensity>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MTankGrp>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MTankGrp>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MTankManual>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MTankManual>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MClock>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MClock>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MClock>()
                .HasMany(e => e.MClockExport)
                .WithRequired(e => e.MClock)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MImportInfo>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MImportInfo>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            //modelBuilder.Entity<MImportInfo>()
            //    .HasMany(e => e.MClockExport)
            //    .WithRequired(e => e.MImportInfo)
            //    .HasForeignKey(e => e.ImportInfoId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<MImportInfo>()
            //   .HasMany(e => e.MTankImport)
            //   .WithRequired(e => e.MImportInfo)
            //   .HasForeignKey(e => e.ImportInfoId)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<MImportInfo>()
            //    .HasMany(e => e.MTankImportTemps)
            //    .WithRequired(e => e.MImportInfo)
            //    .HasForeignKey(e => e.ImportInfoId)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<MUser>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<MUser>()
                .Property(e => e.Passwd)
                .IsUnicode(false);

            modelBuilder.Entity<MUser>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MUser>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MUserGrp>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MUserGrp>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MUserGrp>()
                .HasMany(e => e.MUserGrpPermissions)
                .WithRequired(e => e.MUserGrp)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WSystemSetting>()
                .Property(e => e.KeyCode)
                .IsUnicode(false);

            modelBuilder.Entity<WSystemSetting>()
                .Property(e => e.Value)
                .IsUnicode(true);

            modelBuilder.Entity<MDriver>()
                .Property(e => e.IdentificationNumber)
                .IsUnicode(false);

            modelBuilder.Entity<MDriver>()
                .Property(e => e.DriversLicense)
                .IsUnicode(false);

            modelBuilder.Entity<MDriver>()
                .Property(e => e.SavetyCertificates)
                .IsUnicode(false);

            modelBuilder.Entity<MDriver>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDriver>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);


            modelBuilder.Entity<MConfigArm>()
                .Property(e => e.ProductCode_1)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MConfigArm>()
                .Property(e => e.ProductCode_2)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MConfigArm>()
                .Property(e => e.ProductCode_3)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MConfigArm>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MConfigArm>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MInvoice>()
                .Property(e => e.InvoiceNo)
                .IsUnicode(false);
            modelBuilder.Entity<MCustomerGroup>()
                .Property(e => e.CustomerGroupCode)
                .IsUnicode(false);

            modelBuilder.Entity<MCustomer>()
                .Property(e => e.CustomerCode)
                .IsUnicode(false);

            modelBuilder.Entity<MCustomer>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCustomer>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCustomer>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

           

            modelBuilder.Entity<MDispatch>()
                .Property(e => e.ProductCode)
                .IsUnicode(false);

            modelBuilder.Entity<MDispatch>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDispatch>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);
            modelBuilder.Entity<MDispatchWater>()
                .Property(e => e.ProductCode)
                .IsUnicode(false);

            modelBuilder.Entity<MDispatchWater>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDispatchWater>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);


            modelBuilder.Entity<MDepartment>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<MDepartment>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDepartment>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDriver>()
            .Property(e => e.IdentificationNumber)
            .IsUnicode(false);

            modelBuilder.Entity<MCard>()
            .Property(e => e.CardData)
            .IsUnicode(false);

            modelBuilder.Entity<MCard>()
            .Property(e => e.InsertUser)
            .IsUnicode(false);

            modelBuilder.Entity<MCard>()
            .Property(e => e.UpdateUser)
            .IsUnicode(false);

            modelBuilder.Entity<MPrice>()
            .Property(e => e.Unit)
            .IsUnicode(false);

            modelBuilder.Entity<MDensity>()
            .Property(e => e.ProductCode)
            .IsFixedLength()
            .IsUnicode(false);

            modelBuilder.Entity<MDensity>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDensity>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MDensity>()
            .Property(e => e.InsertUser)
            .IsUnicode(false);

            modelBuilder.Entity<MDensity>()
            .Property(e => e.UpdateUser)
            .IsUnicode(false);

            modelBuilder.Entity<MSeal>()
                .Property(e => e.WorkOrder)
                .HasPrecision(7, 0);


            modelBuilder.Entity<MSeal>()
                .Property(e => e.Seal1)
                .IsUnicode(false);

            modelBuilder.Entity<MSeal>()
                .Property(e => e.Seal2)
                .IsUnicode(false);

            modelBuilder.Entity<MSeal>()
                .Property(e => e.CardData)
                .IsUnicode(false);

            modelBuilder.Entity<MSeal>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MSeal>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MSeal>()
                .Property(e => e.Ratio)
                .IsUnicode(false);

            modelBuilder.Entity<MVehicle>()
                .Property(e => e.VehicleNumber)
                .IsUnicode(false);

            modelBuilder.Entity<MVehicle>()
                .Property(e => e.RegisterNumber)
                .IsUnicode(false);

            modelBuilder.Entity<MVehicle>()
                .Property(e => e.AccreditationNumber)
                .IsUnicode(false);

            modelBuilder.Entity<MVehicle>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MVehicle>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MWareHouse>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MWareHouse>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCommand>()
                .Property(e => e.CardData)
                .IsUnicode(false);

            modelBuilder.Entity<MCommand>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCommand>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.WorkOrder)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.ProductCode)
                .IsFixedLength();

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.CardData)
                .IsUnicode(false);

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.Vehicle)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MCommandDetail>()
                .Property(e => e.Flaglog)
                .IsUnicode(false);

            modelBuilder.Entity<MInvoice>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MInvoice>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MInvoiceDetail>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MInvoiceDetail>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MLiveDataArm>()
               .Property(e => e.WorkOrder)
               .HasPrecision(10, 0);

            modelBuilder.Entity<MLiveDataArm>()
                .Property(e => e.ProductCode)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MLiveDataArm>()
                .Property(e => e.VehicleNumber)
                .IsUnicode(false);

            modelBuilder.Entity<MLiveDataArm>()
                .Property(e => e.CardData)
                .IsUnicode(false);

            //modelBuilder.Entity<MLiveDataArm>()
            //    .Property(e => e.InsertUser)
            //    .IsUnicode(false);

            //modelBuilder.Entity<MLiveDataArm>()
            //    .Property(e => e.UpdateUser)
            //    .IsUnicode(false);

            modelBuilder.Entity<MUserGroupUser>()
                .Property(e => e.InsertUser)
                .IsUnicode(false);

            modelBuilder.Entity<MUserGroupUser>()
                .Property(e => e.UpdateUser)
                .IsUnicode(false);

            modelBuilder.Entity<MExportArmImport>()
            .Property(e => e.ProductCode)
            .IsFixedLength();


        }

        /// <summary>
        /// Call Store procedure "Report_TankExport"
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="wareHouseCode"></param>
        /// <param name="armNo"></param>
        /// <param name="productCode"></param>
        /// <param name="vehicle"></param>
        /// <param name="customerName"></param>
        /// <param name="typeExport"></param>
        /// <returns></returns>
        public virtual ObjectResult<ReportTankExport> Report_TankExport(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<byte> wareHouseCode, Nullable<byte> armNo, string productCode, string vehicle, string customerName, Nullable<byte> typeExport)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));

            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));

            var wareHouseCodeParameter = wareHouseCode.HasValue ?
                new ObjectParameter("WareHouseCode", wareHouseCode) :
                new ObjectParameter("WareHouseCode", typeof(byte));

            var armNoParameter = armNo.HasValue ?
                new ObjectParameter("ArmNo", armNo) :
                new ObjectParameter("ArmNo", typeof(byte));

            var productCodeParameter = productCode != null ?
                new ObjectParameter("ProductCode", productCode) :
                new ObjectParameter("ProductCode", typeof(string));

            var vehicleParameter = vehicle != null ?
                new ObjectParameter("Vehicle", vehicle) :
                new ObjectParameter("Vehicle", typeof(string));

            var customerNameParameter = customerName != null ?
                new ObjectParameter("CustomerName", customerName) :
                new ObjectParameter("CustomerName", typeof(string));

            var typeExportParameter = typeExport.HasValue ?
                new ObjectParameter("TypeExport", typeExport) :
                new ObjectParameter("TypeExport", typeof(byte));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportTankExport>("Report_TankExport", startDateParameter, endDateParameter, wareHouseCodeParameter, armNoParameter, productCodeParameter, vehicleParameter, customerNameParameter, typeExportParameter);
        }

    }
}
