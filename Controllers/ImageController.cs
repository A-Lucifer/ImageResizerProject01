using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureProject01.Controllers
{
    public class ImageController : Controller
    {
        //Instance of ImageService class
        ImageService imageService = new ImageService();

        // GET: Image  
        public ActionResult Index()
        {
            return View("Upload");      //returning Upload view 
        }

        //Parameterized Upload action with input picture from user as parameter
        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo)
        {
            //condition to check extension of input file
            //and accept only .jpeg/.jpg/.png
            if (Path.GetExtension(photo.FileName).ToLower().ToString() == ".jpeg" || Path.GetExtension(photo.FileName).ToLower().ToString() == ".jpg" || Path.GetExtension(photo.FileName).ToLower().ToString() == ".png")
            {
                //pass the image to method of ImageService class to upload on container
                var imageUrl = await imageService.UploadImageAsync(photo);
                //condition to check if input image is already resized or not
                //if yes then it will pass resized image url to view "Cache"
                //view "Cache" will directly disply resized image instead of uploading and resizing
                if (imageUrl.Contains("thumbnail-image"))
                {
                    ViewBag.thumbnailImage = imageUrl.ToString();
                    return View("Cache");
                }
                else
                {   
                    //url of original image will get stored in TempData dictionary 
                    TempData["originalImage"] = imageUrl.ToString();
                    //Delay of 10000 miliseconds i.e. 10 seconds
                    //so that we can wait till resized image get stored in 'thumbnail-image' container
                    //and fetch its url
                    await Task.Delay(10000);
                    var thumbnailUrl = imageService.FetchImage();
                    //url of thumbnail image will get stored in TempData dictionary
                    TempData["thumbnailImage"] = thumbnailUrl.ToString();
                }
            }
            else
            {
                //if input file is not of image type, this return a view "Error"
                ViewBag.errorMsg = "Invalid filetype!!! Please upload image only.";
                return View("Error");
            }
                //Redirect to action "LatestImage"
                return RedirectToAction("LatestImage");
        }

        //LatestImage action will store urls from TempData dictionary in respective ViewBags
        //ViewBags will be used in LatestImage view
        public ActionResult LatestImage()
        {
            if (TempData["originalImage"] != null && TempData["thumbnailImage"] != null)
            {
                //Covert url to string and store in ViewBag to access in views
                ViewBag.originalImage = Convert.ToString(TempData["originalImage"]);
                ViewBag.thumbnailImage = Convert.ToString(TempData["thumbnailImage"]);
            }
            else
            {
                ViewBag.errorMsg = "Bad Request";
                return View("Error");
            }
            return View();
        }


    }
}