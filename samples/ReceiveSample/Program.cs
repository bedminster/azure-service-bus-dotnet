﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ReceiveSample
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;

    public class Program
    {
        private static IQueueClient queueClient;
        private const string ServiceBusConnectionString = "{Service Bus connection string}";
        private const string QueueName = "{Queue path/name}";

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            ReceiveMessages();

            Console.WriteLine("Press any key to stop receiving messages.");
            Console.ReadKey();

            // Close the client after the ReceiveMessages method has exited.
            await queueClient.CloseAsync();
        }

        // Receives messages from the queue in a loop
        private static void ReceiveMessages()
        {
            try
            {
                // Register a OnMessage callback
                queueClient.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        // Process the message
                        Console.WriteLine($"Received message: SequenceNumber:{message.SequenceNumber} Body:{message.GetBody<string>()}");

                        // Complete the message so that it is not received again.
                        // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode.
                        await queueClient.CompleteAsync(message.LockToken);
                    },
                    new RegisterHandlerOptions() {MaxConcurrentCalls = 1, AutoComplete = false});
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }
    }
}
