using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;


public static class SerialPortExtensions
{
    public async static Task ReadAsync(this SerialPort serialPort, byte[] buffer, int offset, int count)
    {
        var bytesToRead = count;
        var temp = new byte[count];

        while (bytesToRead > 0)
        {
            var readBytes = await serialPort.BaseStream.ReadAsync(temp, 0, bytesToRead);
            Array.Copy(temp, 0, buffer, offset + count - bytesToRead, readBytes);
            bytesToRead -= readBytes;
        }
    }

    public async static Task<byte[]> ReadAsync(this SerialPort serialPort, int count)
    {
        var buffer = new byte[count];
        await serialPort.ReadAsync(buffer, 0, count);
        return buffer;
    }
    /*
    public async static Task<string> ReadAsyncLine(this SerialPort serialPort)
    {
        var lastByte = ' ';
        var temp = new byte[1];
        var byteList = new List <byte>();

        while (lastByte != '\n')
        {
            lastByte = (char) await serialPort.BaseStream.ReadAsync(temp, 0, 1);
            var readBytes = await serialPort.BaseStream.ReadAsync(temp, 0, 1);
            Array.Copy(temp, 0, buffer, offset + count - bytesToRead, readBytes);
            bytesToRead -= readBytes;
        }
    }
    */
}
