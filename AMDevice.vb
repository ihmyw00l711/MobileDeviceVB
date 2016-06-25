'MIT License
'
'Copyright (c) 2016 [ihmyw00l711]
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.

Imports System.Runtime.InteropServices
Namespace MobileDeviceVB

	<StructLayout(LayoutKind.Sequential, Pack := 1)> _
	Public Structure AMDevice
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := &H10)> _
		Friend unknown0 As Byte()
		Friend device_id As UInteger
		Friend product_id As UInteger
		Public serial As String
		Friend unknown1 As UInteger
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := 4)> _
		Friend unknown2 As Byte()
		Friend lockdown_conn As UInteger
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := 8)> _
		Friend unknown3 As Byte()
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := &H61)> _
		Friend unknown4 As Byte()
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := 8)> _
		Friend unknown5 As Byte()
	End Structure
End Namespace

