using DeskMarket.Common;
using DeskMarket.Data;
using DeskMarket.Data.Models;
using DeskMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Security.Claims;

namespace DeskMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            CheckIfAuthenticated();

            AddProductViewModel model = new AddProductViewModel();
            model.Categories = await GetAllCategoriesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel model)
        {
            CheckIfAuthenticated();

            if (ModelState.IsValid)
            {
                string dateString = $"{model.AddedOn}";

                if (!DateTime.TryParseExact(dateString,
                    ValidationConstants.DateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime validDate))
                {
                    throw new InvalidOperationException("Invalid date format");
                }

                Product product = new Product()
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    ImageUrl = model.ImageUrl,
                    SellerId = GetUserId(),
                    AddedOn = DateTime.Today,
                    CategoryId = model.CategoryId
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            model.Categories = await GetAllCategoriesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Product> filteredProducts = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductsClients)
                .AsNoTracking()
                .Where(p => p.IsDeleted == false)
                .ToListAsync();

            try
            {
                CheckIfAuthenticated();
            }
            catch (Exception ex)
            {
                List<AllViewModel> nonAutModels = filteredProducts
                    .Select(p => new AllViewModel
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl
                    })
                    .ToList();

                return View(nonAutModels);
            }

            List<AllViewModel> models = filteredProducts
                    .Select(p => new AllViewModel
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        IsSeller = CheckIsSeller(p),
                        HasBought = CheckHasBought(p)
                    })
                    .ToList();

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            CheckIfAuthenticated();

            List<Product> myProducts = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.ProductsClients)
                .Where(p => p.ProductsClients
                                .Select(pc => pc.ClientId)
                                .Any(cIds => cIds.Contains(GetUserId())))
                .AsNoTracking()
                .ToListAsync();

            List<CartViewModel> models = myProducts
                .Select(p => new CartViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                })
                .ToList();

            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            CheckIfAuthenticated();

            Product? product = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.ProductsClients)
                .FirstOrDefaultAsync(p => p.Id == id);

            CheckIfProductIsNull(product);

            if (!CheckHasBought(product))
            {
                product
            .ProductsClients
            .Add(new ProductClient { ProductId = id, ClientId = GetUserId() });

                await _context.SaveChangesAsync();
            }
  

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Product? product = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductsClients)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            CheckIfProductIsNull(product);

            DetailsViewModel model = new DetailsViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                AddedOn = product.AddedOn.ToString(ValidationConstants.DateTimeFormat),
                CategoryName = product.Category.Name,
                Seller = product.Seller.UserName,
                HasBought = CheckHasBought(product),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CheckIfAuthenticated();

            Product? product = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Seller)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            CheckIfProductIsNull(product);

            if(product?.Seller.Id != GetUserId())
            {
                throw new ArgumentNullException();
            }


            EdtiViewModel model = new EdtiViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                AddedOn = product.AddedOn.ToString(ValidationConstants.DateTimeFormat),
                CategoryId = product.Category.Id,
                SellerId = product.Seller.Id,
                Categories = await GetAllCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EdtiViewModel model)
        {
            CheckIfAuthenticated();

            Product? product = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Seller)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            CheckIfProductIsNull(product);

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.SellerId = model.SellerId;
            product.AddedOn = DateTime.Today;
            product.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new RouteValueDictionary(
                new 
                { 
                    controller = "Product", action = "Details", Id = model.Id 
                }));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            CheckIfAuthenticated();

            Product? product = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            CheckIfProductIsNull(product);

            DeleteViewModel model = new DeleteViewModel()
            {
                Id = id,
                Seller = product.Seller.UserName,
                ProductName = product.ProductName,
                SellerId = product.Seller.Id,
                IsDeleted = product.IsDeleted
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            Product? product = await _context
                .Products
                .Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            CheckIfProductIsNull(product);

            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            Product? product = await _context
                .Products
                .Include(p => p.ProductsClients)
                .FirstOrDefaultAsync(p => p.Id == id);

            CheckIfProductIsNull(product);

            ProductClient? procuctClient = product
                .ProductsClients
                .FirstOrDefault(pc => pc.ClientId == GetUserId());

            if(procuctClient == null)
            {
                throw new ArgumentNullException();
            }

            product.ProductsClients.Remove(procuctClient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        //*************************************************************

        private async Task<List<GetCategoriesViewModel>> GetAllCategoriesAsync()
        {
            List<GetCategoriesViewModel> categories = await _context
                .Categories
                .Select(c => new GetCategoriesViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();

            return categories;
        }

        private string GetUserId()
        {
            string userId = string.Empty;

            if (User != null)
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return userId;
        }

        private bool CheckIsSeller(Product? product)
        {
            if (product == null)
            {
                throw new NullReferenceException();
            }

            if(product.Seller.Id == GetUserId())
            {
                return true;
            }

            return false;
        }

        private bool CheckHasBought(Product? product)
        {
            if (product == null)
            {
                throw new NullReferenceException();
            }

            if(product.ProductsClients
                .Select(pc => pc.ClientId)
                .Any(pc => pc.Contains(GetUserId())))
            {
                return true;
            }

            return false;
        }

        private void CheckIfProductIsNull(Product? product)
        {
            if (product == null)
            {
                throw new ArgumentNullException();
            }
        }

        private void CheckIfAuthenticated()
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }
        }
    }
}
