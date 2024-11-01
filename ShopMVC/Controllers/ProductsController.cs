using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopMVC.DTOs;
using System.Net.Http.Headers;
using System.Text;

namespace MicroServiceMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly string BaseUrl = "http://ec2-50-18-232-183.us-west-1.compute.amazonaws.com:5001/";

        // GET: Products
        public async Task<ActionResult> Index()
        {
            List<ProductDto> products = new List<ProductDto>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/products"); // Adjusted endpoint
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ProductDto>>(responseContent);
                }
            }
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ProductDto product = new ProductDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/products/{id}"); // Adjusted endpoint
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<ProductDto>(responseContent);
                }
            }
            return View(product);
        }

        public async Task<ActionResult> Create()
        {
            await PopulateCategories();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateDto product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = JsonConvert.SerializeObject(product);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/products", contentString); // Adjusted endpoint

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(product);
        }


        public async Task<ActionResult> Edit(int id)
        {
            ProductDto product = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<ProductDto>(responseContent);
                }
            }

            await PopulateCategories();

            return View(product);
        }




        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ProductDto product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = JsonConvert.SerializeObject(product);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/products/{id}", contentString); // Adjusted endpoint

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error response and provide feedback to the user
                    ViewBag.ErrorMessage = "Failed to update the product.";
                }
            }

            // Repopulate categories in case of error
            await PopulateCategories();

            return View(product);
        }



        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            ProductDto product = new ProductDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/products/{id}"); // Adjusted endpoint
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<ProductDto>(responseContent);
                }
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.DeleteAsync($"api/products/{id}"); // Adjusted endpoint

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }
        private async Task PopulateCategories()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync("api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();


                    var rawResponse = JObject.Parse(responseContent);


                    var categoryValues = rawResponse["$values"] as JArray;


                    var categories = categoryValues.ToObject<List<CategoryDto>>();


                    ViewBag.CategorySelectList = new SelectList(categories, "CategoryId", "Name");
                }
                else
                {

                    ViewBag.CategorySelectList = new SelectList(new List<CategoryDto>(), "CategoryId", "Name");
                }
            }
        }



    }
}
