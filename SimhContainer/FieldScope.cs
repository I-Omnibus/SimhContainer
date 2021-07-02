using System;
using Omnibus.Library.Data;

namespace Omnibus.Simh.Container {

	[MapLayout(Size: 512)]
	public enum FieldScope {
		[Map(Offset: 000, Length: 004, Encoding: typeof(byte[]))] Signature,
		[Map(Offset: 004, Length: 064, Encoding: typeof(byte[]))] CreatingSimulator,
		[Map(Offset: 068, Length: 016, Encoding: typeof(byte[]))] DriveType,
		[Map(Offset: 084, Length: 004, Encoding: typeof(UInt32))] SectorSize,
		[Map(Offset: 088, Length: 004, Encoding: typeof(UInt32))] SectorCount,
		[Map(Offset: 092, Length: 004, Encoding: typeof(UInt32))] TransferElementSize,
		[Map(Offset: 096, Length: 028, Encoding: typeof(byte[]))] CreationTime,
		[Map(Offset: 124, Length: 001, Encoding: typeof(byte))] FooterVersion,
		[Map(Offset: 125, Length: 001, Encoding: typeof(byte))] AccessFormat,
		[Map(Offset: 126, Length: 382, Encoding: typeof(byte[]))] Reserved,
		[Map(Offset: 508, Length: 004, Encoding: typeof(byte[]))] Checksum,
	}
}
