/***************************************************************************
 * HaproxyHeader - the HAProxy PROXY protocol v1 line of --haproxy-protocol / --haproxy-clientip
 *
 * Transpiled from curl's lib/cf-haproxy.c (cf_haproxy_date_out_set). The connect callback in
 * CurlTransport writes it as the first bytes of each new connection (.NET 5+).
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace CurlDotNet.Core
{
    /// <summary>Builds curl's HAProxy PROXY protocol v1 header line.</summary>
    internal static class HaproxyHeader
    {
        /// <summary>
        /// The line curl sends: <c>PROXY TCP4|TCP6 SRC DST SPORT DPORT\r\n</c>, with the connection's local and remote
        /// addresses and ports. With --haproxy-clientip that text is both addresses and decides TCP4 (a dotted IPv4
        /// address) or TCP6 (anything else); the ports stay the connection's.
        /// </summary>
        internal static string Line(WireConnectionInfo info, string? clientIp)
        {
            var local = info.LocalEndPoint;
            var remote = info.RemoteEndPoint;
            string source;
            string destination;
            bool ipv6;
            if (!string.IsNullOrEmpty(clientIp))
            {
                source = destination = clientIp!;
                ipv6 = !IsIPv4Address(clientIp!);
            }
            else
            {
                source = AddressText(local?.Address);
                destination = AddressText(remote?.Address);
                ipv6 = (remote ?? local)?.AddressFamily == AddressFamily.InterNetworkV6;
            }
            return string.Format(CultureInfo.InvariantCulture, "PROXY {0} {1} {2} {3} {4}\r\n",
                ipv6 ? "TCP6" : "TCP4", source, destination, local?.Port ?? 0, remote?.Port ?? 0);
        }

        private static string AddressText(IPAddress? address)
        {
            if (address == null)
            {
                return string.Empty;
            }
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
            }
            if (address.AddressFamily == AddressFamily.InterNetworkV6 && address.ScopeId != 0)
            {
                address = new IPAddress(address.GetAddressBytes()); // inet_ntop prints no %scope
            }
            return address.ToString();
        }

        /// <summary>curl's Curl_is_ipv4addr: exactly four dot-separated decimal numbers 0-255.</summary>
        internal static bool IsIPv4Address(string text)
        {
            var parts = text.Split('.');
            if (parts.Length != 4)
            {
                return false;
            }
            foreach (var part in parts)
            {
                if (part.Length == 0 || part.Length > 3 || (part.Length > 1 && part[0] == '0'))
                {
                    return false; // inet_pton4 refuses leading zeros
                }
                foreach (var c in part)
                {
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }
                if (int.Parse(part, CultureInfo.InvariantCulture) > 255)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
