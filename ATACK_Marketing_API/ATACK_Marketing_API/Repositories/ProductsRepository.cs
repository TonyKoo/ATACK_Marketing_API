using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class ProductsRepository {
        private readonly MarketingDbContext _context;

        public ProductsRepository(MarketingDbContext context) {
            _context = context;
        }

        public Product GetEventProduct(int productId) {
            return _context.Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public (bool, Product) AddEventProduct(EventVendor theEventVendor, ProductInputViewModel newProduct) {
            bool isSuccessful = false;
            Product product = new Product {
                ProductName = newProduct.ProductName,
                ProductPrice = newProduct.ProductPrice,
                EventVendor = theEventVendor
            };

            try {
                _context.Products.Add(product);
                _context.SaveChanges();
                isSuccessful = true;
            } catch(Exception) {

            }

            return (isSuccessful, product);
        }

        public bool UpdateEventProduct(Product theProduct, ProductInputViewModel updatedProduct) {
            bool isSuccessful = false;

            try {
                theProduct.ProductName = updatedProduct.ProductName;
                theProduct.ProductPrice = updatedProduct.ProductPrice;

                _context.Products.Update(theProduct);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }

        public bool RemoveEventProduct(Product theProduct) {
            bool isSuccessful = false;

            try {
                _context.Products.Remove(theProduct);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }
    }
}
