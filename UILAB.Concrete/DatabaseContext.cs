using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Concrete
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DBConn")
        {
        }

        public virtual DbSet<AddressBookTB> AddressBook { get; set; }
        public virtual DbSet<ApprovalDetailTR> ApprovalDetailTRs { get; set; }
        public virtual DbSet<ApprovalGroupTR> ApprovalGroups { get; set; }
        public virtual DbSet<ApprovalHeaderTR> ApprovalHeaderTRs { get; set; }
        public virtual DbSet<ApprovalSchemeMaster> ApprovalSchemeMasters { get; set; }
        public virtual DbSet<ApprovalSettingTB> ApprovalSettings { get; set; }
        public virtual DbSet<AuditUserLog> AuditUserLogs { get; set; }
        public virtual DbSet<CountryMaster> CountryMasters { get; set; }
        public virtual DbSet<CustomerTB> Customers { get; set; }
        public virtual DbSet<DesignationMaster> DesignationMasters { get; set; }
        public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }
        public virtual DbSet<FileProduct> FileProducts { get; set; }
        public virtual DbSet<FileResourceTR> FileResourceTRs { get; set; }
        public virtual DbSet<FileUser> FileUsers { get; set; }
        public virtual DbSet<FileVehicle> FileVehicles { get; set; }
        public virtual DbSet<GenderMaster> GenderMasters { get; set; }
        //public virtual DbSet<LeaveSchemeTB> LeaveSchemes { get; set; }
        //public virtual DbSet<LeaveTypeTB> LeaveTypes { get; set; }
        public virtual DbSet<JobAssignEmpTR> JobAssignEmpTRs { get; set; }
        public virtual DbSet<JobTaskPartTR> JobTaskPartTRs { get; set; }
        public virtual DbSet<JobTaskPartDetailTR> JobTaskPartDetailTRs { get; set; }
        public virtual DbSet<JobStatusTB> JobStatuses { get; set; }
        public virtual DbSet<JobTasksTB> JobTasksTBs { get; set; }
        public virtual DbSet<JobTaskTR> JobTaskTRs { get; set; }
        public virtual DbSet<JobPackageTB> JobPackages { get; set; }
        public virtual DbSet<JobPackageListTB> JobPackageLists { get; set; }
        public virtual DbSet<JobTypeTB> JobTypes { get; set; }
        public virtual DbSet<JobTR> JobTRs { get; set; }
        public virtual DbSet<OrganizationSchemeInfo> OrganizationSchemeInfos { get; set; }
        public virtual DbSet<OrganizationSchemeTR> OrganizationSchemeTRs { get; set; }
        public virtual DbSet<ResourceBrandTB> ResourceBrands { get; set; }
        public virtual DbSet<ResourceDamaged> ResourceDamage { get; set; }
        public virtual DbSet<ResourceType> ResourceTypes { get; set; }
        public virtual DbSet<ResourceSubType> ResourceSubTypes { get; set; }
        public virtual DbSet<ResourceUserTR> ResourceUserTRs { get; set; }
        public virtual DbSet<ResourceTR> ResourceTRs { get; set; }
        public virtual DbSet<SalesInvoiceListTR> SalesInvoiceLists { get; set; }
        public virtual DbSet<SalesStatusTB> SalesStatusTBs { get; set; }
        public virtual DbSet<SalesInvoiceTR> SalesInvoiceTRs { get; set; }
        public virtual DbSet<SalesTaxTB> SalesTaxes { get; set; }
        public virtual DbSet<SecurityActiveUsers> SecurityActiveUser { get; set; }
        public virtual DbSet<SecurityUserDevices> SecurityUserDevice { get; set; }
        public virtual DbSet<SegmentTypeMaster> SegmentTypeMasters { get; set; }
        public virtual DbSet<SettingGeneralTR> SettingGenerals { get; set; }
        public virtual DbSet<StockBrandTB> StockBrands { get; set; }
        public virtual DbSet<StockPurchaseOrderTR> StockPurchaseOrders { get; set; }
        public virtual DbSet<StockPurchaseOrderListTmp> StockPurchaseOrderListTmps { get; set; }
        public virtual DbSet<StockPurchaseOrderListTR> StockPurchaseOrderLists { get; set; }
        public virtual DbSet<StockStatusMasterTB> StockStatusMasters { get; set; }
        public virtual DbSet<StockTR> StockTRs { get; set; }
        public virtual DbSet<StockSubTypeTB> StockSubTypes { get; set; }
        public virtual DbSet<StockTypeTB> StockTypes { get; set; }
        public virtual DbSet<StockWarehouseTB> StockWarehouses { get; set; }
        public virtual DbSet<StockUnitTB> StockUnits { get; set; }
        public virtual DbSet<SupplierTB> Suppliers { get; set; }
        public virtual DbSet<UserRoles> UserRole { get; set; }
        public virtual DbSet<UserSecurity> UserSecurities { get; set; }
        public virtual DbSet<VehicleMakeTB> VehicleMakes { get; set; }
        public virtual DbSet<VehicleMileageTR> VehicleMileageTRs { get; set; }
        public virtual DbSet<VehicleModelTB> VehicleModels { get; set; }
        public virtual DbSet<VehicleTR> VehicleTRs { get; set; }
        public virtual DbSet<VehicleTypeTB> VehicleTypes { get; set; }
        public virtual DbSet<VehicleFuelTypeTB> VehicleFuelTypes { get; set; }
        public virtual DbSet<VehicleTransTypeTB> VehicleTransTypes { get; set; }
        public virtual DbSet<VehicleDriverTR> VehicleDriverTRs { get; set; }

        // -----------------------  Views -------------------------------------
        public virtual DbSet<vw_ApprovalGroupTR> vw_ApprovalGroupTRs { get; set; }
        public virtual DbSet<vw_EmployeeMaster> vw_EmployeeMasters { get; set; }
        public virtual DbSet<vw_ResourceTR> vw_ResourceTRs { get; set; }
        public virtual DbSet<vw_UserSecurity> vw_UserSecurities { get; set; }
        public virtual DbSet<vw_VehicleTR> vw_VehicleTRs { get; set; }
        public virtual DbSet<vw_JobAssignEmpTR> vw_JobAssignEmpTRs { get; set; }
        public virtual DbSet<vw_JobTR> vw_JobTRs { get; set; }        
        public virtual DbSet<vw_JobTasksTR> vw_JobTasksTRs { get; set; }
        public virtual DbSet<vw_JobTaskPartTR> vw_JobTaskPartTRs { get; set; }
        public virtual DbSet<vw_JobTaskPartDetailTR> vw_JobTaskPartDetailTRs { get; set; }
        public virtual DbSet<vw_JobTaskPartDetailTmpTR> vw_JobTaskPartDetailTmpTRs { get; set; }
        public virtual DbSet<vw_SalesInvoiceTR> vw_SalesInvoiceTRs { get; set; }
        public virtual DbSet<vw_StockTR> vw_StockTRs { get; set; }
    }
}
