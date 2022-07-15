﻿using BusinessLayer.Interface;
using DataBaseLayer.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLoggers.Interface;
using RepositoryLayer.Services;
using RepositoryLayer.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FundooNotes_EFCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly FundooContext fundooContext;
        private readonly IUserBL userBL;
        private readonly ILoggerManager logger;

        public UserController(FundooContext fundooContext, IUserBL userBL, ILoggerManager logger)
        {
            this.fundooContext = fundooContext;
            this.userBL = userBL;
            this.logger = logger;
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserModel userModel)
        {
            try
            {
                this.logger.LogInfo($"User Registerd Email : {userModel.Email}");
                this.userBL.AddUser(userModel);
                return this.Ok(new { success = true, message = "User Created Successfully" });
            }
            catch (Exception ex)
            {
                this.logger.LogError($"User Registration Failed : {userModel.Email}");
                throw ex;
            }
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                List<User> getusers = new List<User>();
                getusers = this.userBL.GetAllUsers();
                return Ok(new { success = false, message = "GetAll users Fetch Successfully", data = getusers });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}