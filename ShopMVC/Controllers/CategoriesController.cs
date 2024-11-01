using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopMVC.DTOs;
using System.Net.Http.Headers;
using System.Text;

namespace ShopMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly string BaseUrl = "http://ec2-50-18-232-183.us-west-1.compute.amazonaws.com:5001/";


        public async Task<ActionResult> Index()
        {
            List<CategoryDto> categories = new List<CategoryDto>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    categories = JsonConvert.DeserializeObject<List<CategoryDto>>(responseContent);
                }
            }
            return View(categories);
        }


        public async Task<ActionResult> Details(int id)
        {
            CategoryDto category = new CategoryDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<CategoryDto>(responseContent);
                }
            }
            return View(category);
        }


        public ActionResult Create()
        {
            var categoryDto = new CategoryDto();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryCreateDto category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = JsonConvert.SerializeObject(category);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/categories", contentString);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }


        public async Task<IActionResult> Edit(int id)
        {
            CategoryDto category = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Get category data from API
                var response = await client.GetAsync($"api/categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<CategoryDto>(responseContent);
                }
            }

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.CategoryId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var jsonContent = JsonConvert.SerializeObject(categoryDto);
                    var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                    var response = await client.PutAsync($"api/categories/{id}", contentString);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to update the category.";
                    }
                }
            }

            return View(categoryDto);
        }




        public async Task<ActionResult> Delete(int id)
        {
            CategoryDto category = new CategoryDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<CategoryDto>(responseContent);
                }
            }
            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.DeleteAsync($"api/categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }
    }
}
