// Copyright 2020 Felix Kahle. All rights reserved.

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// The build type.
    /// </summary>
    public enum BuildType
    {
        /// <summary>
        /// Build for the client.
        /// </summary>
        Client = 1 << 0,

        /// <summary>
        /// Build for the server.
        /// </summary>
        Server = 1 << 1
    }
}
