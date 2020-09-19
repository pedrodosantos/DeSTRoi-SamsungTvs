// DeSTRoi.Libraries.Network.LocalIPScanner
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace DeSTRoi.Libraries.Network
{
	public class LocalIPScanner
	{
		public class NetworkSearchEventArgs : EventArgs
		{
			private List<string> _localIPs;

			public List<string> LocalIPs => _localIPs;

			public NetworkSearchEventArgs(List<string> localIPs)
			{
				_localIPs = localIPs;
			}
		}

		public delegate void NetworkSearchCompleteEventHandler(object sender, NetworkSearchEventArgs e);

		private List<string> _localIPs;

		private CountdownEvent countdown;

		private BackgroundWorker bgw;

		private Semaphore teSem;

		public List<string> LocalIPs => _localIPs;

		public event NetworkSearchCompleteEventHandler NetworkSearchComplete;

		public LocalIPScanner()
		{
			bgw = new BackgroundWorker();
			bgw.DoWork += bgw_DoWork;
			bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
			_localIPs = new List<string>();
		}

		protected void OnNetworkSearchComplete(List<string> localIPs)
		{
			if (this.NetworkSearchComplete != null)
			{
				this.NetworkSearchComplete(this, new NetworkSearchEventArgs(localIPs));
			}
		}

		private string GetLocalStaticIPPart()
		{
			string text = "";
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					text = $"{iPAddress.GetAddressBytes()[0]}.{iPAddress.GetAddressBytes()[1]}.{iPAddress.GetAddressBytes()[2]}.";
					break;
				}
			}
			if (text == "")
			{
				throw new FileNotFoundException("No Local IPv4 Address found!");
			}
			return text;
		}

		public List<string> FindLocalIPs()
		{
			_localIPs.Clear();
			string localStaticIPPart = GetLocalStaticIPPart();
			for (int i = 1; i < 256; i++)
			{
				using (Ping ping = new Ping())
				{
					if (ping.Send(localStaticIPPart + i, 100).Status == IPStatus.Success)
					{
						_localIPs.Add(localStaticIPPart + i);
					}
				}
			}
			return _localIPs;
		}

		public void FindLocalIPsAsync()
		{
			teSem = new Semaphore(0, 1);
			bgw.RunWorkerAsync();
		}

		private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			OnNetworkSearchComplete(_localIPs);
		}

		private void bgw_DoWork(object sender, DoWorkEventArgs e)
		{
			_localIPs.Clear();
			string localStaticIPPart = GetLocalStaticIPPart();
			countdown = new CountdownEvent(1);
			for (int i = 1; i < 256; i++)
			{
				string text = localStaticIPPart + i;
				Ping ping = new Ping();
				ping.PingCompleted += p_PingCompleted;
				countdown.AddCount();
				ping.SendAsync(text, 200, text);
			}
			countdown.Signal();
			countdown.Wait();
		}

		private void p_PingCompleted(object sender, PingCompletedEventArgs e)
		{
			string item = (string)e.UserState;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success)
			{
				_localIPs.Add(item);
			}
			countdown.Signal();
		}
	}
}
