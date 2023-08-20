namespace Base16384;
public static class Base16384 {
	public static ulong ReverseEndian(ulong value) =>
		BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse().ToArray());

	public static ushort ReverseEndian(ushort value) =>
		BitConverter.ToUInt16(BitConverter.GetBytes(value).Reverse().ToArray());

	public static ulong HostToBigEndian(ulong value) =>
		BitConverter.IsLittleEndian ? ReverseEndian(value) : value;

	public static ushort HostToBigEndian(ushort value) =>
		BitConverter.IsLittleEndian ? ReverseEndian(value) : value;

	public static ulong BigEndianToHost(ulong value) =>
		BitConverter.IsLittleEndian ? ReverseEndian(value) : value;


	public static unsafe int Encode(byte* data, int dlen, byte* buf, int blen) {
		var outlen = dlen / 7 * 8;
		var offset = dlen % 7;
		switch (offset) {   // 算上偏移标志字符占用的2字节
			case 0: break;
			case 1: outlen += 4; break;
			case 2:
			case 3: outlen += 6; break;
			case 4:
			case 5: outlen += 8; break;
			case 6: outlen += 10; break;
			default: break;
		}
		var vals = (ulong*)buf;
		ulong n = 0;
		long i = 0;
		for (; i <= dlen - 7; i += 7) {
			ulong sum = 0;
			var shift = HostToBigEndian(*(ulong*)(data + i)) >> 2; // 这里有读取越界
			sum |= shift & 0x3fff000000000000;
			shift >>= 2;
			sum |= shift & 0x00003fff00000000;
			shift >>= 2;
			sum |= shift & 0x000000003fff0000;
			shift >>= 2;
			sum |= shift & 0x0000000000003fff;
			sum += 0x4e004e004e004e00;
			vals[n++] = BigEndianToHost(sum);
		}
		var o = offset;
		if (Convert.ToBoolean(o--)) {
			var sum = 0x000000000000003f & ((ulong)data[i] >> 2);
			sum |= ((ulong)data[i] << 14) & 0x000000000000c000;
			if (Convert.ToBoolean(o--)) {
				sum |= ((ulong)data[i + 1] << 6) & 0x0000000000003f00;
				sum |= ((ulong)data[i + 1] << 20) & 0x0000000000300000;
				if (Convert.ToBoolean(o--)) {
					sum |= ((ulong)data[i + 2] << 12) & 0x00000000000f0000;
					sum |= ((ulong)data[i + 2] << 28) & 0x00000000f0000000;
					if (Convert.ToBoolean(o--)) {
						sum |= ((ulong)data[i + 3] << 20) & 0x000000000f000000;
						sum |= ((ulong)data[i + 3] << 34) & 0x0000003c00000000;
						if (Convert.ToBoolean(o--)) {
							sum |= ((ulong)data[i + 4] << 26) & 0x0000000300000000;
							sum |= ((ulong)data[i + 4] << 42) & 0x0000fc0000000000;
							if (Convert.ToBoolean(o--)) {
								sum |= ((ulong)data[i + 5] << 34) & 0x0000030000000000;
								sum |= ((ulong)data[i + 5] << 48) & 0x003f000000000000;
							}
						}
					}
				}
			}
			sum += 0x004e004e004e004e;
			vals[n] = BitConverter.IsLittleEndian ? sum : ReverseEndian(sum);
			buf[outlen - 2] = (byte)'=';
			buf[outlen - 1] = (byte)offset;
		}
		return outlen;
	}


	public static unsafe int Decode(byte* data, int dlen, byte* buf, int blen) {
		var outlen = dlen;
		var offset = 0;
		if (data[dlen - 2] == '=') {
			offset = data[dlen - 1];
			switch (offset) {   // 算上偏移标志字符占用的2字节
				case 0: break;
				case 1: outlen -= 4; break;
				case 2:
				case 3: outlen -= 6; break;
				case 4:
				case 5: outlen -= 8; break;
				case 6: outlen -= 10; break;
				default: break;
			}
		}
		outlen = (outlen / 8 * 7) + offset;
		var vals = (ulong*)data;
		ulong n = 0;
		long i = 0;
		for (; i <= outlen - 7; n++, i += 7) {
			ulong sum = 0;
			var shift = HostToBigEndian(vals[n]) - 0x4e004e004e004e00;
			shift <<= 2;
			sum |= shift & 0xfffc000000000000;
			shift <<= 2;
			sum |= shift & 0x0003fff000000000;
			shift <<= 2;
			sum |= shift & 0x0000000fffc00000;
			shift <<= 2;
			sum |= shift & 0x00000000003fff00;
			*(ulong*)(buf + i) = BigEndianToHost(sum);
		}
		if (Convert.ToBoolean(offset--)) {
			// 这里有读取越界
			var sum = (BitConverter.IsLittleEndian ? vals[n] : ReverseEndian(vals[n])) - 0x000000000000004e;
			buf[i++] = (byte)(((sum & 0x000000000000003f) << 2) | ((sum & 0x000000000000c000) >> 14));
			if (Convert.ToBoolean(offset--)) {
				sum -= 0x00000000004e0000;
				buf[i++] = (byte)(((sum & 0x0000000000003f00) >> 6) | ((sum & 0x0000000000300000) >> 20));
				if (Convert.ToBoolean(offset--)) {
					buf[i++] = (byte)(((sum & 0x00000000000f0000) >> 12) | ((sum & 0x00000000f0000000) >> 28));
					if (Convert.ToBoolean(offset--)) {
						sum -= 0x0000004e00000000;
						buf[i++] = (byte)(((sum & 0x000000000f000000) >> 20) | ((sum & 0x0000003c00000000) >> 34));
						if (Convert.ToBoolean(offset--)) {
							buf[i++] = (byte)(((sum & 0x0000000300000000) >> 26) | ((sum & 0x0000fc0000000000) >> 42));
							if (Convert.ToBoolean(offset--)) {
								sum -= 0x004e000000000000;
								buf[i] = (byte)(((sum & 0x0000030000000000) >> 34) | ((sum & 0x003f000000000000) >> 48));
							}
						}
					}
				}
			}
		}
		return outlen;
	}
}