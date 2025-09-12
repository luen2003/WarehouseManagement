using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PetroBM.Domain.Entities;
using PetroBM.Data.Repositories;
using PetroBM.Data.Infrastructure;
using PetroBM.Common.Util;

namespace PetroBM.Services.Services
{
    public interface IConfigurationService
    {
        WSystemSetting GetConfiguration(string keyCode);
        bool SaveConfiguration(string updateUser, string keyCode, string value, string eventName);
        bool SaveCompNameAndUnit(string updateUser, string keyCodeCompName, string valueCompName, string keyCodeUnit, string valueUnit, string eventName);
        IEnumerable<WSystemSetting> GetAllConfiguration();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository ConfigurationRepository;
        private readonly IEventService eventService;
        private readonly IUnitOfWork unitOfWork;

        public ConfigurationService(IConfigurationRepository ConfigurationRepository,
            IEventService eventService, IUnitOfWork unitOfWork)
        {
            this.ConfigurationRepository = ConfigurationRepository;
            this.eventService = eventService;
            this.unitOfWork = unitOfWork;
        }

        #region IConfigurationService Members

        public WSystemSetting GetConfiguration(string keyCode)
        {
            var config = ConfigurationRepository.GetMany(cf => cf.KeyCode.Equals(keyCode)).SingleOrDefault();

            if (config == null)
                config = new WSystemSetting();

            return config;
        }

        public bool SaveConfiguration(string updateUser, string keyCode, string value, string eventName)
        {
            bool rs = false;

            try
            {
                var config = GetConfiguration(keyCode);

                if (String.IsNullOrEmpty(config.KeyCode))
                {
                    config = new WSystemSetting();
                    config.KeyCode = keyCode;
                    config.Value = value;
                    ConfigurationRepository.Add(config);
                }
                else
                {
                    config.Value = value;
                    ConfigurationRepository.Update(config);
                }

                SaveConfig();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    eventName, updateUser);
            }
            catch { }

            return rs;
        }

        private void SaveConfig()
        {
            unitOfWork.Commit();
        }

        public bool SaveHomeImage(string updateUser, string fileName)
        {
            bool rs = false;

            try
            {
                var config = GetConfiguration(Constants.CONFIG_HOME_IMAGE);

                if (String.IsNullOrEmpty(config.KeyCode))
                {
                    config = new WSystemSetting();
                    config.KeyCode = Constants.CONFIG_HOME_IMAGE;
                    config.Value = fileName;
                    ConfigurationRepository.Add(config);
                }
                else
                {
                    config.Value = fileName;
                    ConfigurationRepository.Update(config);
                }

                SaveConfig();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_HOME_IMAGE, updateUser);
            }
            catch { }

            return rs;
        }

        public IEnumerable<WSystemSetting> GetAllConfiguration()
        {
            return ConfigurationRepository.GetAll();
        }

        public bool SaveCompNameAndUnit(string updateUser, string keyCodeCompName, string valueCompName, string keyCodeUnit, string valueUnit, string eventName)
        {
            bool rs = false;

            try
            {
                var configComp = GetConfiguration(keyCodeCompName);
                var configUnit = GetConfiguration(keyCodeUnit);

                if (String.IsNullOrEmpty(configComp.KeyCode))
                {
                    configComp = new WSystemSetting();
                    configComp.KeyCode = keyCodeCompName;
                    configComp.Value = valueCompName;
                    ConfigurationRepository.Add(configComp);
                }
                else
                {
                    configComp.Value = valueCompName;
                    ConfigurationRepository.Update(configComp);
                }

                if (String.IsNullOrEmpty(configUnit.KeyCode))
                {
                    configUnit = new WSystemSetting();
                    configUnit.KeyCode = keyCodeUnit;
                    configUnit.Value = valueUnit;
                    ConfigurationRepository.Add(configUnit);
                }
                else
                {
                    configUnit.Value = valueUnit;
                    ConfigurationRepository.Update(configUnit);
                }
                SaveConfig();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    eventName, updateUser);
            }
            catch { }

            return rs;
        }
        #endregion
    }
}
