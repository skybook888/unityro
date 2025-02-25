﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class OutPacket {

    private PacketHeader Header;
    public int Size;

    private bool IsFixed => Size > 0;
    private IEnumerable<byte> buffer = new List<byte>();
    private Stream Stream;

    public OutPacket(PacketHeader header, int size) {
        Header = header;
        Size = size;
        Stream = NetworkClient.GetStream();
    }

    public virtual void Send() {
        IEnumerable<byte> packet = BitConverter.GetBytes((ushort)Header);
        if (!IsFixed) {
            Size = buffer.Count() + 4;
            packet = packet.Concat(BitConverter.GetBytes((short)Size));
        }
        packet = packet.Concat(buffer);

        Debug.Log($"<color='green'>Sent: {Header} \tSize:{Size}</color>");
        Stream.Write(packet.ToArray(), 0, packet.Count());
        Stream.Flush();
        buffer = new List<byte>();
    }

    public void Write(int value) => buffer = buffer.Concat(BitConverter.GetBytes(value));
    public void Write(long value) => buffer = buffer.Concat(BitConverter.GetBytes(value));
    public void Write(byte value) => buffer = buffer.Append(value);
    public void Write(short value) => buffer = buffer.Concat(BitConverter.GetBytes(value));
    public void Write(ushort value) => buffer = buffer.Concat(BitConverter.GetBytes(value));
    public void Write(ulong value) => buffer = buffer.Concat(BitConverter.GetBytes(value));
    public void Write(uint value) => buffer = buffer.Concat(BitConverter.GetBytes(value));
    public void Write(string value) => buffer = buffer.Concat(Encoding.ASCII.GetBytes(value));
    public void Write(string value, int size) {
        byte[] chunk = new byte[size];
        for (int i = 0; i < size; i++) {
            if (i < value.Length)
                chunk[i] = (byte)value[i];
            else
                chunk[i] = 0;
        }

        buffer = buffer.Concat(chunk);
    }
    public void WritePos(short x, short y, byte dir) {
        Write((byte)(x >> 2));
        Write((byte)((x << 6) | ((y >> 4) & 0x3f)));
        Write((byte)((y << 4) | (dir & 0xf)));
    }
}