using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Services.Services;

namespace PetroBM.Web
{
    public static class UnityConfig
    {
        public static UnityContainer container;

        public static void RegisterComponents()
        {
            container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            // e.g. container.RegisterType<ITestService, TestService>();
            // Infrastructure


            container.RegisterType<IDbFactory, DbFactory>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            // Repositories
            container.RegisterType<IExportArmImportRepository, ExportArmImportRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserGroupRepository, UserGroupRepository>();
            container.RegisterType<IPermissionRepository, PermissionRepository>();
            container.RegisterType<IUserGrpPermissionRepository, UserGrpPermissionRepository>();
            container.RegisterType<IEventRepository, EventRepository>();
            container.RegisterType<IEventTypeRepository, EventTypeRepository>();
            container.RegisterType<IAlarmRepository, AlarmRepository>();
            container.RegisterType<IAlarmTypeRepository, AlarmTypeRepository>();
            container.RegisterType<IConfigurationRepository, ConfigurationRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<ITankRepository, TankRepository>();
            container.RegisterType<ITankDensityRepository, TankDensityRepository>();
            container.RegisterType<ITankGroupRepository, TankGroupRepository>();
            container.RegisterType<ITankManualRepository, TankManualRepository>();
            container.RegisterType<ITankLiveRepository, TankLiveRepository>();
            container.RegisterType<ITankLogRepository, TankLogRepository>();
            container.RegisterType<IBaremRepository, BaremRepository>();
            container.RegisterType<IImportRepository, ImportRepository>();
            container.RegisterType<ITankImportRepository, TankImportRepository>();
            container.RegisterType<IClockRepository, ClockRepository>();
            container.RegisterType<IClockExportRepository, ClockExportRepository>();
            container.RegisterType<ITankImportTempRepository, TankImportTempRepository>();
            container.RegisterType<IDriverRepository, DriverRepository>();
            container.RegisterType<IConfigArmRepository, ConfigArmRepository>();
            container.RegisterType<IInvoiceRepository, InvoiceRepository>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerGroupRepository, CustomerGroupRepository>();
            container.RegisterType<IWareHouseRepository, WareHouseRepository>();
            container.RegisterType<ICardRepository, CardRepository>();
            container.RegisterType<IVehicleRepository, VehicleRepository>();
            container.RegisterType<IPriceRepository, PriceRepository>();
            container.RegisterType<IDensityRepository, DensityRepository>();
            container.RegisterType<ICommandRepository, CommandRepository>();
            container.RegisterType<ICommandDetailRepository, CommandDetailRepository>();
            container.RegisterType<ISealRepository, SealRepository>();
            container.RegisterType<IInvoiceDetailRepository, InvoiceDetailRepository>();
            container.RegisterType<ILiveDataArmRepository, LiveDataArmRepository>();
            container.RegisterType<ITankGrpTankRepository, TankGrpTankRepository>();
            container.RegisterType<ITankLiveRepository, TankLiveRepository>();
            container.RegisterType<IConfigArmGrpRepository, ConfigArmGrpRepository>();
            container.RegisterType<IConfigArmGrpConfigArmRepository, ConfigArmGrpConfigArmRepository>();
            container.RegisterType<IDispatchRepository, DispatchRepository>();
            container.RegisterType<IDepartmentRepository, DepartmentRepository>();
            container.RegisterType<IDispatchWaterService, DispatchWaterService>();
            container.RegisterType<IDispatchWaterRepository, DispatchWaterRepository>();
            container.RegisterType<ILocationRepository, LocationRepository>();
            container.RegisterType<IShipRepository, ShipRepository>();
            container.RegisterType<IImageRepository, ImageRepository>();

            // Service
            container.RegisterType<IShipService, ShipService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserGroupService, UserGroupService>();
            container.RegisterType<IEventService, EventService>();
            container.RegisterType<IAlarmService, AlarmService>();
            container.RegisterType<IConfigurationService, ConfigurationService>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<ITankService, TankService>();
            container.RegisterType<ITankGroupService, TankGroupService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IChartService, ChartService>();
            container.RegisterType<IImportService, ImportService>();
            container.RegisterType<IDriverService, DriverService>();
            container.RegisterType<IConfigArmService, ConfigArmService>();
            container.RegisterType<IInvoiceService, InvoiceService>();
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<ICustomerGroupService, CustomerGroupService>();
            container.RegisterType<IWareHouseService, WareHouseService>();
            container.RegisterType<ICardService, CardService>();
            container.RegisterType<IVehicleService, VehicleService>();
            container.RegisterType<IPriceService, PriceService>();
            container.RegisterType<IDensityService, DensityService>();
            container.RegisterType<ICommandService, CommandService>();
            container.RegisterType<IDispatchService, DispatchService>();
            container.RegisterType<IDepartmentService, DepartmentService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<ICommandDetailService, CommandDetailService>();
            container.RegisterType<ISealService, SealService>();
            container.RegisterType<IInvoiceDetailService, InvoiceDetailService>();     
            container.RegisterType<ILiveDataArmService, LiveDataArmService>();
            container.RegisterType<ITankGrpTankService, TankGrpTankService>();
            container.RegisterType<ITankImportTempService, TankImportTempService>();
            container.RegisterType<ITankImportService, TankImportService>();
            container.RegisterType<IClockService, ClockService>();
            container.RegisterType<IClockExportService, ClockExportService>();
            container.RegisterType<ITankLiveService, TankLiveService>();
            container.RegisterType<IConfigArmGrpService, ConfigArmGrpService>();
            container.RegisterType<IConfigArmGrpConfigArmService, ConfigArmGrpConfigArmService>();
            container.RegisterType<IExportArmImportService, ExportArmImportService>();
            container.RegisterType<IDispatchWaterService, DispatchWaterService>();
            container.RegisterType<ILocationService, LocationService>();
            container.RegisterType<IImageService, ImageService>();


            //container.
        }
    }
}