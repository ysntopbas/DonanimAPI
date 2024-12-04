﻿using DonanimAPI.Models;
using DonanimAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DonanimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMongoCollection<UserDevice> _userDeviceCollection;
        private readonly IMongoCollection<DonanimBilgileri> _donanimBilgileriCollection;

        public DeviceController(UserService userService, IMongoDatabase database)
        {
            _userService = userService;
            _userDeviceCollection = database.GetCollection<UserDevice>("UserDevices");
            _donanimBilgileriCollection = database.GetCollection<DonanimBilgileri>("DonanimBilgileri");
        }

        [Authorize]
        [HttpPost("AddDevice")]
        public async Task<IActionResult> AddDevice([FromBody] UserDevice userDevice)
        {
            try
            {
                await _userService.AddUserDeviceAsync(userDevice.Username, userDevice.DeviceID);
                return Ok("Device added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        //[HttpGet("GetDevices/{username}")]
        //public async Task<IActionResult> GetDevices(string username)
        //{
        //    try
        //    {
        //        var devices = await _userService.GetUserDevicesAsync(username);
        //        return Ok(devices);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}

        [Authorize]
        [HttpGet("GetDeviceInfo/{username}")]
        public async Task<IActionResult> GetDeviceInfo(string username)
        {
            try
            {
                // Kullanıcıya ait cihaz ID'lerini al
                var deviceIDs = await _userService.GetUserDevicesAsync(username);

                // Donanım bilgileri koleksiyonundan bu ID'lere ait bilgileri getir
                var devicesInfo = await _donanimBilgileriCollection.Find(d => deviceIDs.Contains(d.DeviceID)).ToListAsync();

                return Ok(devicesInfo);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
