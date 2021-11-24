using AutoMapper;
using Perkjam.API.Services;
using Perkjam.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Perkjam.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;

        public UsersController(
            IUserRepository repository,
            IWebHostEnvironment hostingEnvironment,
            IMapper mapper)
        {
            _repository = repository ?? 
                                 throw new ArgumentNullException(nameof(repository));
            _hostingEnvironment = hostingEnvironment ?? 
                                  throw new ArgumentNullException(nameof(hostingEnvironment));
            _mapper = mapper ?? 
                      throw new ArgumentNullException(nameof(mapper));
        }

        //[Authorize]
        [HttpGet()]
        public IActionResult GetUsers()
        {
            try
            {
                var usersFromRepo = _repository.GetAllUsers();

                // Mapping 
                var usersToReturn = _mapper.Map<IEnumerable<User>>(usersFromRepo);

                return Ok(usersToReturn);
            }
            catch (Exception ex)
            {
                // TODO Add Logging
                return StatusCode(500, ex);
            }
        }

        //[Authorize]
        [HttpGet("{id:int}", Name = "GetUser")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var result = _repository.GetUser(id);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<User>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //[Authorize]
        [HttpPost()]
        public IActionResult CreateUser([FromBody] UserForCreation userForCreation)
        {
            try
            {
                if (_repository.GetUser(userForCreation.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email in use");
                }

                if (ModelState.IsValid)
                {
                    var userEntity = _mapper.Map<Entities.User>(userForCreation);

                    _repository.AddUser(userEntity);

                    if (_repository.Save())
                    {
                        var userToReturn = _mapper.Map<User>(userEntity);

                        return CreatedAtRoute("GetUser", new { id = userToReturn.Id }, userToReturn);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            return BadRequest(ModelState);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserForUpdate userForUpdate)
        {
            try
            {
                var userFromRepo = _repository.GetUser(id);
                if (userFromRepo == null) return NotFound();

                _mapper.Map(userForUpdate, userFromRepo);
                _repository.UpdateUser(userFromRepo);

                if (_repository.Save())
                {
                    return Ok(_mapper.Map<User>(userFromRepo));
                }
                else
                {
                    return StatusCode(500);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userFromRepo = _repository.GetUser(id);
                if (userFromRepo == null) return NotFound();

                _repository.DeleteUser(userFromRepo);

                if (_repository.Save())
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}