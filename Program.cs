﻿using System;
using System.Linq;
using System.Collections.Generic;
using SharpPcap;
using PacketDotNet;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SharpPcap.LibPcap;
using System.Net.NetworkInformation;

class Program
{
    static void Main(string[] args)
    {
        string? interfaceName = null;
        bool showInterfaces = false;
        bool tcp = false;
        bool udp = false;
        int? port = null;
        int? sourcePort = null;
        int? destinationPort = null;
        bool arp = false;
        bool ndp = false;
        bool icmp4 = false;
        bool icmp6 = false;
        bool igmp = false;
        bool mld = false;
        int numberOfPackets = 1;

        for (int i = 0; i < args.Length; i++){
            switch (args[i]){
                case "-i":
                case "--interface":
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-")) {
                        interfaceName = args[i + 1];
                    }
                    else {
                        showInterfaces = true;
                    }
                    break;
                case "-p":
                    port = int.Parse(args[i + 1]);
                    break;
                case "--port-source":
                    sourcePort = int.Parse(args[i + 1]);
                    break;
                case "--port-destination":
                    destinationPort = int.Parse(args[i + 1]);
                    break;
                case "--tcp":
                case "-t":
                    tcp = true;
                    break;
                case "--udp":
                case "-u":
                    udp = true;
                    break;
                case "--arp":
                    arp = true;
                    break;
                case "--icmp4":
                    icmp4 = true;
                    break;
                case "--icmp6":
                    icmp6 = true;
                    break;
                case "--igmp":
                    igmp = true;
                    break;
                case "--mld":
                    mld = true;
                    break;
                case "-n":
                    numberOfPackets = int.Parse(args[i + 1]);
                    break;
                default:
                    break;
            }
        }
        var devices = CaptureDeviceList.Instance;
        if (showInterfaces)
        {
            foreach (var dev in devices)
            {
                Console.WriteLine(dev.Description);
            }
            Environment.Exit(0);
        }
        LibPcapLiveDevice? device = null;
        foreach (var dev in devices)
        {
            if (interfaceName == dev.Name) {
                device = dev;
                break;
            }
        }
        if (device != null) {
            device.OnPacketArrival += PacketHandler;
            device.Open(DeviceMode.Promiscuous, 3000);

            device.StartCapture();

            Console.CancelKeyPress += (sender, e) =>
            {
                device.StopCapture();
                device.Close();
                Environment.Exit(0);
            };
        }

    }
    static void PacketHandler(object sender, CaptureEventArgs e)
    {
        var time = e.Packet.Timeval.Date;
        var output = new StringBuilder();

        output.AppendLine($"timestamp: {time}");

        Console.WriteLine(output.ToString());
    }
}