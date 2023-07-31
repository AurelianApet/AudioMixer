using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.CoreAudioApi
{
    /// <summary>
    /// Windows CoreAudio DeviceTopology
    /// </summary>
    public class DeviceTopology
    {
        private readonly IDeviceTopology deviceTopologyInterface;

        internal DeviceTopology(IDeviceTopology deviceTopology)
        {
            deviceTopologyInterface = deviceTopology;
        }

        /// <summary>
        /// Retrieves the number of connections associated with this device-topology object
        /// </summary>
        public uint ConnectorCount
        {
            get
            {
                uint count;
                deviceTopologyInterface.GetConnectorCount(out count);
                return count;
            }
        }

        /// <summary>
        /// Retrieves the connector at the supplied index
        /// </summary>
        public Connector GetConnector(uint index)
        {
            IConnector connectorInterface;
            deviceTopologyInterface.GetConnector(index, out connectorInterface);
            return new Connector(connectorInterface);
        }

        /// <summary>
        /// Retrieves the device id of the device represented by this device-topology object
        /// </summary>
        public string DeviceId
        {
            get
            {
                string result;
                deviceTopologyInterface.GetDeviceId(out result);
                return result;
            }
        }

    }
}
