﻿using RealisticDependencies;
using System;
using System.Threading.Tasks;

namespace BehavioralPatterns.Mediator.FoodCarts {
    public class BicycleCart : FoodCart {
        private IMediator _network;
        private readonly IApplicationLogger _logger;
        private readonly IDatabase _database;
        private readonly string _handle;

        public BicycleCart(
            string handle, 
            int lat, 
            int lon, 
            IApplicationLogger logger, 
            IDatabase database) : base(logger, handle, lat, lon) {
            _database = database;
            _database.Connect().Wait();
        }

        public override async Task Receive(NetworkMessage message) {
            await Task.Delay(500);
            var payload = message.Read();
            var sendTime = message.GetTimestamp();
            _logger.LogInfo($"[{_handle}] Received Message at {payload}: ({sendTime})", ConsoleColor.Magenta);
            if (payload.Contains("thanks bikes")) {
                var returnMessage = new NetworkMessage("no problem! 👍");
                returnMessage.Sign(this);
                await _network.Broadcast(returnMessage);
            }
        }

        public override async Task Send(ICommunicates receiver, NetworkMessage message) {
            await Task.Delay(500);
            message.Sign(this);
            await _network.DeliverPayload(receiver.Handle, message);
        }

    }
}