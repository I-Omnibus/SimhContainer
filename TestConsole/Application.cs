using System;
using Omnibus.Simh.Container;

var info = new InformationBlock(@"..\..\..\TestData\SingleDiskBlockImageSample.bin");

Console.WriteLine(new {
	Signature            /**/ = info.GetField<string>(FieldScope.Signature),
	CreatingSimulator    /**/ = info.GetField<string>(FieldScope.CreatingSimulator),
	DriveType            /**/ = info.GetField<string>(FieldScope.DriveType),
	SectorSize           /**/ = info.GetField<UInt32>(FieldScope.SectorSize),
	SectorCount          /**/ = info.GetField<UInt32>(FieldScope.SectorCount),
	TransferElementSize  /**/ = info.GetField<UInt32>(FieldScope.TransferElementSize),
	CreationTime         /**/ = info.GetField<string>(FieldScope.CreationTime),
	FooterVersion        /**/ = info.GetField<byte>(FieldScope.FooterVersion),
	AccessFormat         /**/ = info.GetField<byte>(FieldScope.AccessFormat),
	Checksum             /**/ = info.GetField<UInt32>(FieldScope.Checksum),
});

var checksumVerified = info.CalculatedChecksum == info.GetField<UInt32>(FieldScope.Checksum);
Console.WriteLine(new { checksumVerified });
