using System;
using System.Linq;

namespace Heijden.DNS
{
    public class RecordUnknown : Record
	{
		public byte[] RDATA;
		public RecordUnknown(RecordReader rr)
		{
			// re-read length
			ushort RDLENGTH = rr.ReadUInt16(-2);
			RDATA = rr.ReadBytes(RDLENGTH);
		}

        public override string ToString()
        {
            if (RDATA == null)
                return "RDATA = null";
            return "RDATA = [" + string.Join(" ", RDATA.Select(b => b.ToString("x2"))) + "]";
        }
	}
}
