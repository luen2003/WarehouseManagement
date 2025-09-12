using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using log4net;

namespace PetroBM.Services.Services
{
    public interface IProductService
    {
        IEnumerable<MProduct> GetAllProduct();
        IEnumerable<MProduct> GetAllProductOrderByName();
        bool CreateProduct(MProduct product);
        bool UpdateProduct(MProduct product);
        bool DeleteProduct(int id);
        MProduct FindProductById(int id);
    }

    public class ProductService : IProductService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(ProductService));

        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;
        private readonly ICommandDetailService commandDetailService;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IEventService eventService, ICommandDetailService commandDetailService)
        {
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
            this.commandDetailService = commandDetailService;
        }

        public bool CreateProduct(MProduct product)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    productRepository.Add(product);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_PRODUCT_CREATE, product.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteProduct(int id)
        {
            MProduct product = this.FindProductById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    product.DeleteFlg = Constants.FLAG_ON;
                    productRepository.Update(product);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_PRODUCT_DELETE, product.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MProduct> GetAllProduct()
        {
            return productRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MProduct> GetAllProductOrderByName()
        {
            return productRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ProductName);
        }

        public bool UpdateProduct(MProduct product)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    productRepository.Update(product);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }
                //commandDetailService.UpdateCommandDetailProduct(product.ProductCode, product.ProductName);
                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_PRODUCT_UPDATE, product.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MProduct FindProductById(int id)
        {
            return productRepository.GetById(id);
        }
    }
}