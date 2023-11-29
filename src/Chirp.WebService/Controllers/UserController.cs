using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace Chirp.WebService.Controllers;




    public class UserController : BaseController
        {
            private readonly ICheepRepository _service;

            public UserController(ICheepRepository service)
            {
                _service = service;
            }

            
            //TODO: Doesn't work yet
        [HttpPost] 
        [Route("User/Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser()
        {
            string deleteUserUrl =
                $"https://BDSAGROUP11CHIRP.b2clogin.com/BDSAGROUP11CHIRP.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_DEMO_DELETE_MY_ACCOUNT&client_id=beb64166-c054-4fd6-b6a0-41828e2bae4b&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fbdsagroup11chirprazor.azurewebsites.net%2Fsignin-oidc&scope=openid&response_type=id_token&prompt=login";

            return Redirect(deleteUserUrl);
        }

}