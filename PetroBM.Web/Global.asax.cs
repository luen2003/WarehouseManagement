using PetroBM.Common.Util;
using PetroBM.Domain.Entities;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PetroBM.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public List<MTankLive> TankLiveList { get; set; }
        public List<MLiveDataArm> LiveDataArmList { get; set; }
        private ITankService tankService;
        private ICommandDetailService CommandDetailService;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "Messages";
            DefaultModelBinder.ResourceClassKey = "Messages";

            log4net.Config.XmlConfigurator.Configure();

            // Auto log tanklive data in 6 hours
             TankLiveList = new List<MTankLive>();
            LiveDataArmList = new List<MLiveDataArm>();

            this.tankService = DependencyResolver.Current.GetService<ITankService>();
            this.CommandDetailService = DependencyResolver.Current.GetService<ICommandDetailService>();

            Thread liveChartThread = new Thread(UpdateTankLiveList);
            liveChartThread.IsBackground = true;
            liveChartThread.Start();
            Application["LiveChartThread"] = liveChartThread;

            //Thread calculateV15Thread = new Thread(CalculateV15);
            //calculateV15Thread.IsBackground = true;
            //calculateV15Thread.Start();
            //Application["calculateV15Thread"] = calculateV15Thread;

            Application["TankLiveList"] = TankLiveList;
            Application["LiveDataArmList"] = LiveDataArmList;


			Application["CollectionThread"] = new Thread(CollectMemory);
            ((Thread)Application["CollectionThread"]).IsBackground = true;
            ((Thread)Application["CollectionThread"]).Start();
        }
        protected void Application_PreSendRequestHeaders()

        {
            Response.Headers.Remove("Server");           //Remove Server Header  
            Response.Headers.Remove("X-AspNet-Version"); //Remove X-AspNet-Version Header
        }
        protected void Application_End()
        {
            //try
            //{
            //    Thread thread = (Thread)Application["calculateV15Thread"];
            //    if (thread != null && thread.IsAlive)
            //    {
            //        thread.Abort();

            //    }
            //}
            //catch { }

            try
            {
                Thread thread = (Thread)Application["LiveChartThread"];
                if (thread != null && thread.IsAlive)
                {
                    thread.Abort();
                }
            }
            catch { }

            try
            {
                Thread collectionThread = (Thread)Application["CollectionThread"];
                if (collectionThread != null && collectionThread.IsAlive)
                {
					collectionThread.Abort();
                }
            } catch { }
        }

        private void CollectMemory()
        {
            while (true)
            {
                GC.Collect();
                try
                {
                    Thread.Sleep(5000);
                } catch { }
            }
        }

        private void UpdateTankLiveList()
        {
            while (true)
            {
                //Thread.Sleep(1);
                //Task CalculateValue15Task = Task.Run(() =>
                //{
                //    CommandDetailService.CalculateValue15();
                //});
                //CommandDetailService.CalculateValue15();

                //*******Xử lí cho bể trực tuyến **************************************************************************************************
                var tankLiveList = tankService.GetNewestTankLive();
                foreach (var tankLive in tankLiveList)
                {
                    var tempLive = TankLiveList.Where(tl => tl.TankId == tankLive.TankId && tl.WareHouseCode == tankLive.WareHouseCode)
                    .OrderByDescending(tl => tl.InsertDate).FirstOrDefault();

                    if (tempLive != null)
                    {
                        if (tankLive.InsertDate > tempLive.InsertDate)
                        {
                           // tankLive.MTank = null; //ThangNK Do đã bỏ quan hệ rồi không dùng thằng này nữa
                            TankLiveList.Add(tankLive);
                        }
                    }
                    else
                    {
                        //tankLive.MTank = null; //ThangNK Do đã bỏ quan hệ rồi không dùng thằng này nữa
                        TankLiveList.Add(tankLive);
                    }
                }

                ////// Remove all tank live 6 hours ago
                TankLiveList.RemoveAll(tl => tl.InsertDate < DateTime.Now.AddHours(-Constants.HOURS_LIVE_CHART));
                //***********************************************************************************************************************************


                ///// Xử lí cho họng trực tuyển *********************************************************************************************
                var liveDataArmList = tankService.GetNewestLiveDataArm();
                foreach (var liveDataArm in liveDataArmList)
                {
                    var tempLive = LiveDataArmList.Where(tl => tl.ArmNo == liveDataArm.ArmNo && tl.WareHouseCode == liveDataArm.WareHouseCode)
                    .OrderByDescending(tl => tl.TimeLog).FirstOrDefault();

                    if (tempLive != null)
                    {
                        if (liveDataArm.TimeLog > tempLive.TimeLog)
                        {
                            LiveDataArmList.Add(liveDataArm);
                        }
                    }
                    else
                    {
                        LiveDataArmList.Add(liveDataArm);
                    }
                }

                ////// Remove all tank live 6 hours ago
                LiveDataArmList.RemoveAll(tl => tl.TimeLog < DateTime.Now.AddHours(-Constants.HOURS_LIVE_CHART));

                //***************************************************************************************************************************

                Thread.Sleep(10000);
            }
        }

        private void CalculateV15()
        {
            while (true)
            {
                Thread.Sleep(1);
                //Task CalculateValue15Task = Task.Run(() =>
                //{
                //    CommandDetailService.CalculateV15();
                //});
                CommandDetailService.CalculateV15();


                Thread.Sleep(20000); //nghi 20 giay
            }
        }
    }
}
