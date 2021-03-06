using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Perkjam.Client.ViewModels;
using Perkjam.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Perkjam.Client.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/users/");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                return View(new PerkIndexViewModel(await JsonSerializer.DeserializeAsync<List<User>>(responseStream)));
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception("Problem accessing the API");  
        }

        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> GetAddressFromIDP()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            if (metaDataResponse.IsError)
            {
                throw new Exception("Problem accessing the discovery endpoint.", metaDataResponse.Exception);
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoResponse = await idpClient.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Address = metaDataResponse.UserInfoEndpoint,
                    Token = accessToken
                });

            if (userInfoResponse.IsError)
            {
                throw new Exception("Problem accessing the userInfo endpoint.", userInfoResponse.Exception);
            }

            var address = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "address")?.Value;

            return View(new GetAddressFromIDPViewModel(address));
        }
        
        [Authorize(Policy = "CanAddUser")]
        public IActionResult AddUser()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanAddUser")]
        public async Task<IActionResult> AddUser(AddUserViewModel addUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index");
            }

            // create an UserForCreation instance
            var userForCreation = new UserForCreation()
            {
                Name = addUserViewModel.Name,
                Email = addUserViewModel.Email
            };

            // serialize it
            var serializedUserForCreation = JsonSerializer.Serialize(userForCreation);  
            
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/users");

            request.Content = new StringContent(
                serializedUserForCreation,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }
        
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/users/{id}");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }
        
        public async Task Logout()
        {
            var client = _httpClientFactory.CreateClient("IDPClient");
            
            var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }
            
            var accessTokenRevocationResponse = await client.RevokeTokenAsync(
                new TokenRevocationRequest
                {
                    Address = discoveryDocumentResponse.RevocationEndpoint,
                    ClientId = "perkjamclient",
                    ClientSecret = "secret",
                    Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
                });
            
            if (accessTokenRevocationResponse.IsError)
            {
                throw new Exception(accessTokenRevocationResponse.Error);
            }
            
            var refreshTokenRevocationResponse = await client.RevokeTokenAsync(
                new TokenRevocationRequest
                {
                    Address = discoveryDocumentResponse.RevocationEndpoint,
                    ClientId = "perkjamclient",
                    ClientSecret = "secret",
                    Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken)
                });
            
            if (refreshTokenRevocationResponse.IsError)
            {
                throw new Exception(accessTokenRevocationResponse.Error);
            }
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
