using Microsoft.AspNetCore.Mvc;
using OdeToFood.Entities;
using OdeToFood.Services;
using OdeToFood.ViewModels;

namespace OdeToFood.Controllers
{
    public class HomeController : Controller
    {
        private IGreeter _greeter;
        private IRestaurantData _restaurantData;

        public HomeController(IRestaurantData restaurantData, IGreeter greeter)
        {
            _restaurantData = restaurantData;
            _greeter = greeter;
        }

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

        // These route constraint attributes help the MVC framework know which view to display.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RestaurantEditViewModel model)
        {
            Restaurant newRestaurant = new Restaurant();
            newRestaurant.Cuisine = model.Cuisine;
            newRestaurant.Name = model.Name;

            newRestaurant = _restaurantData.Add(newRestaurant);

            // This follows the POST-REDIRECT-GET pattern.
            // If I return the Details view here (as seen below), it works, but the URL is still set to /home/create.
            // Thus, refreshing the page could result in the user accidentally posting data to the server twice.
            // RedirectToAction tells the browser to instead request the Details view on its own properly, i.e. via /home/details/{id}.
            return RedirectToAction("Details", new { id = newRestaurant.Id } );

            // return View("Details", newRestaurant);
        }
    }
}