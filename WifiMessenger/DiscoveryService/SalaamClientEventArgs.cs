using System;

namespace Dolphins.Salaam
{
    /// <summary>
    /// Provides data for the <see cref="SalaamBrowser"/> events.
    /// </summary>
    public class SalaamClientEventArgs:EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SalaamClientEventArgs"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="isFromLocalMachine">if set to <c>true</c> this instance is from the local machine.</param>
        public SalaamClientEventArgs(SalaamClient client, bool isFromLocalMachine)
        {
            Client = client;

            IsFromLocalMachine = isFromLocalMachine;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        public SalaamClient Client { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is from local machine.
        /// </summary>
        public bool IsFromLocalMachine { get; private set; }
    }
}
