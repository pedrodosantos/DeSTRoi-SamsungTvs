// MinimalisticTelnet.TelnetConnection
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace MinimalisticTelnet
{
  public class TelnetConnection
  {
    private TcpClient tcpSocket;

    private int TimeOutMs = 100;

    public bool IsConnected => tcpSocket.Connected;

    public TelnetConnection(string Hostname, int Port)
    {

      tcpSocket = new TcpClient(Hostname, Port);
    }

    public string Login(string Username, string Password, int LoginTimeOutMs)
    {
      int timeOutMs = TimeOutMs;
      TimeOutMs = LoginTimeOutMs;
      string text = Read();
      if (!text.TrimEnd().EndsWith(":"))
      {
        throw new Exception("Failed to connect : no login prompt");
      }
      WriteLine(Username);
      text += Read();
      if (!text.TrimEnd().EndsWith(":"))
      {
        throw new Exception("Failed to connect : no password prompt");
      }
      WriteLine(Password);
      text += Read();
      TimeOutMs = timeOutMs;
      return text;
    }

    public void WriteLine(string cmd)
    {
      Write(cmd + "\n");
    }

    public void Write(string cmd)
    {
      if (tcpSocket.Connected)
      {
        byte[] bytes = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
        tcpSocket.GetStream().Write(bytes, 0, bytes.Length);
      }
    }

    public string Read()
    {
      if (!tcpSocket.Connected)
      {
        return null;
      }
      StringBuilder stringBuilder = new StringBuilder();
      do
      {
        ParseTelnet(stringBuilder);
        Thread.Sleep(TimeOutMs);
      }
      while (tcpSocket.Available > 0);
      return stringBuilder.ToString();
    }

    private void ParseTelnet(StringBuilder sb)
    {
      while (tcpSocket.Available > 0)
      {
        int num = tcpSocket.GetStream().ReadByte();
        switch (num)
        {
          case 255:
            {
              int num2 = tcpSocket.GetStream().ReadByte();
              switch (num2)
              {
                case 255:
                  sb.Append(num2);
                  break;
                case 251:
                case 252:
                case 253:
                case 254:
                  {
                    int num3 = tcpSocket.GetStream().ReadByte();
                    if (num3 != -1)
                    {
                      tcpSocket.GetStream().WriteByte(byte.MaxValue);
                      if (num3 == 3)
                      {
                        tcpSocket.GetStream().WriteByte((byte)((num2 == 253) ? 251 : 253));
                      }
                      else
                      {
                        tcpSocket.GetStream().WriteByte((byte)((num2 == 253) ? 252 : 254));
                      }
                      tcpSocket.GetStream().WriteByte((byte)num3);
                    }
                    break;
                  }
              }
              break;
            }
          default:
            sb.Append((char)num);
            break;
          case -1:
            break;
        }
      }
    }
  }
}