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
	Friend Structure AMRecoveryDevice
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := 8)> _
		Public unknown0 As Byte()
		Public callback As DeviceRestoreNotificationCallback
		Public user_info As IntPtr
		<MarshalAs(UnmanagedType.ByValArray, SizeConst := 12)> _
		Public unknown1 As Byte()
		Public readwrite_pipe As UInteger
		Public read_pipe As Byte
		Public write_ctrl_pipe As Byte
		Public read_unknown_pipe As Byte
		Public write_file_pipe As Byte
		Public write_input_pipe As Byte
	End Structure
End Namespace
