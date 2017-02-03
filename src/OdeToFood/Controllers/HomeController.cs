using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.Entities;
using OdeToFood.Services;
using OdeToFood.ViewModels;

namespace OdeToFood.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IGreeter _greeter;
        private IRestaurantData _restaurantData;

        public HomeController(IRestaurantData restaurantData, IGreeter greeter)
        {
            _restaurantData = restaurantData;
            _greeter = greeter;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = new HomePageViewModel();
            model.Restaurants = _restaurantData.GetAll();
            model.CurrentMessage = _greeter.GetGreeting();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var model = _restaurantData.Get(id);

            if (model == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _restaurantData.Get(id);

            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RestaurantEditViewModel model)
        {
            Restaurant restauraunt = _restaurantData.Get(id);

            if (ModelState.IsValid)
            {
                restauraunt.Cuisine = model.Cuisine;
                restauraunt.Name = model.Name;
                _restaurantData.Commit();
                return RedirectToAction("Details", new { id = restauraunt.Id });
            }

            return View(restauraunt);
        }

        // These route constraint attributes help the MVC framework know which view to display.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RestaurantEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Restaurant newRestaurant = new Restaurant();
                newRestaurant.Cuisine = model.Cuisine;
                newRestaurant.Name = model.Name;

                newRestaurant = _restaurantData.Add(newRestaurant);
                _restaurantData.Commit();

                // This follows the POST-REDIRECT-GET pattern.
                // If I return the Details view here (as seen below), it works, but the URL is still set to /home/create.
                // Thus, refreshing the page could result in the user accidentally posting data to the server twice.
                // RedirectToAction tells the browser to instead request the Details view on its own properly, i.e. via /home/details/{id}.
                return RedirectToAction("Details", new { id = newRestaurant.Id });

                // return View("Details", newRestaurant);
            }

            return View();
        }
    }
}