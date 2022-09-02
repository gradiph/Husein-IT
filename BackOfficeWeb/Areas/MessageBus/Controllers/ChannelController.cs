﻿using BackOfficeWeb.Interfaces;
using BackOfficeWeb.Models.MessageBus;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeWeb.Areas.MessageBus.Controllers
{
    [Area("MessageBus")]
    public class ChannelController : Controller
    {
        private readonly IMessageBusService _messageBusService;

        public ChannelController(IMessageBusService messageBusService)
        {
            _messageBusService = messageBusService;
        }

        public async Task<IActionResult> Index()
        {
            ChannelViewModel channelViewModel = new ChannelViewModel();
            channelViewModel.Channels = await _messageBusService.GetAllChannelsAsync();
            return View(channelViewModel);
        }

        public IActionResult Show()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
